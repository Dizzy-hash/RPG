using UnityEngine;
using System.Collections;
using Cfg.Level;
using LVL;
using System.Collections.Generic;
using System;

namespace LVL
{
    public class LevelManager : GTMonoSingleton<LevelManager>
    {
        [SerializeField] public int    MapID;
        [SerializeField] public string MapName = string.Empty;

        private MapConfig                            mConfig = new MapConfig();
        private Dictionary<EMapHolder, LevelElement> mHolders = new Dictionary<EMapHolder, LevelElement>();

        public void Init()
        {
            AddHolder<GroupBorn>(EMapHolder.Born);
            AddHolder<GroupMonsterSet>(EMapHolder.MonsterSet);
            AddHolder<GroupWaveSet>(EMapHolder.WaveSet);
            AddHolder<GroupBarrier>(EMapHolder.Barrier);
            AddHolder<GroupRegion>(EMapHolder.Region);
            AddHolder<GroupPortal>(EMapHolder.Portal);
            AddHolder<GroupNpc>(EMapHolder.Npc);
            AddHolder<GroupMineSet>(EMapHolder.MineSet);
            AddHolder<GroupObj>(EMapHolder.Obj);
            foreach (var current in mHolders)
            {
                current.Value.transform.ResetLocalTransform(transform);
            }
        }

        public void AddHolder<T>(EMapHolder type) where T : LevelElement, ILevelGroup
        {
            LevelElement holder = null;
            mHolders.TryGetValue(type, out holder);
            if (holder == null)
            {
                holder = new GameObject(typeof(T).Name).AddComponent<T>();
                mHolders[type] = holder;
            }
        }

        public void EnterWorld(int mapID)
        {
            this.MapID = mapID;
            this.Init();
            if (OnInitConfig())
            {
                this.OnAwakeSceneEventsStart();
                this.OnSceneStart();
            }
            else
            {
                GTLauncher.CurScene.InitWindows();
            }
        }

        public void LeaveWorld()
        {
            CharacterManager.Instance.DelCharacters();
        }

        void OnSceneStart()
        {
            StartCoroutine(DoSceneStartEvents());
        }

        void OnSceneEnd()
        {

        }

        bool OnInitConfig()
        {
            string fsPath = LevelUtil.GetConfigPath(this.MapID);
            this.mConfig = new MapConfig();
            return this.mConfig.Load(fsPath);
        }

        void OnAwakeSceneEventsStart()
        {

        }

        void AddMainPlayer()
        {
            int id = GTLauncher.CurPlayerID;
            if (mConfig.A == null)
                return;
            KTransform bornData = KTransform.Create(mConfig.A.Pos, mConfig.A.Euler);
            CharacterManager.Instance.AddMainPlayer(bornData);
        }

        void AddMainPet()
        {

        }

        void AddPartner()
        {

        }

