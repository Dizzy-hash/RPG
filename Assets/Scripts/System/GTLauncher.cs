using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Xml;
using System.IO;
using System.Reflection;

public class GTLauncher : MonoBehaviour
{
    public static GTLauncher  Instance;

    public bool ShowFPS;
    public bool UseGuide = true;
    public bool MusicDisable = true;

    public GameState currGameState;
    public SceneState currSceneState;
    public SceneState nextSceneState;
    private IStateMachine<GTLauncher, GameState> mGameStateMachine;
    private IStateMachine<GTLauncher, SceneState> mSceneStateMachine;

    public int lastMapId = 3;

    private SceneType currSceneType;

    void Awake()
    {
        Application.runInBackground = true;
        Instance = this;
        DontDestroyOnLoad(gameObject);
        PlayCGMovie();
    }

    void Start()
    {
        TryShowFPS();
        IgnorePhysicsLayer();
        AddFSM();
        LoadManager();
        OpenTag();
        StartGame();
    }

    void OpenTag()
    {
        GTLog.Open(GTLogTag.TAG_ACTOR);
    }

    void LoadManager()
    {
        NetworkManager.       Instance.Init();
        NetworkManager.       Instance.Start("127.0.0.1", 3001);

        GTCoroutinueManager.  Instance.SetDontDestroyOnLoad(transform);
        GTResourceManager.    Instance.SetDontDestroyOnLoad(transform);
        GTAudioManager.       Instance.SetDontDestroyOnLoad(transform);
        GTCameraManager.      Instance.SetDontDestroyOnLoad(transform);
        GTConfigManager.      Instance.Init();

        GTWindowManager.      Instance.Init();
        GTInputManager.       Instance.SetDontDestroyOnLoad(transform);
        GTWorld.              Instance.SetDontDestroyOnLoad(transform);
        GTTouchEffect.        Instance.SetDontDestroyOnLoad(transform);

        GTTimerManager.       Instance.AddListener(1, SecondTick, 0);
        GTCtrlManager.        Instance.AddAllCtrls();
    }

    void AddFSM()
    {
        this.mGameStateMachine = new IStateMachine<GTLauncher, GameState>(this);
        this.mGameStateMachine.AddState(GameState.None, new GameNoneState());
        this.mGameStateMachine.AddState(GameState.Update, new GameUpdateState());
        this.mGameStateMachine.AddState(GameState.Loading, new GameLoadingState());
        this.mGameStateMachine.AddState(GameState.Battle, new GameBattleState());
        this.mGameStateMachine.SetCurState(this.mGameStateMachine.GetState(GameState.None));

        this.mSceneStateMachine = new IStateMachine<GTLauncher, SceneState>(this);
        this.mSceneStateMachine.AddState(SceneState.None, new SceneNone());
        this.mSceneStateMachine.AddState(SceneState.Login, new SceneLogin());
        this.mSceneStateMachine.AddState(SceneState.Create, new SceneCreate());
        this.mSceneStateMachine.AddState(SceneState.City, new SceneCity());
        this.mSceneStateMachine.AddState(SceneState.World, new SceneWorld());
        this.mSceneStateMachine.SetCurState(this.mSceneStateMachine.GetState(SceneState.None));
    }

    void SecondTick()
    {
        GTEventCenter.FireEvent(GTEventID.TYPE_SECOND_TICK);
    }

    void IgnorePhysicsLayer()
    {
        Physics.IgnoreLayerCollision(GTLayer.LAYER_AVATAR,  GTLayer.LAYER_PARTNER);
        Physics.IgnoreLayerCollision(GTLayer.LAYER_AVATAR,  GTLayer.LAYER_PET);
        Physics.IgnoreLayerCollision(GTLayer.LAYER_AVATAR,  GTLayer.LAYER_MONSTER);
        Physics.IgnoreLayerCollision(GTLayer.LAYER_AVATAR,  GTLayer.LAYER_NPC);
        Physics.IgnoreLayerCollision(GTLayer.LAYER_AVATAR,  GTLayer.LAYER_PLAYER);
        Physics.IgnoreLayerCollision(GTLayer.LAYER_MOUNT,   GTLayer.LAYER_PARTNER);
        Physics.IgnoreLayerCollision(GTLayer.LAYER_MOUNT,   GTLayer.LAYER_PET);
        Physics.IgnoreLayerCollision(GTLayer.LAYER_MOUNT,   GTLayer.LAYER_MONSTER);
        Physics.IgnoreLayerCollision(GTLayer.LAYER_MOUNT,   GTLayer.LAYER_NPC);
        Physics.IgnoreLayerCollision(GTLayer.LAYER_PARTNER, GTLayer.LAYER_BARRER);
    }

