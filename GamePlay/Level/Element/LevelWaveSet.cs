using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cfg.Map;

namespace LevelDirector
{
    public class LevelWaveSet : LevelContainerBase<LevelWave>
    {
        public int            CurrIndex                 { get; private set; }
        public int            CurrKillNum               { get; private set; }
        public int            CurrMonsterNum            { get; private set; }
        public bool           IsEnd                     { get; private set; }
        public LevelRegion    Region;

        private MapWaveSet    mWaveSet;
        private HashSet<int>  mMonsterGUIDSet = new HashSet<int>();
        private bool          mIsCreateDone = false;

        public override void SetName()
        {
            transform.name = "WaveSet_" + Id.ToString();
        }

        public override void Import(CfgBase pData,bool pBuild)
        {
            MapWaveSet data = pData as MapWaveSet;
            Id = data.Id;
            this.Build();
            this.SetName();
            if (pBuild)
            {
                for (int i = 0; i < data.Waves.Count; i++)
                {
                    GameObject go = NGUITools.AddChild(gameObject);
                    LevelWave pWave = go.AddComponent<LevelWave>();
                    pWave.Import(data.Waves[i],pBuild);
                }
            }
            mWaveSet = data;
        }

        public override CfgBase Export()
        {
            MapWaveSet data = new MapWaveSet();
            data.Id= Id ;
            for (int i = 0; i < Elements.Count; i++)
            {
                data.Waves.Add(Elements[i].Export() as MapMonsterWave);
            }
            return data;
        }

        public override void Init()
        {
            this.CurrIndex = 0;
            this.Enter();
            GTEventCenter.AddHandler<int,int>(GTEventID.RECV_KILL_MONSTER, OnKillMonster);
        }

        public void Enter()
        {
            CreateWaveMonsters(mWaveSet.Waves[CurrIndex]);
        }

        public void Exit()
        {
            IsEnd = true;
            if (Region != null)
            {
                Region.ActiveEventsByCondition(ETriggerCondition.TYPE_WAVES_FINISH, mWaveSet.Id);
            }
            GTEventCenter.DelHandler<int,int>(GTEventID.RECV_KILL_MONSTER, OnKillMonster);
            DestroyImmediate(gameObject);
        }

        private void CreateMonster(MapMonster data)
        {
            Vector3 bornEulerAngles = data.Euler;
            Vector3 bornPosition = GTTools.NavSamplePosition(data.Pos);
            XTransform param = XTransform.Create(bornPosition, bornEulerAngles);
            Actor pActor = GTLevelManager.Instance.AddActor(data.Id, EActorType.MONSTER,EBattleCamp.B,param);
            if (pActor != null)
            {
                mMonsterGUIDSet.Add(pActor.GUID);
            }
        }

        private IEnumerator CreateMonsterDelay(MapMonster data, float delay, bool isDone)
        {
            yield return delay;
            CreateMonster(data);
            if (isDone != mIsCreateDone)
            {
                mIsCreateDone = isDone;
            }
        }

        private void CreateWaveMonsters(MapMonsterWave pWaveData)
        {
            mIsCreateDone = false;
            switch (pWaveData.Spawn)
            {
                case EMonsterWaveSpawn.TYPE_WHOLE:
                    {
                        pWaveData.Monsters.ForEach(delegate (MapMonster data)
                        {
                            CreateMonster(data);
                        });
                        CurrIndex++;
                        mIsCreateDone = true;
                    }
                    break;
                case EMonsterWaveSpawn.TYPE_ALONG:
                    {
                        for (int i = 0; i < pWaveData.Monsters.Count; i++)
                        {
                            MapMonster data = pWaveData.Monsters[i];
                            float delay = 0.2f + 0.2f * i;
                            bool isDone = (i == pWaveData.Monsters.Count - 1);
                            GTCoroutinueManager.Instance.StartCoroutine(CreateMonsterDelay(data, delay, isDone));
                        }
                        CurrIndex++;
                    }
                    break;
                case EMonsterWaveSpawn.TYPE_RADOM:
                    {
                        int range = UnityEngine.Random.Range(0, pWaveData.Monsters.Count);
                        CreateMonster(pWaveData.Monsters[range]);
                        CurrIndex++;
                        mIsCreateDone = true;
                    }
                    break;
            }
        }

        private void OnKillMonster(int guid,int id)
        {
            if(!this.mMonsterGUIDSet.Contains(guid))
            {
                return;
            }
            this.mMonsterGUIDSet.Remove(guid);
            this.CurrKillNum++;
            this.CurrMonsterNum = mMonsterGUIDSet.Count;
            if (CurrMonsterNum > 0)
            {
                return;
            }
            if (CurrIndex >= mWaveSet.Waves.Count)
            {
                Exit();
            }
            else
            {
                CreateWaveMonsters(mWaveSet.Waves[CurrIndex]);
            }
        }
    }
}

