using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Cfg.Map;

namespace LevelDirector
{
    public class LevelRegion : LevelElement
    {
        public bool                     AllowRide   = true;
        public bool                     AllowPK     = true;
        public bool                     AllowTrade  = true;
        public bool                     AllowFight  = true;
        public bool                     AwakeActive = false;
        public Color                    color       = new Color(1, 0.92f, 0.016f, 0.35f);
        public Vector3                  Size        = new Vector3(5, 2, 5);

        public Callback<Collider>       onTriggerEnter;
        public Callback<Collider>       onTriggerStay;
        public Callback<Collider>       onTriggerExit;
        private MapRegion               mData = null;
        private Mesh                    mMesh;
        private Material                mMaterial;
        private static MeshFilter       mMeshFilter;
        private BoxCollider             mCollider;
        private List<MapEvent>          mHasActiveEvents = new List<MapEvent>();

        public List<LevelEvent> Events
        {
            get
            {
                List<LevelEvent> pList = new List<LevelEvent>();
                GetAllComponents(transform,pList);
                return pList;
            }
        }

        public LevelEvent AddElement()
        {
            LevelEvent pElem = new GameObject().AddComponent<LevelEvent>();
            pElem.transform.parent = transform;
            pElem.transform.localPosition = Vector3.zero;
            pElem.Build();
            pElem.SetName();
            return pElem;
        }

        public override void Init()
        {
            ActiveEventsByCondition(ETriggerCondition.TYPE_AWAKE_REGION);
        }