        void AddMonster()
        {
            if (GTLauncher.CurSceneID == 3)
            {
                {
                    Vector3 pos1 = new Vector3(-17.3f, 1.44f, -16.37f);
                    Vector3 euler1 = Vector3.zero;
                    KTransform bornData1 = KTransform.Create(pos1, euler1);
                    CharacterManager.Instance.AddActorNoneSync(50001, EBattleCamp.B, EActorType.MONSTER, bornData1);
                }
                {
                    Vector3 pos2 = new Vector3(-11.1f, 1.44f, -16.37f);
                    Vector3 euler2 = Vector3.zero;
                    KTransform bornData2 = KTransform.Create(pos2, euler2);
                    CharacterManager.Instance.AddActorNoneSync(50002, EBattleCamp.B, EActorType.MONSTER, bornData2);
                }
                {
                    Vector3 pos3 = new Vector3(-21.7f, 1.44f, -16.37f);
                    Vector3 euler3 = Vector3.zero;
                    KTransform bornData3 = KTransform.Create(pos3, euler3);
                    CharacterManager.Instance.AddActorNoneSync(50003, EBattleCamp.B, EActorType.MONSTER, bornData3);
                }
                {
                    Vector3 pos1 = new Vector3(-17.3f, 1.44f, -12.37f);
                    Vector3 euler1 = Vector3.zero;
                    KTransform bornData1 = KTransform.Create(pos1, euler1);
                    CharacterManager.Instance.AddActorNoneSync(50004, EBattleCamp.B, EActorType.MONSTER, bornData1);
                }
                {
                    Vector3 pos2 = new Vector3(-11.1f, 1.44f, -12.37f);
                    Vector3 euler2 = Vector3.zero;
                    KTransform bornData2 = KTransform.Create(pos2, euler2);
                    CharacterManager.Instance.AddActorNoneSync(50005, EBattleCamp.B, EActorType.MONSTER, bornData2);
                }
                {
                    Vector3 pos3 = new Vector3(-21.7f, 1.44f, -12.37f);
                    Vector3 euler3 = Vector3.zero;
                    KTransform bornData3 = KTransform.Create(pos3, euler3);
                    CharacterManager.Instance.AddActorNoneSync(50006, EBattleCamp.B, EActorType.MONSTER, bornData3);
                }

                {
                    Vector3 pos1 = new Vector3(-17.3f, 1.44f, -8.37f);
                    Vector3 euler1 = Vector3.zero;
                    KTransform bornData1 = KTransform.Create(pos1, euler1);
                    CharacterManager.Instance.AddActorNoneSync(50004, EBattleCamp.B, EActorType.MONSTER, bornData1);
                }
                {
                    Vector3 pos2 = new Vector3(-11.1f, 1.44f, -8.37f);
                    Vector3 euler2 = Vector3.zero;
                    KTransform bornData2 = KTransform.Create(pos2, euler2);
                    CharacterManager.Instance.AddActorNoneSync(50005, EBattleCamp.B, EActorType.MONSTER, bornData2);
                }
                {
                    Vector3 pos3 = new Vector3(-21.7f, 1.44f, -8.37f);
                    Vector3 euler3 = Vector3.zero;
                    KTransform bornData3 = KTransform.Create(pos3, euler3);
                    CharacterManager.Instance.AddActorNoneSync(50006, EBattleCamp.B, EActorType.MONSTER, bornData3);
                }

                {
                    Vector3 pos1 = new Vector3(-17.3f, 1.44f, -4.37f);
                    Vector3 euler1 = Vector3.zero;
                    KTransform bornData1 = KTransform.Create(pos1, euler1);
                    CharacterManager.Instance.AddActorNoneSync(50004, EBattleCamp.B, EActorType.MONSTER, bornData1);
                }
                {
                    Vector3 pos2 = new Vector3(-11.1f, 1.44f, -4.37f);
                    Vector3 euler2 = Vector3.zero;
                    KTransform bornData2 = KTransform.Create(pos2, euler2);
                    CharacterManager.Instance.AddActorNoneSync(50005, EBattleCamp.B, EActorType.MONSTER, bornData2);
                }
                {
                    Vector3 pos3 = new Vector3(-21.7f, 1.44f, -4.37f);
                    Vector3 euler3 = Vector3.zero;
                    KTransform bornData3 = KTransform.Create(pos3, euler3);
                    CharacterManager.Instance.AddActorNoneSync(50006, EBattleCamp.B, EActorType.MONSTER, bornData3);
                }
            }
            else if(GTLauncher.CurSceneID==5)
            {
                //< MapBorn Camp = "0" Pos = "(-26.5,0.50,-63.02)"   Euler = "(0.00,0.00,0.00)" Scale = "(1.00,1.00,1.00)" />

                {
                    Vector3 pos1 = new Vector3(-17.3f, 1.44f, -60.02f);
                    Vector3 euler1 = Vector3.zero;
                    KTransform bornData1 = KTransform.Create(pos1, euler1);
                    CharacterManager.Instance.AddActorNoneSync(50001, EBattleCamp.B, EActorType.MONSTER, bornData1);
                }
                {
                    Vector3 pos2 = new Vector3(-11.1f, 1.44f, -56.37f);
                    Vector3 euler2 = Vector3.zero;
                    KTransform bornData2 = KTransform.Create(pos2, euler2);
                    CharacterManager.Instance.AddActorNoneSync(50002, EBattleCamp.B, EActorType.MONSTER, bornData2);
                }
                {
                    Vector3 pos3 = new Vector3(-21.7f, 1.44f, -58.37f);
                    Vector3 euler3 = Vector3.zero;
                    KTransform bornData3 = KTransform.Create(pos3, euler3);
                    CharacterManager.Instance.AddActorNoneSync(50003, EBattleCamp.B, EActorType.MONSTER, bornData3);
                }
                {
                    Vector3 pos1 = new Vector3(-17.3f, 1.44f, -56.37f);
                    Vector3 euler1 = Vector3.zero;
                    KTransform bornData1 = KTransform.Create(pos1, euler1);
                    CharacterManager.Instance.AddActorNoneSync(50004, EBattleCamp.B, EActorType.MONSTER, bornData1);
                }
                {
                    Vector3 pos2 = new Vector3(-11.1f, 1.44f, -55.37f);
                    Vector3 euler2 = Vector3.zero;
                    KTransform bornData2 = KTransform.Create(pos2, euler2);
                    CharacterManager.Instance.AddActorNoneSync(50005, EBattleCamp.B, EActorType.MONSTER, bornData2);
                }
                {
                    Vector3 pos3 = new Vector3(-21.7f, 1.44f, -54.37f);
                    Vector3 euler3 = Vector3.zero;
                    KTransform bornData3 = KTransform.Create(pos3, euler3);
                    CharacterManager.Instance.AddActorNoneSync(50006, EBattleCamp.B, EActorType.MONSTER, bornData3);
                }

                {
                    Vector3 pos1 = new Vector3(-17.3f, 1.44f, -53.37f);
                    Vector3 euler1 = Vector3.zero;
                    KTransform bornData1 = KTransform.Create(pos1, euler1);
                    CharacterManager.Instance.AddActorNoneSync(50004, EBattleCamp.B, EActorType.MONSTER, bornData1);
                }
                {
                    Vector3 pos2 = new Vector3(-11.1f, 1.44f, -52.37f);
                    Vector3 euler2 = Vector3.zero;
                    KTransform bornData2 = KTransform.Create(pos2, euler2);
                    CharacterManager.Instance.AddActorNoneSync(50005, EBattleCamp.B, EActorType.MONSTER, bornData2);
                }
                {
                    Vector3 pos3 = new Vector3(-21.7f, 1.44f, -51.37f);
                    Vector3 euler3 = Vector3.zero;
                    KTransform bornData3 = KTransform.Create(pos3, euler3);
                    CharacterManager.Instance.AddActorNoneSync(50006, EBattleCamp.B, EActorType.MONSTER, bornData3);
                }

                {
                    Vector3 pos1 = new Vector3(-17.3f, 1.44f, -50.37f);
                    Vector3 euler1 = Vector3.zero;
                    KTransform bornData1 = KTransform.Create(pos1, euler1);
                    CharacterManager.Instance.AddActorNoneSync(50004, EBattleCamp.B, EActorType.MONSTER, bornData1);
                }
                {
                    Vector3 pos2 = new Vector3(-11.1f, 1.44f, -49.37f);
                    Vector3 euler2 = Vector3.zero;
                    KTransform bornData2 = KTransform.Create(pos2, euler2);
                    CharacterManager.Instance.AddActorNoneSync(50005, EBattleCamp.B, EActorType.MONSTER, bornData2);
                }
                {
                    Vector3 pos3 = new Vector3(-21.7f, 1.44f, -48.37f);
                    Vector3 euler3 = Vector3.zero;
                    KTransform bornData3 = KTransform.Create(pos3, euler3);
                    CharacterManager.Instance.AddActorNoneSync(50006, EBattleCamp.B, EActorType.MONSTER, bornData3);
                }
            }
        }

        void SetFollowCamera()
        {
            Camera cam = GTCameraManager.Instance.MainCamera;
            object[] args = new object[] { CharacterManager.Main.CacheTransform };
            GTCameraManager.Instance.FollowPlayer(CharacterManager.Main.CacheTransform);
        }

        IEnumerator DoSceneStartEvents()
        {
            yield return null;
            AddMainPlayer();
            yield return null;
            AddMainPet();
            yield return null;
            AddPartner();
            yield return null;
            AddMonster();
            yield return null;
            SetFollowCamera();
            yield return null;
            GTLauncher.CurScene.InitWindows();
        }
    }
}