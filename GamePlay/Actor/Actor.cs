using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class Actor : ICharacter
{

    public GameObject                          Obj            { get; set; }
    public Transform                           ObjTrans       { get; set; }
    public Transform                           CacheTransform { get; set; }
    public int                                 GUID           { get; set; }
    public int                                 Id             { get; set; }
    public EActorType                          Type           { get; set; }
    public EBattleCamp                         Camp           { get; set; }
    public CharacterCommand                    Command        { get; set; }
    public CharacterAttr                       BaseAttr       { get; set; }
    public CharacterAttr                       CurrAttr       { get; set; }
    public CharacterView                       View           { get; set; }
    public CharacterSkill                      Skill          { get; set; }
    public CharacterBuff                       Buff           { get; set; }
    public CharacterPathFinding                PathFinding    { get; set; }
    public CharacterAvatar                     Avatar         { get; set; }
    public CharacterAI                         AI             { get; set; }
    public Character                           Host           { get; set; }
    public Character                           Target         { get; set; }

    public Animator                            Anim           { get; set; }
    public CharacterController                 CC             { get; set; }
    public IStateMachine<Character, FSMState>  Machine        { get; set; }
    public XTransform                          BornData       { get; set; }
    public GTAction                            Action         { get; set; }

    protected CharacterAttr mBaseAttr = new CharacterAttr();
    protected CharacterAttr mCurrAttr = new CharacterAttr();

    protected XTransform mBornParam;
    protected CharacterController mCharacter;
    protected ActorBehavior mBehavior;

    protected IStateMachine<Actor,FSMState> mMachine;
    protected GTAction      mActorAction;
    protected Animator      mActorAnim;
    protected Transform[]   mHands;

    protected Actor mTarget;       //当前目标
    protected Actor mHost;         //主人
    protected Actor mVehicle;      //交通工具
    protected Actor mPartner1;
    protected Actor mPartner2;
    protected Actor mPartner3;
    protected Actor mMount;
    protected Actor mPet;

    protected ActorPathFinding mActorPathFinding;
    protected ActorBuff  mActorBuff;
    protected ActorSkill mActorSkill;
    protected ActorCard  mActorCard;
    protected ActorAI    mActorAI;

    protected UInt32 mAIMark = 0;

    protected List<Actor> mEnemys = new List<Actor>();
    protected List<Actor> mAllys = new List<Actor>();
    protected List<Actor> mTargets = new List<Actor>();

    protected Dictionary<int, EquipAvatar>      mEquipAvatars = new Dictionary<int, EquipAvatar>();
    protected Dictionary<EActorNature, bool>    mBaseFeatures = new Dictionary<EActorNature, bool>();
    protected Dictionary<EActorEffect, bool>    mActorEffects = new Dictionary<EActorEffect, bool>();

    public EActorSort   MonsterType { get; private set; }
    public EPartnerSort   Sort { get; set; }

    public class EquipAvatar
    {
        public GameObject[] Models = new GameObject[2];
    }

    public Actor Partner1
    {
        get { return mPartner1; }
        set { mPartner1 = value; if (mPartner1 != null) { mPartner1.SetHost(this); } }
    }

    public Actor Partner2
    {
        get { return mPartner2; }
        set { mPartner2 = value; if (mPartner2 != null) { mPartner2.SetHost(this); } }
    }

    public Actor Partner3
    {
        get { return mPartner3; }
        set { mPartner3 = value; if (mPartner3 != null) { mPartner3.SetHost(this); } }
    }

    public Actor Mount
    {
        get { return mMount; }
        set { mMount = value; if (mMount != null) { mMount.SetHost(this); } }
    }

    public Actor Pet
    {
        get { return mPet; }
        set { mPet = value; if (mPet != null) { mPet.SetHost(this); } }
    }

    public ActorBehavior Behavior
    {
        get { return mBehavior; }
    }

    public void ApplyAnimator(bool enabled)
    {
        if (mActorAnim != null)
        {
            mActorAnim.enabled = enabled;
        }
    }

    public void ApplyCharacterCtrl(bool enabled)
    {
        if (mCharacter != null)
        {
            mCharacter.enabled = enabled;
        }
    }

    public void ApplyRootMotion(bool enabled)
    {
        if(mActorAnim!=null)
        {
            mActorAnim.applyRootMotion = enabled;
        }
    }

    public void SetActorEffect(EActorEffect type,bool flag)
    {
        mActorEffects[type] = flag;
    }

    public bool GetActorEffect(EActorEffect type)
    {
        bool flag;
        mActorEffects.TryGetValue(type, out flag);
        return flag;
    }

    public bool GetAIFeature(EActorNature type)
    {
        bool flag;
        mBaseFeatures.TryGetValue(type, out flag);
        return flag;
    }

    public FSMState FSM
    {
        get
        {
            if (this.mMachine == null)
            {
                return FSMState.FSM_BORN;
            }
            return (FSMState)this.mMachine.GetCurStateID();
        }
    }

    public Vector3 Dir
    {
        get { return CacheTransform.forward; }
        set { CacheTransform.forward = value; }
    }

    public Vector3 Euler
    {
        get { return CacheTransform.localEulerAngles; }
        set { CacheTransform.localEulerAngles = value; }
    }

    public Vector3 Pos
    {
        get { return CacheTransform.localPosition; }
        set { CacheTransform.localPosition = value; }
    }

    public Vector3 GetPartnerPosBySort(EPartnerSort sort)
    {
        switch(sort)
        {
            case EPartnerSort.LF:
                return Pos + new Vector3(-2, 0, 0);
            case EPartnerSort.RT:
                return Pos + new Vector3( 2, 0, 0);
            case EPartnerSort.MD:
                return Pos - Dir * 3;
        }
        return Pos;
    }

    public float Height
    {
        get { return mCharacter.height * CacheTransform.localScale.x; }
    }

    public float Radius
    {
        get { return mCharacter.radius * CacheTransform.localScale.x; }
    }


    public Vector3 Scale
    {
        get;set;
    }

    public Transform[] GetHands()
    {
        if(mHands==null&&CacheTransform!=null)
        {
            mHands = new Transform[2];
            mHands[0] = GTTools.GetBone(CacheTransform, "Bip01 L Hand");
            mHands[1] = GTTools.GetBone(CacheTransform, "Bip01 R Hand");
        }
        return mHands;
    }

    public Actor GetTarget()
    {
        return mTarget;
    }

    public Actor GetHost()
    {
        return mHost;
    }

    public Actor GetVehicle()
    {
        return mVehicle;
    }

    public Actor(int id, int guid, EActorType type, EBattleCamp camp)
    {
        this.Id   = id;
        this.GUID = guid;
        this.Type = type;
        this.Camp = camp;
        this.MonsterType = GTConfigManager.Instance.RdCfgActor.GetCfgById(Id).Sort;
    }

    public void InitAttr(bool init=false)
    {
        Dictionary<EProperty, int> propertys = null;
        CfgActor db = GTConfigManager.Instance.RdCfgActor.GetCfgById(Id);
        if (Type == EActorType.PLAYER)
        {
            XRole role = GTDataManager.Instance.GetCurRole();
            if (role != null)
            {
                propertys = GTAttr.GetPropertys(role);
            }
        }
        if (propertys == null)
        {
            propertys = db.Propertys;
        }
        mBaseAttr.CopyFrom(propertys);
        mBaseAttr.Update(EAttr.RSpeed, (int)db.RSpeed);
        UpdateCurrAttr(init);
    }

    public void InitAction()
    {
        this.mActorAnim   = Obj.GetComponent<Animator>();
        this.mActorAnim.applyRootMotion = true;
        this.mActorAnim.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        this.mActorAction = new GTAction(mActorAnim);
    }

    public void InitFeature()
    {
        for (int i = 0; i < Enum.GetNames(typeof(EActorNature)).Length; i++)
        {
            EActorNature type = (EActorNature)i;
            bool flag = GTTools.GetValueFromBitMark(mAIMark, i);
            this.mBaseFeatures[type] = flag;
        }
    }

    public void InitLayer()
    {
        switch (Type)
        {
            case EActorType.PLAYER:
                NGUITools.SetLayer(Obj, (this is ActorMainPlayer) ? GTLayerDefine.LAYER_AVATAR : GTLayerDefine.LAYER_PLAYER);
                break;
            case EActorType.MONSTER:
                NGUITools.SetLayer(Obj, GTLayerDefine.LAYER_MONSTER);
                break;
            case EActorType.NPC:
                NGUITools.SetLayer(Obj, GTLayerDefine.LAYER_NPC);
                break;
            case EActorType.PET:
                NGUITools.SetLayer(Obj, GTLayerDefine.LAYER_PET);
                break;
            case EActorType.MOUNT:
                NGUITools.SetLayer(Obj, GTLayerDefine.LAYER_MOUNT);
                break;
            case EActorType.PARTNER:
                NGUITools.SetLayer(Obj, GTLayerDefine.LAYER_PARTNER);
                break;
        }
    }

    public GameObject LoadObject(XTransform data)
    {
        CfgActor db = GTConfigManager.Instance.RdCfgActor.GetCfgById(Id);
        if (db != null)
        {
            GameObject go= GTPoolManager.Instance.GetObject(db.Model);
            go.transform.localPosition = data.Position;
            go.transform.localEulerAngles = data.EulerAngles;
            if(data.Scale!=Vector3.zero)
            {
                go.transform.localScale = data.Scale;
            }
            return go;
        }
        return null;
    }

    public virtual void Load(XTransform data)
    {
        this.Obj = LoadObject(data);
        if (this.Obj == null)
        {
            return;
        }
        this.CacheTransform = Obj.transform;
        this.mBornParam = data;
        this.mCharacter = Obj.GetComponent<CharacterController>();
        this.mVehicle = this;
        this.Init();
    }

    public virtual void Init()
    {
        mActorPathFinding = new ActorPathFinding(this);
        mActorCard = new ActorCard(this);
        mActorBuff = new ActorBuff(this);
        mActorSkill = new ActorSkill(this);
        this.InitFeature();
        this.InitAttr(true);
        this.InitLayer();
        this.InitAction();
        this.InitState();
        this.InitBoard();
        this.InitFSM();
        this.InitBehavior();
        this.InitAI();
        this.InitCommands();
        this.InitAvatar();
    }

    protected void InitBehavior()
    {
        this.mBehavior = Obj.GET<ActorBehavior>();
        this.mBehavior.SetOwner(this);
    }

    protected void InitAvatar()
    {
        if(this.Type!=EActorType.PLAYER)
        {
            return;
        }
        for (int i = 0; i < 8; i++)
        {
            ChangeEquipAvatar(0, i + 1);
        }
    }

    protected void InitState()
    {
        this.mBaseFeatures[EActorNature.CAN_MOVE] = true;
        this.mBaseFeatures[EActorNature.CAN_TURN] = true;

        this.mActorEffects[EActorEffect.IS_RIDE] = false;
        this.mActorEffects[EActorEffect.IS_SILENT] = false;
        this.mActorEffects[EActorEffect.IS_DIVIVE] = false;
        this.mActorEffects[EActorEffect.IS_AUTOMOVE] = false;
        this.mActorEffects[EActorEffect.IS_STEALTH] = false;

        this.ApplyAnimator(true);
    }

    protected void InitAI()
    {
        mActorAI = new ActorAI(this);     
        mActorAI.Startup();
    }

    protected void InitFSM()
    {
        this.mMachine = new IStateMachine<Actor,FSMState>(this);
        this.mMachine.AddState(FSMState.FSM_EMPTY, new ActorEmptyFSM());
        this.mMachine.AddState(FSMState.FSM_IDLE, new ActorIdleFSM());
        this.mMachine.AddState(FSMState.FSM_RUN, new ActorRunFSM());
        this.mMachine.AddState(FSMState.FSM_WALK, new ActorWalkFSM());
        this.mMachine.AddState(FSMState.FSM_TURN, new ActorTurnFSM());

        this.mMachine.AddState(FSMState.FSM_SKILL, new ActorSkillFSM());
        this.mMachine.AddState(FSMState.FSM_DEAD, new ActorDeadFSM());

        this.mMachine.AddState(FSMState.FSM_BEATFLY, new ActorBeatFlyFSM());
        this.mMachine.AddState(FSMState.FSM_BEATBACK, new ActorBeatBackFSM());
        this.mMachine.AddState(FSMState.FSM_BEATDOWN, new ActorBeatDownFSM());

        this.mMachine.AddState(FSMState.FSM_WOUND, new ActorWoundFSM());
        this.mMachine.AddState(FSMState.FSM_STUN, new ActorStunFSM());
        this.mMachine.AddState(FSMState.FSM_FROST, new ActorFrostFSM());
        this.mMachine.AddState(FSMState.FSM_FIXBODY, new ActorFixBodyFSM());
        this.mMachine.AddState(FSMState.FSM_FEAR, new ActorFearFSM());
        this.mMachine.AddState(FSMState.FSM_VARIATION, new ActorVariationFSM());
        this.mMachine.AddState(FSMState.FSM_JUMP, new ActorJumpFSM());
        this.mMachine.AddState(FSMState.FSM_REBORN, new ActorRebornFSM());
        this.mMachine.AddState(FSMState.FSM_MINE, new ActorMineFSM());
        this.mMachine.AddState(FSMState.FSM_INTERACTIVE, new ActorInterActiveFSM());
        this.mMachine.SetCurState(this.mMachine.GetState(FSMState.FSM_IDLE));
        this.mMachine.GetState(this.mMachine.GetCurStateID()).Enter();
    }

    public virtual void TranslateTo(Vector3 postion, bool idle)
    {
        if (CacheTransform == null)
        {
            return;
        }
        CacheTransform.position = postion;
        if (idle)
        {
            GotoEmptyFSM();
        }
    }

    public virtual void MoveTo(Vector3 destPosition)
    {
        this.SetActorEffect(EActorEffect.IS_AUTOMOVE, true);
        mActorPathFinding.SetDestPosition(destPosition);
    }

    public virtual void StopPathFinding()
    {
        this.SetActorEffect(EActorEffect.IS_AUTOMOVE, false);
        mActorPathFinding.StopPathFinding();
    }

    public void SetTarget(Actor actor)
    {
        if (actor == null)
        {
            this.mTarget = null;
            return;
        }
        if (mTarget == actor)
        {
            return;
        }
        this.mTarget = actor;
        CacheTransform.LookAt(this.mTarget.CacheTransform);
    }

    public void SetHost(Actor actor)
    {
        mHost = actor;
    }

    public Vector3 GetBind(EActorBindPos bind,Vector3 offset)
    {
        switch(bind)
        {
            case EActorBindPos.Body:
                return Pos + new Vector3(0, Height * 0.5f, 0) + offset;
            case EActorBindPos.Head:
                return Pos + new Vector3(0, Height, 0)+offset;
            case EActorBindPos.Foot:
                return Pos + offset;
            default:
                return Pos;
        }
    }

    public XTransform GetBornParam()
    {
        return mBornParam;
    }

    public bool IsEnemy(Actor actor)
    {
        if (actor == null)
        {
            return false;
        }
        return GetTargetCamp(actor) == ETargetCamp.TYPE_ENEMY;
    }

    public bool IsAlly(Actor actor)
    {
        if (actor == null)
        {
            return false;
        }
        return GetTargetCamp(actor) == ETargetCamp.TYPE_ALLY;
    }

    public bool IsMain()
    {
        return (this is ActorMainPlayer);
    }
      

    public List<Actor> GetAllEnemy()
    {
        mEnemys.Clear();
        FindActorsByTargetCamp(ETargetCamp.TYPE_ENEMY,ref mEnemys,true);      
        return mEnemys;
    }

    public List<Actor> GetAllAlly()
    {
        mAllys.Clear();
        FindActorsByTargetCamp(ETargetCamp.TYPE_ALLY, ref mAllys);
        return mAllys;
    }

    public List<Actor> GetActorsByAffectType(EAffect type)
    {
        switch(type)
        {
            case EAffect.Ally:
                return GetAllAlly();
            case EAffect.Host:
                if(mHost==null)
                {
                    return null;
                }
                else
                {
                    return new List<Actor>() { mHost };
                }
            case EAffect.Enem:
                return GetAllEnemy();
            case EAffect.Boss:
                List<Actor> list = new List<Actor>();
                List<Actor> enemys = GetAllEnemy();
                for(int i=0;i<enemys.Count;i++)
                {
                    Actor monster = enemys[i];
                    if (monster.Type==EActorType.MONSTER)
                    {
                        if(monster.IsBoss())
                        {
                            list.Add(monster);
                        }
                    }
                }
                return list;
            case EAffect.Self:
                return new List<Actor>() { this };
            case EAffect.Each:
                return LevelData.AllActors;
            default:
                return new List<Actor>();
        }
    }

    public Actor GetNearestEnemy(float radius = 10000)
    {
        List<Actor> actors = GetAllEnemy();
        Actor nearest = null;
        float min = radius;
        for (int i = 0; i < actors.Count; i++)
        {
            float dist = GTTools.GetHorizontalDistance(actors[i].CacheTransform.position, this.CacheTransform.position);
            if (dist < min)
            {
                min=dist;
                nearest = actors[i];
            }
        }
        return nearest;
    }

    public ETargetCamp GetTargetCamp(Actor actor)
    {
        if (actor.Camp == EBattleCamp.D)
        {
            return ETargetCamp.TYPE_NONE;
        }
        if (actor.Camp == Camp)
        {
            return ETargetCamp.TYPE_ALLY;
        }
        if (actor.Camp != EBattleCamp.C && Camp != EBattleCamp.C)
        {
            return ETargetCamp.TYPE_ENEMY;
        }
        return ETargetCamp.TYPE_NEUTRAL;
    }

    public void FindActorsByTargetCamp(ETargetCamp actorCamp,ref List<Actor> list,bool ignoreStealth=false)
    {
        for (int i = 0; i < LevelData.AllActors.Count; i++)
        {
            Actor actor = LevelData.AllActors[i];
            if (GetTargetCamp(actor) == actorCamp && actor.IsDead() == false)
            {
                if (ignoreStealth == false)
                {
                    list.Add(actor);
                }
                else
                {
                    if (actor.GetActorEffect(EActorEffect.IS_STEALTH) == false)
                    {
                        list.Add(actor);
                    }
                }
            }
        }
    }

    protected void ShowFlyword(EFlyWordType wordType, int value)
    {
        if (IsDead())
        {
            return;
        }
        GTFlywordManager.Instance.Show(value.ToString(), CacheTransform.position, wordType);
    }

    protected void RemoveBoard()
    {
        GTBoardManager.Instance.Release(this);
    }

    protected void RemoveEffect()
    {

    }

    protected void InitBoard()
    {
        if (this is ActorPlayer && GTLauncher.Instance.CurrSceneType != ESceneType.TYPE_CITY)
        {
            return;
        }
        switch (Type)
        {
            case EActorType.PLAYER:
                GTBoardManager.Instance.Create(this, EBoardType.TYPE_PLAYER);
                break;
            case EActorType.NPC:
                GTBoardManager.Instance.Create(this, EBoardType.TYPE_NPC);
                break;
            case EActorType.MONSTER:
                GTBoardManager.Instance.Create(this, EBoardType.TYPE_MONSTER);
                break;
        }
    }

    public void ChangeState(FSMState fsm, ICommand ev)
    {
        if (mMachine == null|| CacheTransform == null)
        {
            return;
        }
        if (FSM == FSMState.FSM_DEAD && fsm != FSMState.FSM_REBORN)
        {
            return;
        }
        if (!mMachine.Contains(fsm))
        {
            return;
        }
        mMachine.GetState(fsm).SetCommand(ev);
        mMachine.ChangeState(fsm);
    }

    public void SendStateMessage(FSMState fsm)
    {
        ChangeState(fsm, null);
    }

    public void SendStateMessage(FSMState fsm, ICommand ev)
    {
        ChangeState(fsm, ev);
    }

    public void GotoEmptyFSM()
    {
        ChangeState(FSMState.FSM_EMPTY, null);
    }

    public virtual void Execute()
    {
        if(CacheTransform==null|| mMachine == null)
        {
            return;
        }

        mMachine.Execute();
        mActorBuff.Execute();
        mActorPathFinding.Execute();
        mActorAI.Execute();
        mActorSkill.Execute();
    }

    public void UpdateAttr(EAttr attr, int value)
    {
        mCurrAttr.Update(attr, value);
    }

    public virtual void UpdateCurrAttr(bool init=false)
    {
        CharacterAttr bfAttr = new CharacterAttr();
        CharacterAttr bpAttr = new CharacterAttr();
        foreach(var current in mActorBuff.GetAllBuff())
        {
            int buffID = current.Key;
            CfgBuff db = GTConfigManager.Instance.RdCfgBuff.GetCfgById(buffID);
            if (db.ResultAttr > 0)
            {
                CfgBuffAttr attrDB = GTConfigManager.Instance.RdCfgBuffAttr.GetCfgById(db.ResultAttr);
                for (int i = 0; i < attrDB.Attrs.Count; i++)
                {
                    CfgValueType data = attrDB.Attrs[i];
                    switch (data.ValueType)
                    {
                        case EValueType.FIX:
                            bfAttr.Update(data.Attr, data.Value);
                            break;
                        case EValueType.PER:
                            bpAttr.Update(data.Attr, data.Value);
                            break;
                    }
                }
            }
        }

        mCurrAttr.MaxHP = (int)((mBaseAttr.MaxHP + bfAttr.MaxHP) * (1 + bpAttr.MaxHP / 10000f));
        mCurrAttr.MaxMP = (int)((mBaseAttr.MaxMP + bfAttr.MaxMP) * (1 + bpAttr.MaxMP / 10000f));
        mCurrAttr.Atk = (int)((mBaseAttr.Atk + bfAttr.Atk) * (1 + bpAttr.Atk / 10000f));
        mCurrAttr.Def = (int)((mBaseAttr.Def + bfAttr.Def) * (1 + bpAttr.Def / 10000f));
        mCurrAttr.Absorb = (int)((mBaseAttr.Absorb + bfAttr.Absorb) * (1 + bpAttr.Absorb / 10000f));
        mCurrAttr.RSpeed = (int)((mBaseAttr.RSpeed + bfAttr.RSpeed) * (1 + bpAttr.RSpeed / 10000f));
        mCurrAttr.Reflex = (int)((mBaseAttr.Reflex + bfAttr.Reflex) * (1 + bpAttr.Reflex / 10000f));
        mCurrAttr.Suck = (int)((mBaseAttr.Suck + bfAttr.Suck) * (1 + bpAttr.Suck / 10000f));
        mCurrAttr.Crit = (int)((mBaseAttr.Crit + bfAttr.Crit) * (1 + bpAttr.Crit / 10000f));
        mCurrAttr.CritDamage = (int)((mBaseAttr.CritDamage + bfAttr.CritDamage) * (1 + bpAttr.CritDamage / 10000f));
        mCurrAttr.Dodge = (int)((mBaseAttr.Dodge + bfAttr.Dodge) * (1 + bpAttr.Dodge / 10000f));
        mCurrAttr.Hit = (int)((mBaseAttr.Hit + bfAttr.Hit) * (1 + bpAttr.Hit / 10000f));
        mCurrAttr.MPRecover = (int)((mBaseAttr.MPRecover + bfAttr.MPRecover) * (1 + bpAttr.MPRecover / 10000f));
        mCurrAttr.HPRecover = (int)((mBaseAttr.HPRecover + bfAttr.HPRecover) * (1 + bpAttr.HPRecover / 10000f));
        if (init)
        {
            mCurrAttr.HP = mBaseAttr.HP;
            mCurrAttr.MP = mBaseAttr.MP;
        }
        else
        {
            mCurrAttr.HP = mCurrAttr.HP > mBaseAttr.MaxHP ? mBaseAttr.MaxHP : mCurrAttr.HP;
            mCurrAttr.MP = mCurrAttr.MP > mBaseAttr.MaxMP ? mBaseAttr.MaxMP : mCurrAttr.MP;
        }
    }


    protected void ShowFlywordByDamage(Actor attacker,int damage, bool critial)
    {
        if ((Camp == EBattleCamp.A && IsMain()) || attacker == null)
        {
            if (critial)
            {
                ShowFlyword(EFlyWordType.TYPE_ENEMY_CRIT, damage);
            }
            else
            {
                ShowFlyword(EFlyWordType.TYPE_ENEMY_HURT, damage);
            }
        }
        else
        {
            switch (attacker.Type)
            {
                case EActorType.PLAYER:
                    if (critial)
                    {
                        ShowFlyword(EFlyWordType.TYPE_AVATAR_CRIT, damage);
                    }
                    else
                    {
                        ShowFlyword(EFlyWordType.TYPE_AVATAR_HURT, damage);
                    }
                    break;
                case EActorType.PARTNER:
                    if (critial)
                    {
                        ShowFlyword(EFlyWordType.TYPE_PARTNER_CRIT, damage);
                    }
                    else
                    {
                        ShowFlyword(EFlyWordType.TYPE_PARTNER_HURT, damage);
                    }
                    break;
            }
        }
    }

    protected void UpdateHealth()
    {
        GTBoardManager.Instance.Refresh(this);
        if(this is ActorMainPlayer)
        {
            GTEventCenter.FireEvent(GTEventID.UPDATE_AVATAR_HEALTH);
        }
        if (this == LevelData.MainPlayer.Partner1 ||
            this == LevelData.MainPlayer.Partner2 ||
            this == LevelData.MainPlayer.Partner3)
        {
            GTEventCenter.FireEvent(GTEventID.UPDATE_PARTNER_HEALTH);
        }
    }

    protected void UpdatePower()
    {
        if (this is ActorMainPlayer)
        {
            GTEventCenter.FireEvent(GTEventID.UPDATE_AVATAR_POWER);
        }
    }

    public void BeDamage(Actor attacker,int damage, bool critial = false)
    {
        ShowFlywordByDamage(attacker,damage, critial);
        UpdateAttr(EAttr.HP, this.mCurrAttr.HP > damage ? this.mCurrAttr.HP - damage : 0);
        UpdateHealth();
        if (this.mCurrAttr.HP<=0)
        {
            this.Command.Get<CommandDead>().Update(EDeadReason.Normal).Do();
        }
    }

    public void AddHP(int hp,bool showFlyword)
    {
        if (IsDead())
        {
            return;
        }
        int newHP = GetAttr(EAttr.HP);
        newHP = newHP + hp > GetAttr(EAttr.MaxHP) ? GetAttr(EAttr.MaxHP) : newHP + hp;
        mCurrAttr.Update(EAttr.HP, newHP);
        if (showFlyword && this is ActorMainPlayer)
        {
            ShowFlyword(EFlyWordType.TYPE_AVATAR_HEAL, hp);
        }
        this.UpdateHealth();
    }

    public void AddMP(int mp,bool showFlyword)
    {
        if (IsDead())
        {
            return;
        }
        int newMP = GetAttr(EAttr.MP);
        newMP = newMP + mp > GetAttr(EAttr.MaxHP) ? GetAttr(EAttr.MaxHP) : newMP + mp;
        mCurrAttr.Update(EAttr.MP, newMP);
        UpdatePower();
    }

    public bool UseMP(int use)
    {
        if (this.mCurrAttr.MP > use)
        {
            UpdateAttr(EAttr.MP, this.mCurrAttr.MP - use);
            UpdatePower();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool UseHP(int use)
    {
        if (this.mCurrAttr.HP > use)
        {
            UpdateAttr(EAttr.MP, this.mCurrAttr.MP - use);
            UpdateHealth();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsFullHP()
    {
        return GetAttr(EAttr.HP) >= GetAttr(EAttr.MaxHP);
    }

    public bool IsFullMP()
    {
        return GetAttr(EAttr.MP) >= GetAttr(EAttr.MaxMP);
    }

    public bool IsDead()
    {
        return FSM == FSMState.FSM_DEAD;
    }

    public bool IsDestroy()
    {
        return CacheTransform == null;
    }

    public virtual void Destroy()
    {
        mActorAction.Release();
        mMachine.Release();
        mActorAI.Release();
    }

    public virtual void Pause(bool pause)
    {
        this.ApplyAnimator(!pause);
        if (!pause)
        {
            SendStateMessage(FSMState.FSM_IDLE);
        }
    }

    public Transform GetRidePoint()
    {
        return GTTools.GetBone(CacheTransform, "Bone026");
    }

    public void OnArrive()
    {
        GotoEmptyFSM();
        if (mHost != null && mHost.GetActorEffect(EActorEffect.IS_RIDE))
        {
            mHost.OnArrive();
        }
    }

    private void InitCommands()
    {
        this.Command.Add<CommandIdle>(CheckIdle);
        this.Command.Add<CommandRunTo>(CheckRunTo);
        this.Command.Add<CommandUseSkill>(CheckUseSkill);
        this.Command.Add<CommandDead>(CheckDead);
        this.Command.Add<CommandTurn>(CheckTurnTo);
        this.Command.Add<CommandMoveTo>(CheckMoveTo);
        this.Command.Add<CommandTalk>(CheckTalk);
        this.Command.Add<CommandFrost>(CheckFrost);
        this.Command.Add<CommandStun>(CheckStun);
        this.Command.Add<CommandPalsy>(CheckPalsy);
        this.Command.Add<CommandSleep>(CheckSleep);
        this.Command.Add<CommandBlind>(CheckBlind);
        this.Command.Add<CommandFear>(CheckFear);
        this.Command.Add<CommandFixBody>(CheckFixBody);
        this.Command.Add<CommandWound>(CheckWound);
        this.Command.Add<CommandBeatBack>(CheckBeatBack);
        this.Command.Add<CommandBeatDown>(CheckBeatDown);
        this.Command.Add<CommandBeatFly>(CheckBeatFly);
        this.Command.Add<CommandFloat>(CheckFly);
        this.Command.Add<CommandHook>(CheckHook);
        this.Command.Add<CommandGrab>(CheckGrab);
        this.Command.Add<CommandVariation>(CheckVariation);
        this.Command.Add<CommandRide>(CheckRide);
        this.Command.Add<CommandJump>(CheckJump);
        this.Command.Add<CommandStealth>(CheckSteal);
        this.Command.Add<CommandReborn>(CheckReborn);
        this.Command.Add<CommandMine>(CheckMine);
        this.Command.Add<CommandInterActive>(CheckInterActive);
    }

    #region Command
    protected virtual Resp CheckIdle(CommandIdle cmd)
    {
        if (CacheTransform == null)
        {
            return Resp.N;
        }
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (FSM == FSMState.FSM_SKILL)
        {
            return Resp.N;
        }
        SendStateMessage(FSMState.FSM_IDLE, cmd);
        return Resp.Y;
    }

    //寻路至
    protected virtual Resp CheckRunTo(CommandRunTo cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (FSM == FSMState.FSM_SKILL)
        {
            return Resp.N;
        }
        if (GetAIFeature(EActorNature.CAN_MOVE) == false)
        {
            return Resp.N;
        }
        Vector3 destPosition = Vector3.zero;
        if (mVehicle.GetActorPathFinding().CanReachPosition(cmd.DestPosition) == false)
        {
            ShowWarning("300001");
            return Resp.N;
        }
        this.GetActorAI().Auto = true;
        SendStateMessage(FSMState.FSM_RUN, cmd);
        return Resp.Y; ;
    }

    //使用技能
    protected virtual Resp CheckUseSkill(CommandUseSkill cmd)
    {
        if (CacheTransform == null)
        {
            return Resp.N;
        }
        if (CannotControlSelf())
        {
            ShowWarning("100012");
            return Resp.N;
        }
        if(FSM==FSMState.FSM_SKILL)
        {
            return Resp.N;
        }
        if(GetActorEffect(EActorEffect.IS_RIDE))
        {
            ShowWarning("100011");
            return Resp.N;
        }
        SkillTree skill = this.mActorSkill.GetSkill(cmd.Pos);
        if (skill == null) return Resp.N;
        if (skill.IsInCD()) return Resp.N;
        switch (skill.CostType)
        {
            case ESkillCostType.HP:
                {
                    bool success = UseHP(skill.CostNum);
                    if (!success)
                    {
                        return Resp.N;
                    }
                }
                break;
            case ESkillCostType.MP:
                {
                    bool success = UseMP(skill.CostNum);
                    if (!success)
                    {
                        return Resp.N;
                    }
                }
                break;
        }
        cmd.LastTime = skill.StateTime;

        SendStateMessage(FSMState.FSM_SKILL, cmd);
        return Resp.Y;
    }

    //死亡
    protected virtual Resp CheckDead(CommandDead cmd)
    {
        mActorSkill.Release();
        if(GetActorEffect(EActorEffect.IS_RIDE))
        {
            OnEndRide();
        }
        SendStateMessage(FSMState.FSM_DEAD, cmd);
        return Resp.Y;
    }

    //转向
    protected virtual Resp CheckTurnTo(CommandTurn cmd)
    {
        if (GetAIFeature(EActorNature.CAN_TURN) == false)
        {
            return Resp.N;
        }
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        SendStateMessage(FSMState.FSM_TURN,cmd);
        return Resp.Y;
    }

    //强制移动
    protected virtual Resp CheckMoveTo(CommandMoveTo cmd)
    {
        if(CannotControlSelf())
        {
            return Resp.N;
        }
        if(FSM==FSMState.FSM_SKILL)
        {
            return Resp.N;
        }
        if(GetAIFeature(EActorNature.CAN_MOVE) == false)
        {
            return Resp.N;
        }
        if (this is ActorPlayer)
        {
            this.GetActorAI().Auto = false;
            SendStateMessage(FSMState.FSM_RUN, cmd);
            return Resp.Y;
        }
        return Resp.N;
    }

    //交谈
    protected virtual Resp CheckTalk(CommandTalk cmd)
    {
        SendStateMessage(FSMState.FSM_TALK, cmd);
        return Resp.Y;
    }

    //冰冻
    protected virtual Resp CheckFrost(CommandFrost cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE)== true)
        {
            return Resp.N;
        }
        mActorSkill.Release();
        SendStateMessage(FSMState.FSM_FROST, cmd);
        return Resp.Y;
    }

    //昏迷
    protected virtual Resp CheckStun(CommandStun cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE) == true)
        {
            return Resp.N;
        }
        mActorSkill.Release();
        SendStateMessage(FSMState.FSM_STUN, cmd);
        return Resp.Y;
    }

    //麻痹
    protected virtual Resp CheckPalsy(CommandPalsy cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE) == true)
        {
            return Resp.N;
        }
        mActorSkill.Release();
        SendStateMessage(FSMState.FSM_PARALY, cmd);
        return Resp.Y;
    }

    //睡眠
    protected virtual Resp CheckSleep(CommandSleep cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE) == true)
        {
            return Resp.N;
        }
        mActorSkill.Release();
        SendStateMessage(FSMState.FSM_SLEEP, cmd);
        return Resp.Y;
    }

    //致盲
    protected virtual Resp CheckBlind(CommandBlind cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE) == true)
        {
            return Resp.N;
        }
        mActorSkill.Release();
        SendStateMessage(FSMState.FSM_BLIND, cmd);
        return Resp.Y;
    }

    //恐惧
    protected virtual Resp CheckFear(CommandFear cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE) == true)
        {
            return Resp.N;
        }
        mActorSkill.Release();
        SendStateMessage(FSMState.FSM_FEAR, cmd);
        return Resp.Y;
    }

    //定身
    protected virtual Resp CheckFixBody(CommandFixBody cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE) == true)
        {
            return Resp.N;
        }
        SendStateMessage(FSMState.FSM_FIXBODY, cmd);
        return Resp.Y;
    }

    //受击
    protected virtual Resp CheckWound(CommandWound cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE) == true)
        {
            return Resp.N;
        }
        mActorSkill.Release();
        cmd.LastTime = mActorAction.GetLength("hit");
        SendStateMessage(FSMState.FSM_WOUND, cmd);
        return Resp.Y;
    }

    //击退
    protected virtual Resp CheckBeatBack(CommandBeatBack cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE) == true)
        {
            return Resp.N;
        }
        mActorSkill.Release();
        SendStateMessage(FSMState.FSM_BEATBACK, cmd);
        return Resp.Y;
    }

    //击飞
    protected virtual Resp CheckBeatFly(CommandBeatFly cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE) == true)
        {
            return Resp.N;
        }
        mActorSkill.Release();
        cmd.LastTime = mActorAction.GetLength("fly");
        SendStateMessage(FSMState.FSM_BEATFLY, cmd);
        return Resp.Y;
    }

    //击倒
    protected virtual Resp CheckBeatDown(CommandBeatDown cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE) == true)
        {
            return Resp.N;
        }
        mActorSkill.Release();
        cmd.LastTime = mActorAction.GetLength("down");
        SendStateMessage(FSMState.FSM_BEATDOWN, cmd);
        return Resp.Y;
    }

    //浮空
    protected virtual Resp CheckFly(CommandFloat cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE) == true)
        {
            return Resp.N;
        }
        mActorSkill.Release();
        SendStateMessage(FSMState.FSM_FLOATING, cmd);
        return Resp.Y;
    }

    //被勾取
    protected virtual Resp CheckHook(CommandHook cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE) == true)
        {
            return Resp.N;
        }
        mActorSkill.Release();
        SendStateMessage(FSMState.FSM_HOOK,cmd);
        return Resp.Y;
    }

    //被抓取
    protected virtual Resp CheckGrab(CommandGrab cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE) == true)
        {
            return Resp.N;
        }
        mActorSkill.Release();
        SendStateMessage(FSMState.FSM_GRAB, cmd);
        return Resp.Y;
    }

    //变形
    protected virtual Resp CheckVariation(CommandVariation cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GetActorEffect(EActorEffect.IS_DIVIVE) == true)
        {
            return Resp.N;
        }
        mActorSkill.Release();
        SendStateMessage(FSMState.FSM_VARIATION, cmd);
        return Resp.Y;
    }

    //骑乘
    protected virtual Resp CheckRide(CommandRide cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (GTLauncher.Instance.CurrSceneType != ESceneType.TYPE_CITY&&
            GTLauncher.Instance.CurrSceneType != ESceneType.TYPE_WORLD
            )
        {
            ShowWarning("300002");
            return Resp.N;
        }
        if (FSM == FSMState.FSM_RUN || FSM == FSMState.FSM_WALK||FSM==FSMState.FSM_SKILL)
        {
            ShowWarning("300003");
            return Resp.N;
        }
        if (mActorCard.GetMountID() == 0)
        {
            ShowWarning("300004");
            return Resp.N;
        }
        OnBeginRide();
        return Resp.Y;
    }

    private Resp CheckReborn(CommandReborn cmd)
    {
        cmd.LastTime = mActorAction.GetLength("fuhuo");
        SendStateMessage(FSMState.FSM_REBORN, cmd);
        return Resp.Y;
    }

    //跳跃
    protected virtual Resp CheckJump(CommandJump cmd)
    {
        if(CannotControlSelf())
        {
            return Resp.N;
        }
        if(GetActorEffect(EActorEffect.IS_RIDE)==true)
        {
            ShowWarning("100011");
            return Resp.N;
        }
        SendStateMessage(FSMState.FSM_JUMP);
        return Resp.Y;
    }

    //隐身
    protected virtual Resp CheckSteal(CommandStealth cmd)
    {
        if(FSM!=FSMState.FSM_IDLE||
           FSM!=FSMState.FSM_RUN||
           FSM!=FSMState.FSM_WALK)
        {
            return Resp.N;
        }
        this.OnBeginStealth(cmd.LastTime);
        return Resp.Y;
    }

    //交互
    private Resp CheckInterActive(CommandInterActive cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if(FSM==FSMState.FSM_DEAD)
        {
            return Resp.N;
        }
        if(cmd.AnimName=="idle")
        {
            cmd.LastTime = 1;
        }
        else
        {
            cmd.LastTime = mActorAction.GetLength(cmd.AnimName);
        }
        SendStateMessage(FSMState.FSM_INTERACTIVE,cmd);
        return Resp.Y;
    }

    private Resp CheckMine(CommandMine cmd)
    {
        if (CannotControlSelf())
        {
            return Resp.N;
        }
        if (FSM == FSMState.FSM_DEAD)
        {
            return Resp.N;
        }
        cmd.LastTime = mActorAction.GetLength("miss");
        SendStateMessage(FSMState.FSM_INTERACTIVE, cmd);
        return Resp.Y;
    }

    #endregion

    #region  Action
    public virtual void OnForceToMove(CommandMoveTo ev)
    {
        StopPathFinding();
        Vector2 delta = ev.Delta;
        CacheTransform.LookAt(new Vector3(CacheTransform.position.x + delta.x, CacheTransform.position.y, CacheTransform.position.z + delta.y));
        mCharacter.SimpleMove(mCharacter.transform.forward * GetAttr(EAttr.RSpeed));
        this.mActorAction.Play("run", null, true);
    }

    public virtual void OnPursue(CommandRunTo ev)
    {
        this.mActorPathFinding.SetOnFinished(ev.OnFinish);
        MoveTo(ev.DestPosition);
        this.mActorAction.Play("run", null, true);
    }

    public virtual void OnIdle()
    {
        StopPathFinding();
        this.mActorAction.Play("idle", null, true);
    }

    public virtual void OnTalk(CommandTalk ev)
    {
        this.mActorAction.Play("talk", null, true);
    }

    public virtual void OnDead(CommandDead ev)
    {
        StopPathFinding();
        this.mActorAction.Play("die");
        mCurrAttr.Update(EAttr.HP, 0);
        mCurrAttr.Update(EAttr.MP, 0);
        this.Release();
        this.ApplyCharacterCtrl(false);
        this.mActorAI.Release();
        CfgActor db = GTConfigManager.Instance.RdCfgActor.GetCfgById(Id);
        if ((this is ActorMainPlayer) == false)
        {
            LevelData.AllActors.Remove(this);
        }
        switch (Type)
        {
            case EActorType.PLAYER:
                if (this is ActorMainPlayer)
                {
                    GTEventCenter.FireEvent(GTEventID.RECV_MAINPLAYER_DEAD);
                }
                break;
            case EActorType.MONSTER:
                GTEventCenter.FireEvent(GTEventID.RECV_KILL_MONSTER, GUID, Id);
                GTTimerManager.Instance.AddListener(1.5f, OnDeadEnd);
                break;
        }
    }

    public virtual void OnUseSkill(CommandUseSkill ev)
    {
        StopPathFinding();
        LookAtEnemy();
        this.mActorSkill.UseSkill(ev.Pos);
    }

    public virtual void OnTurnTo(CommandTurn ev)
    {
        Vector3 pos = new Vector3(ev.LookDirection.x, CacheTransform.position.y, ev.LookDirection.z);
        CacheTransform.LookAt(pos);
    }

    public virtual void OnBeatBack(CommandBeatBack ev)
    {
        StopPathFinding();
    }

    public virtual void OnBeatDown()
    {
        StopPathFinding();
        mActorAction.Play("down", GotoEmptyFSM, false);
    }

    public virtual void OnBeatFly()
    {
        StopPathFinding();
        mActorAction.Play("fly", GotoEmptyFSM, false);
    }

    public virtual void OnWound()
    {
        StopPathFinding();
        mActorAction.Play("hit", GotoEmptyFSM, false);
    }

    public virtual void OnWalk()
    {
        StopPathFinding();
        mActorAction.Play("walk", GotoEmptyFSM, true);
    }

    public virtual void OnStun(float lastTime)
    {
        StopPathFinding();
        mActorAction.Play("yun", GotoEmptyFSM, true,1, lastTime);
    }

    public void OnJump()
    {
        StopPathFinding();
        mActorAction.Play("jump", GotoEmptyFSM, false);
    }

    public void OnReborn()
    {
        Callback callback = delegate ()
        {
            SendStateMessage(FSMState.FSM_IDLE);
        };      
        
        AddHP(mCurrAttr.MaxHP,false);
        AddMP(mCurrAttr.MaxMP,false);
        if (mCharacter != null)
        {
            mCharacter.enabled = true;
        }
        mActorAction.Play("fuhuo", callback, false);
    }

    public virtual void OnBeginRide()
    {
 
    }

    public virtual void OnEndRide()
    {

    }

    public void OnMine(CommandMine ev)
    {
        StopPathFinding();
        Callback callback = delegate ()
        {
            GotoEmptyFSM();
            if(ev.OnFinish!=null)
            {
                ev.OnFinish();
            }
        };
        mActorAction.Play("miss", callback, false);
    }

    public void OnInterActive(CommandInterActive ev)
    {
        StopPathFinding();
        Callback callback = delegate ()
        {
            GotoEmptyFSM();
            if (ev.OnFinish != null)
            {
                ev.OnFinish();
            }
        };
        mActorAction.Play(ev.AnimName, callback, false);
    }

    protected void OnBeginStealth(float lifeTime)
    {
        SetActorEffect(EActorEffect.IS_STEALTH,true);
        OnFadeOut();
    }

    protected void OnEndStealth()
    {
        SetActorEffect(EActorEffect.IS_STEALTH,false);
        OnFadeIn();
    }

    protected void OnFadeOut()
    {
        SetAlphaVertexColorOff(0.1f);
        mActorBuff.SetAllParticleEnabled(false);
    }

    protected void OnFadeIn()
    {
        SetAlphaVertexColorOn(0.1f);
        mActorBuff.SetAllParticleEnabled(true);

    }

    protected void OnDeadEnd()
    {
        mActorAction.Release();
        mMachine.Release();
        mActorAI.Release();
        GTLevelManager.Instance.DelActor(this);
    }
    #endregion

    public virtual bool CannotControlSelf()
    {
        switch (FSM)
        {
            case FSMState.FSM_STUN:
            case FSMState.FSM_FROST:
            case FSMState.FSM_FEAR:
            case FSMState.FSM_BEATFLY:
            case FSMState.FSM_BEATDOWN:
            case FSMState.FSM_BEATBACK:
            case FSMState.FSM_DROP:
            case FSMState.FSM_DEAD:
            case FSMState.FSM_FLOATING:
            case FSMState.FSM_HOOK:
            case FSMState.FSM_VARIATION:
            case FSMState.FSM_WOUND:
            case FSMState.FSM_GRAB:
            case FSMState.FSM_SLEEP:
            case FSMState.FSM_PARALY:
            case FSMState.FSM_BLIND:
            case FSMState.FSM_JUMP:
            case FSMState.FSM_REBORN:
                return true;
            default:
                return false;
        }
    }

    public GTAction GetActorAction()
    {
        return mActorAction;
    }

    public ActorPathFinding GetActorPathFinding()
    {
        return mActorPathFinding;
    }

    public ActorSkill GetActorSkill()
    {
        return mActorSkill;
    }

    public ActorBuff GetActorBuff()
    {
        return mActorBuff;
    }

    public CharacterAttr GetCurrAttr()
    {
        return mCurrAttr;
    }

    public ActorCard GetActorCard()
    {
        return mActorCard;
    }

    public ActorAI GetActorAI()
    {
        return mActorAI;
    }

    public int GetAttr(EAttr attr)
    {
        return this.mCurrAttr.GetAttr(attr);
    }

    public EquipAvatar GetEquipModelsByPos(int pos)
    {
        EquipAvatar pModel = null;
        mEquipAvatars.TryGetValue(pos, out pModel);
        if (pModel==null)
        {
            pModel = new EquipAvatar();
            mEquipAvatars.Add(pos,pModel);
        }
        return pModel;
    }

    public void RemoveEquip(int pos)
    {
        EquipAvatar pModel = GetEquipModelsByPos(pos);
        for(int i=0;i<pModel.Models.Length;i++)
        {
            if(pModel.Models[i]!=null)
            {
                pModel.Models[i].SetActive(false);
                GameObject.Destroy(pModel.Models[i]);
            }
        }
    }

    public void ChangeEquipAvatar(int pos1, int pTargetPos)
    {
        RemoveEquip(pTargetPos - 1);
        XEquip equip = GTDataManager.Instance.GetDressEquipByPos(pTargetPos);
        if (equip != null)
        {
            ChangeEquip(pTargetPos - 1, equip.Id);
        }
    }

    public void ChangeEquip(int pDressPos, int pEquipID)
    {
        CfgItem itemDB = GTConfigManager.Instance.RdCfgItem.GetCfgById(pEquipID);
        string[] modelPaths = { itemDB.Model_R, itemDB.Model_L };
        EquipAvatar pModel = GetEquipModelsByPos(pDressPos);
        for (int i = 0; i < 2; i++)
        {
            string path = modelPaths[i];
            string bone = EQUIP_BONES[pDressPos, i];
            if (string.IsNullOrEmpty(path)) { continue; }
            Transform boneTrans = GTTools.GetBone(CacheTransform, bone);
            if (boneTrans == null) { continue; }
            pModel.Models[i] = GTResourceManager.Instance.Load<GameObject>(path, true);
            if (pModel.Models[i] != null)
            {
                GameObject model = pModel.Models[i];
                model.transform.parent = boneTrans;
                NGUITools.SetLayer(pModel.Models[i], Obj.layer);
                model.transform.localPosition = Vector3.zero;
                model.transform.localEulerAngles = Vector3.zero;
                model.transform.localScale = Vector3.one;
            }
        }
    }

    protected void LoadMount()
    {
        XTransform param = XTransform.Create(CacheTransform.position, CacheTransform.eulerAngles);
        Mount = GTLevelManager.Instance.AddActor(mActorCard.GetMountID(), EActorType.MOUNT, EBattleCamp.C, param);
        mVehicle = mMount;
        Transform ridePoint = mMount.GetRidePoint();
        if (ridePoint != null)
        {
            CacheTransform.parent = ridePoint;
            CacheTransform.localPosition = Vector3.zero;
            CacheTransform.localRotation = Quaternion.identity;
        }
    }

    public void ChangeModel(int modelID)
    {

    }

    public virtual int Attack(Actor defender,int value)
    {
        if(defender==null||value<=0)
        {
            return 0;
        }
        float v= (value - defender.GetAttr(EAttr.Def)*0.2f);
        v = v * GTDefine.V_DAMAGE_RATIO;
        if(v<=0)
        {
            v = UnityEngine.Random.Range(3, 7);
        }

        float cRate = this.GetAttr(EAttr.Crit) *0.01f;
        float bRate = this.GetAttr(EAttr.CritDamage) *0.01f;
        bool critical = GTTools.IsTrigger(cRate);
        if(critical)
        {
            v = (v * (1 + bRate));
        }
        int dmg = Mathf.FloorToInt(UnityEngine.Random.Range(0.85f, 1.08f)*v);
        defender.BeDamage(this, dmg, critical);
        return dmg;
    }

    public virtual int SuckBlood(Actor defender,int value,float suckRate)
    {
        if (defender == null || value <= 0)
        {
            return 0;
        }
        int v = Attack(defender,value);
        if(suckRate<0)
        {
            suckRate = 0;
        }
        int suckValue = Mathf.FloorToInt(v * suckRate);
        AddHP(suckValue,true);
        return suckValue;
    }

    public virtual void SetAlphaVertexColorOff(float time)
    {

    }

    public virtual void SetAlphaVertexColorOn(float time)
    {

    }

    public bool IsBoss()
    {
        return MonsterType == EActorSort.Boss || MonsterType == EActorSort.World;
    }

    public bool IsChest()
    {
        return MonsterType == EActorSort.Chest;
    }

    public void LookAtEnemy()
    {
        if (mTarget == null || mTarget.IsDead() || !IsEnemy(mTarget) || mTarget.GetActorEffect(EActorEffect.IS_STEALTH) == true)
        {
            mTarget = null;
        }
        Actor enemy = GetNearestEnemy(mActorAI.WARDIST);
        this.SetTarget(enemy);
        if (mTarget != null)
        {
            CacheTransform.LookAt(new Vector3(mTarget.Pos.x, Pos.y, mTarget.Pos.z));
        }
    }

    protected void ShowWarning(string localKey)
    {
        if (!mActorAI.Auto)
        {
            GTItemHelper.ShowTip(localKey);
        }
    }

    public void Release()
    {
        RemoveBoard();
        RemoveEffect();
        for(int i=0;i<8;i++)
        {
            RemoveEquip(i);
        }
    }

    public void TryMoveTo(Vector3 destPosition)
    {

    }

    public void TryTranTo(Vector3 destPosition)
    {

    }

    public static string[,] EQUIP_BONES = new string[8, 2]
    {
        {"NULL" ,"NULL"},
        {"NULL" ,"NULL"},
        {"NULL" ,"NULL"},
        {"NULL" ,"NULL"},
        {"NULL" ,"NULL"},
        {"NULL" ,"NULL"},
        {"NULL" ,"NULL"},
        {"Bip01 Prop1" ,"Bip01 Prop2"},
    };
}
