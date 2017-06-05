using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelData
{
    public static List<Actor> AllActors = new List<Actor>();
    public static Dictionary<EBattleCamp, List<Actor>> CampActors = new Dictionary<EBattleCamp, List<Actor>>()
    {
        {EBattleCamp.A,new List<Actor>() },
        {EBattleCamp.B,new List<Actor>() },
        {EBattleCamp.C,new List<Actor>() },
        {EBattleCamp.D,new List<Actor>() }
    };
    public static int Chapter;
    public static int SceneID;
    public static int CopyID;
    public static ECopyType CopyType;
    public static float StrTime;
    public static float EndTime;
    public static bool Win;

    public static ActorMainPlayer MainPlayer = null;
    public static ActorPlayer     EnemPlayer = null;

    public static List<Actor> GetActorsByActorType(EActorType pType)
    {
        List<Actor> pList = new List<Actor>();
        for (int i = 0; i < AllActors.Count; i++)
        {
            if (AllActors[i].Type == pType)
            {
                pList.Add(AllActors[i]);
            }
        }
        return pList;
    }

    public static float CurTime
    {
        get { return Time.realtimeSinceStartup - StrTime; }
    }

    public static Callback Call
    {
        get; set;
    }

    public static int Star
    {
        get;
        private set;
    }

    public static bool[] PassContents
    {
        get;
        private set;
    }

    public static void CalcResult()
    {
        CfgCopy copyDB = GTConfigManager.Instance.RdCfgCopy.GetCfgById(CopyID);
        if (copyDB == null)
        {
            return;
        }
        Star = CalcStar(copyDB);
        if (Call != null)
        {
            Call();
        }
        RaidMediator.Instance.TryPassCopy(CopyType, Chapter, CopyID, Star);
    }

    public static void Reset()
    {
        CopyID = 0;
        Win = false;
        Call = null;
    }

    static int CalcStar(CfgCopy copyDB)
    {
        int star = 0;
        PassContents = new bool[3] { false, false, false };
        if (Win == false)
        {
            return 0;
        }
        for (int i = 0; i < copyDB.StarConditions.Length; i++)
        {
            EStarCondition type = copyDB.StarConditions[i];
            int v = copyDB.StarValues[i];
            switch (type)
            {
                case EStarCondition.TYPE_MAIN_HEALTH:
                    {
                        if (MainPlayer != null)
                        {
                            int maxHealth = MainPlayer.GetAttr(EAttr.MaxHP);
                            int curHealth = MainPlayer.GetAttr(EAttr.HP);
                            float ratio = curHealth / (maxHealth * 1f);
                            if (ratio >= v / 100f)
                            {
                                star++;
                                PassContents[i] = true;
                            }
                        }
                    }
                    break;
                case EStarCondition.TYPE_PASSCOPY:
                    {
                        star++;
                        PassContents[i] = true;
                    }
                    break;
                case EStarCondition.TYPE_TIME_LIMIT:
                    {
                        if (CurTime < v)
                        {
                            star++;
                            PassContents[i] = true;
                        }
                    }
                    break;
            }
        }
        return star;
    }

    public static void ShowResult()
    {
        CfgCopy copyDB = GTConfigManager.Instance.RdCfgCopy.GetCfgById(CopyID);
        if (copyDB == null)
        {
            return;
        }
        switch (copyDB.CopyType)
        {
            case ECopyType.TYPE_EASY:
            case ECopyType.TYPE_WORLD:
            case ECopyType.TYPE_ELITE:
            case ECopyType.TYPE_DAILY:
                {
                    GTWindowManager.Instance.OpenWindow(EWindowID.UI_MAINRESULT);
                    UIMainResult window = (UIMainResult)GTWindowManager.Instance.GetWindow(EWindowID.UI_MAINRESULT);
                    window.ShowView();
                }
                break;
        }
    }
}
