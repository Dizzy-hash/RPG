using UnityEngine;
using System.Collections;

namespace BT
{
    public class Log : Action
    {
        public string Text = string.Empty;
        public string Level = string.Empty;

        protected override void ReadAttribute(string key, string value)
        {
            switch (key)
            {
                case "Text":
                    Text = value;
                    break;
                case "Level":
                    Level = value;
                    break;
            }
        }

        protected override EBTStatus Execute()
        {
            switch (Level)
            {
                case "1":
                    Debug.Log(Text);
                    break;
                case "2":
                    Debug.LogWarning(Text);
                    break;
                case "3":
                    Debug.LogError(Text);
                    break;
                case "Time":
                    Debug.Log(Time.time);
                    break;
                default:
                    Debug.Log(Text);
                    break;
            }
            return EBTStatus.BT_SUCCESS;
        }

        public override BTNode DeepClone()
        {
            Log log = new Log();
            log.Level = this.Level;
            log.Text = this.Text;
            return log;
        }
    }
}
