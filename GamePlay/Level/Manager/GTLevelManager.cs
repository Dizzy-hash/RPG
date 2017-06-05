using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LevelDirector;
using System;
using Cfg.Map;
using System.Linq;


public class GTLevelManager : GTMonoSingleton<GTLevelManager>
{
    [SerializeField] public int                MapID;
    [SerializeField] public float              Delay;
    [SerializeField] public string             MapName = string.Empty;

    private MapConfig                            mConfig  = new MapConfig();
    private Dictionary<EMapHolder, LevelElement> mHolders = new Dictionary<EMapHolder, LevelElement>();

    private static int GUIDStart = 100001;
    public int    GetGUID()
    {
        GUIDStart++;
        return GUIDStart;
    }

    public string GetConfigPath(int mapID)
    {
        return string.Format("Text/Map/{0}", mapID);
    }

    public void Init()
    {
        AddHolder<HolderBorn>(EMapHolder.Born);
        AddHolder<HolderMonsterGroup>(EMapHolder.MonsterGroup);
        AddHolder<HolderWaveSet>(EMapHolder.WaveSet);
        AddHolder<HolderBarrier>(EMapHolder.Barrier);
        AddHolder<HolderRegion>(EMapHolder.Region);
        AddHolder<HolderPortal>(EMapHolder.Portal);
        AddHolder<HolderNpc>(EMapHolder.Npc);
        AddHolder<HolderMineGroup>(EMapHolder.MineGroup);
        AddHolder<HolderRole>(EMapHolder.Role);
        AddHolder<HolderObj>(EMapHolder.Obj);
        foreach (var current in mHolders)
        {
            current.Value.transform.ResetLocalTransform(transform);
        }
    }

    public void EnterWorld(int mapID)
    {
        this.MapID = mapID;
        this.Init();
        if (InitConfig())
        {
            this.InitAwakeRegions();
            this.OnSceneStart();
        }
        else
        {
            GTLauncher.Instance.CurScene.OnOpenWindows();
        }
    }

    bool InitConfig()
    {
        string fsPath = GetConfigPath(this.MapID);
        this.mConfig = new MapConfig();
        return this.mConfig.Load(fsPath);
    }

    void InitAwakeRegions()
    {
        for (int i = 0; i < mConfig.Regions.Count; i++)
        {
            MapRegion data = mConfig.Regions[i];
            if (data.AwakeActive)
            {
                LevelElement pHolder = GetHolder(EMapHolder.Region);
                GameObject go = NGUITools.AddChild(pHolder.gameObject);
                LevelRegion pRegion = go.AddComponent<LevelRegion>();
                pRegion.Import(data, false);
                pRegion.Init();
            }
        }
    }

    void OnSceneStart()
    {
        AddSceneListeners();
        StartCoroutine(DoSceneStartEvents());
    }

    void OnSceneEnd()
    {
        DelSceneListeners();
    }

    void AddSceneListeners()
    {
        GTEventCenter.AddHandler(GTEventID.RECV_MAINPLAYER_DEAD, OnMainPlayerDead);
    }

    void DelSceneListeners()
    {
        GTEventCenter.DelHandler(GTEventID.RECV_MAINPLAYER_DEAD, OnMainPlayerDead);
    }

    IEnumerator DoSceneStartEvents()
    {
        AddMainPlayer();
        yield return null;
        AddMainPartner();
        yield return null;
        AddMainPet();
        yield return null;
        AddNpc();
        yield return null;
        SetFollowCamera();
        yield return null;
        GTLauncher.Instance.CurScene.OnOpenWindows();
        yield return null;
    }

    bool AddMainPlayer()
    {
        int id = GTLauncher.Instance.TestScene ? GTLauncher.Instance.TestActorID : GTDataManager.Instance.CurRoleID;
        if (mConfig.A == null)
        {
            return false;
        }
        XTransform param = XTransform.Create(mConfig.A.Pos, mConfig.A.Euler);
        ActorMainPlayer pActor = (ActorMainPlayer)AddActor(id, EActorType.PLAYER, EBattleCamp.A, param, true);
        LevelData.MainPlayer = pActor;
        return true;
    }

