using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NLE.Core;
using System;
using System.Xml;
using System.Reflection;

namespace ACT
{
    [Serializable]
    public class ActItem
    {
        public    ActSkill           Skill           { get; set; }          
        public    ActItem            Parent          { get; set; }
        public    Transform          ApplyCenter     { get; set; }
        public    Vector3            ApplyHitPoint   { get; set; }
        public    Vector3            ApplyDir        { get; set; }


        public    EActType           ActType         { get; protected set; }
        public    EActEventType      EventType       { get; protected set; }
        public    EActStatus         Status          { get; protected set; }

        public    float              Duration        { get { return EdTime - StTime; } }
        public    float              ElapsedTime     { get { return Time.time - this.StartupTime; } }
        public    float              StartupTime     { get; protected set; }
        public    bool               CloneObj        { get; protected set; }

        [SerializeField]
        public float                 StTime;
        [SerializeField]
        public float                 EdTime;
        [SerializeField]
        public List<ActItem>         Children     = new List<ActItem>();

        protected HashSet<int>        mFinishList = new HashSet<int>();
        protected List<Character>     mTargetList = new List<Character>();


        public virtual void AddChild(ActItem item)
        {
            Children.Add(item);
            item.Parent = this;
        }

        public virtual void DelChild(ActItem item)
        {
            Children.Remove(item);
        }

        protected virtual void Execute()
        {
            this.Status = EActStatus.RUNNING;
        }

        protected virtual bool Trigger()
        {
            this.Status = EActStatus.TRIGGER;
            return true;
        }

        protected virtual void End()
        {
            this.Status = EActStatus.SELFEND;
        }

        protected virtual void Exit()
        {
            this.Status = EActStatus.SUCCESS;
        }

        protected virtual void ExecuteChildren()
        {
            ExecuteActions(Children);
        }