        void OnTriggerEnter(Collider other)
        {
            if(onTriggerEnter!=null)
            {
                onTriggerEnter(other);
            }
            if (other.gameObject.layer == GTLayerDefine.LAYER_AVATAR ||
                other.gameObject.layer == GTLayerDefine.LAYER_MOUNT)
            {
                ActiveEventsByCondition(ETriggerCondition.TYPE_ENTER_REGION);
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (onTriggerStay != null)
            {
                onTriggerStay(other);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (onTriggerExit != null)
            {
                onTriggerExit(other);
            }

            if (other.gameObject.layer == GTLayerDefine.LAYER_AVATAR ||
                other.gameObject.layer == GTLayerDefine.LAYER_MOUNT)
            {
                ActiveEventsByCondition(ETriggerCondition.TYPE_LEAVE_REGION);
            }
        }

        public override void Build()
        {
            mCollider = gameObject.GET<BoxCollider>();
            if (Scale.x <= 0) Size.x = 1;
            if (Scale.y <= 0) Size.y = 1;
            if (Scale.z <= 0) Size.z = 1;
            mCollider.size = Size;
            mCollider.isTrigger = true;
        }

        public override void SetName()
        {
            this.name = "Region_" + Id.ToString();
        }

        private MeshFilter   RegionMeshFilter
        {
            get
            {
                if (mMeshFilter != null)
                {
                    return mMeshFilter;
                }
                GameObject go = GTResourceManager.Instance.Load<GameObject>("3D/Cube");
                if (go != null)
                {
                    mMeshFilter = go.GetComponent<MeshFilter>();
                }
                return mMeshFilter;
            }
        }

        private int []       Triangles
        {
            get
            {
                if (RegionMeshFilter == null)
                {
                    return null;
                }
                return RegionMeshFilter.sharedMesh.triangles;
            }
        }

        private Vector3[]    Vertices
        {
            get
            {
                Vector3[] data = RegionMeshFilter.sharedMesh.vertices;
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = new Vector3(data[i].x * Size.x, data[i].y * Size.y, data[i].z * Size.z);
                }
                return data;
            }
        }

        public override CfgBase Export()
        {
            MapRegion data = new MapRegion();
            data.Id                = Id;
            data.Scale             = Size;
            data.Pos               = Pos;
            data.Euler             = Euler;
            data.AllowFight        = AllowFight;
            data.AllowPK           = AllowPK;
            data.AllowRide         = AllowRide;
            data.AllowTrade        = AllowTrade;
            data.AwakeActive       = AwakeActive;
            List<LevelEvent> pList = Events;
            for (int i = 0; i < pList.Count; i++)
            {
                data.Events.Add(pList[i].Export() as MapEvent);
            }
            return data;
        }

        public override void Import(CfgBase pData,bool pBuild)
        {
            MapRegion data = pData as MapRegion;
            Id                     = data.Id;
            Size                   = data.Scale;
            Pos                    = data.Pos;
            Euler                  = data.Euler;
            AllowFight             = data.AllowFight;
            AllowPK                = data.AllowPK;
            AllowRide              = data.AllowRide;
            AllowTrade             = data.AllowTrade;
            AwakeActive            = data.AwakeActive;
            this.mData             = data;
            this.Build();
            this.SetName();
            if(pBuild)
            {
                for (int i = 0; i < data.Events.Count; i++)
                {
                    GameObject go = NGUITools.AddChild(gameObject);
                    LevelEvent pEvent = go.AddComponent<LevelEvent>();
                    pEvent.Import(data.Events[i],pBuild);
                }
            }
        }

        public bool CheckCondition(MapEventCondition data, MapEvent pEventData, ETriggerCondition except,params object[] args)
        {
            switch(data.Type)
            {
                case ETriggerCondition.TYPE_ENTER_REGION:
                case ETriggerCondition.TYPE_AWAKE_REGION:
                case ETriggerCondition.TYPE_LEAVE_REGION:
                    {
                        return data.Type == except;
                    }
                case ETriggerCondition.TYPE_WAVES_FINISH:
                    {
                        if (except == data.Type)
                        {
                            return (int)args[0] == data.Args.ToInt32();
                        }
                        else
                        {
                            return false;
                        }
                    }
                default:
                    return false;
            }
        }

        public void ActiveEventsByCondition(ETriggerCondition type,params object [] args)
        {
            for (int i = 0; i < mData.Events.Count; i++)
            {
                MapEvent data = mData.Events[i];
                if (mHasActiveEvents.Contains(data)&&data.Conditions2==null)
                {
                    continue;
                }
                int num = 0;
                for (int k = 0; k < data.Conditions1.Count; k++)
                {
                    MapEventCondition pChild = data.Conditions1[k];
                    bool isTrigger = CheckCondition(pChild, data, type, args);
                    if (isTrigger)
                    {
                        num++;
                    }
                }

                bool active = false;
                switch(data.Relation1)
                {
                    case ECR.AND:
                        active = num >= data.Conditions1.Count;
                        break;
                    case ECR.OR:
                        active = num > 0;
                        break;
                }

                if(active)
                {
                    GTLevelManager.Instance.StartMapEvent(data,this);
                    if(!mHasActiveEvents.Contains(data))
                    {
                        mHasActiveEvents.Add(data);
                    }
                }
            }
        }

        private void DrawMeshNow()
        {
            if (null == mMesh)
            {
                mMesh = new Mesh {hideFlags = HideFlags.HideAndDontSave};
            }
            if (null == mMaterial)
            {
                Shader shader = Shader.Find("Custom/TranspUnlit");
                mMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            }
            mMaterial.color = color;
            mMesh.vertices = Vertices;
            mMesh.triangles = Triangles;
            Vector3[] vertices = mMesh.vertices;
            Vector2[] uvs = new Vector2[vertices.Length];
            int j = 0;
            while (j < uvs.Length)
            {
                uvs[j] = new Vector2(vertices[j].x, vertices[j].z);
                j++;
            }
            mMesh.uv = uvs;
            mMesh.RecalculateNormals();
            for (int i = 0; i < mMaterial.passCount; i++)
            {
                if (mMaterial.SetPass(i))
                {
                    Graphics.DrawMeshNow(mMesh,Pos,Quaternion.Euler(Euler));
                }
            }
        }

        private void OnDrawGizmos()
        {
            DrawMeshNow();
        }

        private Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            return rotation * (point - pivot) + pivot;
        }
    }
}