    bool AddMainPartner()
    {
        for (int i = 0; i < 3; i++)
        {
            AddPartner(LevelData.MainPlayer, i + 1, LevelData.MainPlayer.GetActorCard().Partners[i]);
        }
        return true;
    }

    bool AddMainPet()
    {
        return false;
    }

    bool AddNpc()
    {
        for (int i = 0; i < mConfig.Npcs.Count; i++)
        {
            MapNpc data = mConfig.Npcs[i];
            AddActor(data.Id, EActorType.NPC, EBattleCamp.C, data.Pos, data.Euler, data.Scale);
        }
        return true;
    }

    void SetFollowCamera()
    {
        Camera cam = GTCameraManager.Instance.MainCamera;
        object[] args = new object[] { LevelData.MainPlayer.CacheTransform };
        GTCameraManager.Instance.SwitchCameraEffect(ECameraType.FOLLOW, cam, null, args);
    }

    public LevelElement GetHolder(EMapHolder type)
    {
        LevelElement holder = null;
        mHolders.TryGetValue(type, out holder);
        return holder;
    }

    public void  AddHolder<T>(EMapHolder type) where T : LevelElement
    {
        LevelElement holder = null;
        mHolders.TryGetValue(type, out holder);
        if (holder == null)
        {
            holder = new GameObject(typeof(T).Name).AddComponent<T>();
            mHolders[type] = holder;
        }
    }

    public Actor AddMainPlayer(int id, XTransform param)
    {
        ActorMainPlayer pActor = (ActorMainPlayer)AddActor(id, EActorType.PLAYER, EBattleCamp.A, param, true);
        LevelData.MainPlayer = pActor;
        return pActor;
    }

    public Actor AddActor(int id, EActorType type, EBattleCamp camp, XTransform param,bool isMainPlayer=false)
    {
        int guid = GetGUID();
        Actor pActor = null;
        switch (type)
        {
            case EActorType.PLAYER:
                {
                    if(isMainPlayer)
                    {
                        pActor = new ActorMainPlayer(id, guid, EActorType.PLAYER, camp);
                    }
                    else
                    {
                        pActor = new ActorPlayer(id, guid, EActorType.PLAYER, camp);
                    }
                }
                break;
            case EActorType.MONSTER:
            case EActorType.MACHINE:
            case EActorType.PARTNER:
            case EActorType.PET:
            case EActorType.NPC:
            case EActorType.MOUNT:
                pActor = new Actor(id, guid, type, camp);
                break;

        }
        if (pActor != null)
        {
            param.Position = GTTools.NavSamplePosition(param.Position);
            pActor.Load(param);
            if (pActor.CacheTransform != null)
            {
                pActor.CacheTransform.parent = GetHolder(EMapHolder.Role).transform;
                LevelData.AllActors.Add(pActor);
                LevelData.CampActors[camp].Add(pActor);
            }
        }
        return pActor;
    }

    public Actor AddActor(int id, EActorType type, EBattleCamp camp, Vector3 pos, Vector3 angle)
    {
        return AddActor(id, type, camp, XTransform.Create(pos, angle));
    }

    public Actor AddActor(int id, EActorType type, EBattleCamp camp, Vector3 pos, Vector3 angle, Vector3 scale)
    {
        return AddActor(id, type, camp, XTransform.Create(pos, angle, scale));
    }

    public Actor AddPartner(Actor host, int pos, int id)
    {
        if (host == null || host.Obj == null)
            return null;
        if (id <= 0)
            return null;
        EPartnerSort sort = (EPartnerSort)pos;
        Vector3 pPos = host.GetPartnerPosBySort(sort);
        XTransform param = XTransform.Create(pPos, host.Euler, Vector3.one * 1.5f);
        Actor partner = AddActor(id, EActorType.PARTNER, host.Camp, param);
        if (partner == null)
        {
            return null;
        }
        switch (sort)
        {
            case EPartnerSort.LF:
                host.Partner1 = partner;
                break;
            case EPartnerSort.RT:
                host.Partner2 = partner;
                break;
            case EPartnerSort.MD:
                host.Partner3 = partner;
                break;
        }
        partner.Sort = sort;
        return partner;
    }