        protected void ExecuteActions(List<ActItem> list)
        {
            if (mFinishList.Count >= list.Count)
            {
                Exit();
                return;
            }
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (mFinishList.Contains(i))
                    {
                        continue;
                    }
                    ActItem item = list[i];
                    item.Loop();
                    if (item.Status == EActStatus.SUCCESS)
                    {
                        mFinishList.Add(i);
                    }
                }
            }
            else
            {
                Exit();
            }
        }

        protected void RunClone()
        {
            if (!CloneObj)
            {
                ActItem item = Clone(this);
                item.CloneObj = true;
                item.Skill    = Skill;
                item.Clear();
                GTWorld.Instance.Act.Run(item);
                Exit();
            }
        }

        protected ActItem Clone(ActItem src)
        {
            ActItem item = (ActItem)Activator.CreateInstance(src.GetType());
            FieldInfo[] fields = item.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo newField = fields[i];
                if (newField.IsDefined(typeof(SerializeField), true) &&
                    newField.FieldType != typeof(List<ActItem>))
                {
                    FieldInfo oldField = src.GetType().GetField(newField.Name);
                    if (oldField != null)
                    {
                        newField.SetValue(item, oldField.GetValue(src));
                    }
                }
            }
            for (int i = 0; i < src.Children.Count; i++)
            {
                ActItem child = Clone(src.Children[i]);
                item.AddChild(child);
            }
            return item;
        }

        protected T Clone<T>(ActItem src) where T : ActItem
        {
            T item = Activator.CreateInstance<T>();
            FieldInfo[] fields = item.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo newField = fields[i];
                if (newField.IsDefined(typeof(SerializeField), true) &&
                    newField.FieldType != typeof(List<ActItem>))
                {
                    FieldInfo oldField = src.GetType().GetField(newField.Name);
                    if (oldField != null)
                    {
                        newField.SetValue(item, oldField.GetValue(src));
                    }
                }
            }
            for (int i = 0; i < src.Children.Count; i++)
            {
                ActItem child = Clone(src.Children[i]);
                item.AddChild(child);
            }
            return item;
        }

        public virtual void Begin()
        {
            this.Skill = this.Skill == null ? Parent.Skill : Skill;
            this.StartupTime =  Time.time;
            this.Status = EActStatus.STARTUP;
        }

        public virtual void Loop()
        {
            if (Status == EActStatus.INITIAL)
            {
                Begin();
            }
            if (ElapsedTime < StTime)
            {
                return;
            }
            if (Status == EActStatus.SELFEND)
            {
                ExecuteChildren();
            }
            else
            {
                switch (EventType)
                {
                    case EActEventType.Instant:
                        {
                            DoInstant();
                        }
                        break;
                    case EActEventType.Subtain:
                        {
                            DoSubtain();
                        }
                        break;
                    case EActEventType.Special:
                        {
                            DoSpecial();
                        }
                        break;
                }
            }
        }

        public virtual void Clear()
        {
            this.Status = EActStatus.INITIAL;
            this.Children.ForEach((item) => item.Clear());
            this.mFinishList.Clear();
            this.mTargetList.Clear();
        }

        public virtual void Stop()
        {
            this.Status = EActStatus.INITIAL;
            this.Children.ForEach((item) => item.Stop());
        }

        protected virtual void DoInstant()
        {
            if (Status == EActStatus.STARTUP)
            {
                if(Trigger())
                {
                    Execute();
                    End();
                }
                else
                {
                    Exit();
                }
            }
        }

        protected virtual void DoSubtain()
        {
            if (Status == EActStatus.STARTUP)
            {
                if (!Trigger())
                {
                    Exit();
                }
            }
            Execute();
            if (ElapsedTime >= EdTime && Status == EActStatus.RUNNING)
            {
                End();
            }
        }

        protected virtual void DoSpecial()
        {
            if (Status == EActStatus.STARTUP)
            {
                if (!Trigger())
                {
                    Exit();
                }
            }
        }

        public void AddInAttackList(Character cc)
        {
            mTargetList.Add(cc);
        }

        public void ClearAttackList()
        {
            mTargetList.Clear();
        }

        public void LoadDoc(XmlElement element)
        {
            FieldInfo[] fields = this.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                Type t = field.FieldType;
                string value = element.GetAttribute(field.Name);
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }
                object v = default(object);
                if (t.BaseType == typeof(Enum))
                {
                    v = Enum.ToObject(t, value.ToInt32());
                }
                else if (t == typeof(int))
                {
                    v = value.ToInt32();
                }
                else if (t == typeof(bool))
                {
                    v = value == "true";
                }
                else if (t == typeof(float))
                {
                    v = value.ToFloat();
                }
                else if (t == typeof(string))
                {
                    v = value;
                }
                else if (t == typeof(Vector3))
                {
                    v = value.ToVector3(true);
                }
                else if (t == typeof(List<string>))
                {
                    string[] array = value.Split('~');
                    List<string> list = (List<string>)field.GetValue(this);
                    list.AddRange(array);
                    v = list;
                }
                field.SetValue(this, v);
            }

            XmlElement child = element.FirstChild as XmlElement;
            while (child != null)
            {
                Type type = System.Type.GetType("ACT" + "." + child.Name);
                ActItem act = (ActItem)System.Activator.CreateInstance(type);
                act.LoadDoc(child);
                AddChild(act);
                child = child.NextSibling as XmlElement;
            }
        }

        public void SaveDoc(XmlDocument doc, XmlElement element)
        {
            FieldInfo[] fields = this.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                Type t = field.FieldType;
                if (t.BaseType == typeof(Enum))
                {
                    element.SetAttribute(field.Name, ((int)field.GetValue(this)).ToString());
                }
                else if (t == typeof(int))
                {
                    element.SetAttribute(field.Name, field.GetValue(this).ToString());
                }
                else if (t == typeof(bool))
                {
                    element.SetAttribute(field.Name, field.GetValue(this).ToString());
                }
                else if (t == typeof(float))
                {
                    element.SetAttribute(field.Name, field.GetValue(this).ToString());
                }
                else if (t == typeof(string))
                {
                    element.SetAttribute(field.Name, field.GetValue(this).ToString());
                }
                else if (t == typeof(Vector3))
                {
                    Vector3 vector3 = (Vector3)field.GetValue(this);
                    element.SetAttribute(field.Name, GTTools.SaveVector3(vector3));
                }
            }
            for (int i = 0; i < Children.Count; i++)
            {
                ActItem act = Children[i];
                XmlElement child = doc.CreateElement(act.GetType().Name);
                element.AppendChild(child);
                act.SaveDoc(doc, child);
            }
        }

    }
}