    public void StartGame()
    {
        LoadScene((int)SceneID.Login);
    }

    public void ChangeGameState(GameState state, ICommand ev = null)
    {
        if(currGameState==state)
        {
            return;
        }
        this.mGameStateMachine.GetState(state).SetCommand(ev);
        this.mGameStateMachine.ChangeState(state);
        this.currGameState = state;
    }

    public void ChangeSceneState(SceneState state)
    {
        if (currSceneState == state)
        {
            return;
        }
        this.mSceneStateMachine.GetState(state);
        CurScene = (IScene)this.mSceneStateMachine.GetState(state);
        this.currSceneState = state;
    }

    public void LoadScene(int sceneId)
    {
        DScene db = ReadCfgScene.GetDataById(sceneId);
        if (db.SceneType != SceneType.Login && db.SceneType != SceneType.Create)
        {
            lastMapId = sceneId;
        }
        switch(db.SceneType)
        {
            case SceneType.Login:
                {
                    nextSceneState = SceneState.Login;
                }
                break;
            case SceneType.Create:
                {
                    DataManager.Instance.LoadCommonData();
                    nextSceneState = SceneState.Create;
                }
                break;
            case SceneType.City:
                {
                    if (this.currSceneType == SceneType.Create)
                    {
                        DataManager.Instance.LoadRoleData(CurPlayerID);
                        DataTimer.Instance.Init();
                    }
                    nextSceneState = SceneState.City;
                }
                break;
            case SceneType.World:
                {
                    if (this.currSceneType == SceneType.Create)
                    {
                        DataManager.Instance.LoadRoleData(CurPlayerID);
                        DataTimer.Instance.Init();
                    }
                    nextSceneState = SceneState.World;
                }
                break;
        }
        CommandLoadScene cmd = new CommandLoadScene();
        cmd.SceneID = sceneId;
        ChangeGameState(GameState.Loading, cmd);
    }

    public void TryShowFPS()
    {
        if (ShowFPS)
        {
            GameObject go = new GameObject("FPS");
            go.AddComponent<EFPS>();
            go.transform.parent = transform;
        }
    }

    public AsyncOperation LoadLevelById(int id)
    {
        DScene db = ReadCfgScene.GetDataById(id);
        CurSceneID = db.Id;
        if (string.IsNullOrEmpty(db.SceneName))
        {
            return null;
        }
        ReleaseResource();
        return SceneManager.LoadSceneAsync(db.SceneName);
    }

    public void ReleaseResource()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    public void PlayCGMovie()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Handheld.PlayFullScreenMovie("CG.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
    }

    void Update()
    {
        if (this.mGameStateMachine != null)
        {
            this.mGameStateMachine.Execute();
        }
        if (this.mSceneStateMachine != null)
        {
            this.mSceneStateMachine.Execute();
        }
        GTTimerManager.  Instance.Execute();
        GTUpdate.        Instance.Execute();
        NetworkManager.  Instance.Execute();
    }

    void FixedUpdate()
    {
        GTAction.Update();
    }

    void OnApplicationQuit()
    {
        GTTimerManager.Instance.DelListener(SecondTick);
        DataTimer.Instance.Exit();
        ReleaseResource();
    }

    public static ELanguage Language
    {
        get; set;
    }

    public static float     TimeScale
    {
        get { return Time.timeScale; }
        set { Time.timeScale = value; }
    }

    public static IScene    CurScene
    {
        get;
        private set;
    }

    public static Int32     CurSceneID
    {
        get;
        private set;
    }

    public static Int32     CurPlayerID
    {
        get;
        set;
    }

    public static string    LoadedLevelName
    {
        get { return SceneManager.GetActiveScene().name; }
    }
}