    public bool DelActor(Actor pActor)
    {
        if (pActor != null)
        {
            LevelData.AllActors.Remove(pActor);
            LevelData.CampActors[pActor.Camp].Remove(pActor);
            CfgActor db = GTConfigManager.Instance.RdCfgActor.GetCfgById(pActor.Id);
            pActor.Release();
            GTPoolManager.Instance.ReleaseGo(db.Model, pActor.Obj);
            return true;
        }
        return false;
    }

    public void StartMapEvent(MapEvent pData, LevelRegion pRegion)
    {
        switch (pData.Type)
        {
            case EMapTrigger.TYPE_BARRIER:
                {
                    HolderBarrier pHolder = GetHolder(EMapHolder.Barrier) as HolderBarrier;
                    CreateMapEvent(pData, pRegion, pHolder, mConfig.Barriers);
                }
                break;
            case EMapTrigger.TYPE_REGION:
                {
                    HolderRegion pHolder = GetHolder(EMapHolder.Region) as HolderRegion;
                    CreateMapEvent(pData, pRegion, pHolder, mConfig.Regions);
                }
                break;
            case EMapTrigger.TYPE_PORTAL:
                {
                    HolderPortal pHolder = GetHolder(EMapHolder.Portal) as HolderPortal;
                    CreateMapEvent(pData, pRegion, pHolder, mConfig.Portals);
                }
                break;
            case EMapTrigger.TYPE_WAVESET:
                {
                    HolderWaveSet pHolder = GetHolder(EMapHolder.WaveSet) as HolderWaveSet;
                    CreateMapEvent(pData, pRegion, pHolder, mConfig.WaveSets);
                }
                break;
            case EMapTrigger.TYPE_RESULT:
                {
                    OnSceneEnd();
                }
                break;
            case EMapTrigger.TYPE_MONSTEGROUP:
                {
                    HolderMonsterGroup pHolder = GetHolder(EMapHolder.MonsterGroup) as HolderMonsterGroup;
                    CreateMapEvent(pData, pRegion, pHolder, mConfig.MonsterGroups);
                }
                break;
            case EMapTrigger.TYPE_MINEGROUP:
                {
                    HolderMineGroup pHolder = GetHolder(EMapHolder.MineGroup) as HolderMineGroup;
                    CreateMapEvent(pData, pRegion, pHolder, mConfig.MineGroups);
                }
                break;

        }
    }

    public void CreateMapEvent<T, S>(MapEvent pData, LevelRegion pRegion, LevelContainerBase<T> pHolder, List<S> pElemDataList) where T : LevelElement where S : MapElement
    {
        T tElement = null;
        if (pData.Active)
        {
            for (int i = 0; i < pElemDataList.Count; i++)
            {
                if (pElemDataList[i].Id == pData.Id)
                {
                    tElement = pHolder.AddElement();
                    tElement.Import(pElemDataList[i], false);
                    if (tElement is LevelWaveSet)
                    {
                        LevelWaveSet waveset = tElement as LevelWaveSet;
                        waveset.Region = pRegion;
                    }
                    tElement.Init();
                }
            }
        }
        else
        {
            tElement = pHolder.FindElement(pData.Id);
            if (tElement != null)
            {
                DestroyImmediate(tElement.gameObject);
            }
        }
    }

    public bool FindDestMap(int id, ref Vector3 pPortalPos)
    {
        return false;
    }

    public void Clear()
    {
        for (int i = LevelData.AllActors.Count - 1; i >= 0; i--)
        {
            Actor pActor = LevelData.AllActors[i];
            LevelData.AllActors.RemoveAt(i);
            Destroy(pActor.Obj);
        }
        LevelData.MainPlayer = null;
        foreach (KeyValuePair<EMapHolder, LevelElement> current in mHolders)
        {
            current.Value.transform.DestroyChildren();
        }
        foreach (var current in LevelData.CampActors)
        {
            current.Value.Clear();
        }
    }

    void OnMainPlayerDead()
    {
        List<Actor> pList = LevelData.GetActorsByActorType(EActorType.MONSTER);
        for (int i = 0; i < pList.Count; i++)
        {
            pList[i].SetTarget(null);
        }
        GTLauncher.Instance.CurScene.OnMainPlayerDead();
    }
}
