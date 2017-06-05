using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;

public class CharacterManager : GTMonoSingleton<CharacterManager>
{
    public static List<Character>  Characters = new List<Character>();
    public static List<Character>  Bosses = new List<Character>();
    public static Character Main { get; set; }
    public static Character Enem { get; set; }

    public Character AddActor(int id, EBattleCamp camp, EActorType type, KTransform bornData, XCharacter data, bool isMain = false)
    {
        Character cc = new Character(id, GTGUID.NewGUID(), type, camp, isMain);
        bornData.Pos = GTTools.NavSamplePosition(bornData.Pos);
        cc.Load(bornData);
        cc.SyncData(data, ESyncDataType.TYPE_ALL);
        cc.CacheTransform.parent = transform;
        Characters.Add(cc);
        return cc;
    }

    public Character AddActorNoneSync(int id, EBattleCamp camp, EActorType type, KTransform bornData)
    {
        return AddActor(id, camp, type, bornData, null, false);
    }

    public Character AddActorSync(XCharacter data)
    {
        if (data == null)
        {
            return null;
        }
        if (data.AOI == null)
        {
            return null;
        }
        KTransform dTrans = KTransform.Create(data.AOI);
        Character cc = AddActor(data.Id, (EBattleCamp)data.Camp, (EActorType)data.Type, dTrans, data, false);
        return cc;
    }

    public Character AddRole(int id, KTransform bornData)
    {
        return AddActorNoneSync(id, EBattleCamp.D, EActorType.PLAYER, bornData);
    }

    public Character AddMainPlayer(KTransform bornData)
    {
        XCharacter data = RoleModule.Instance.GetMainPlayer();
        Main = AddActor(data.Id, EBattleCamp.A, EActorType.PLAYER, bornData, data, true);
        return Main;
    }

    public Character AddMainPartner(int pos)
    {
        XCharacter data = RoleModule.Instance.GetMainPartnerByPos(pos);
        if(data==null)
        {
            return null;
        }
        KTransform bornData = new KTransform();
        switch(pos)
        {
            case 1:
                bornData.Euler = Main.Euler;
                bornData.Pos = Main.Pos + new Vector3(-2, 0, 0);
                break;
            case 2:
                bornData.Euler = Main.Euler;
                bornData.Pos = Main.Pos + new Vector3( 2, 0, 0);
                break;                           
        }
        Character actor = AddActor(data.Id, EBattleCamp.A, EActorType.PARTNER, bornData, data);
        actor.Host = Main;
        return actor;
    }

    public Character DelActor(Character cc)
    {
        Characters.Remove(cc);
        cc.Release();
        return cc;
    }

    public Character GetActor(int guid)
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            if(Characters[i].GUID==guid)
            {
                return Characters[i];
            }
        }
        return null;
    }

    public CharacterAvatar AddAvatar(int modelID)
    {
        DActorModel cfg = ReadCfgActorModel.GetDataById(modelID);
        if (cfg == null)
        {
            return null;
        }
        GameObject obj = GTResourceManager.Instance.Load<GameObject>(cfg.Model, true);
        if (obj == null)
        {
            return null;
        }
        CharacterAvatar avatar = new CharacterAvatar(obj.transform);
        avatar.StartupComponent();
        CharacterController cc = obj.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }
        return avatar;
    }

    public CharacterAvatar DelAvatar(CharacterAvatar avatar)
    {
        if (avatar == null)
        {
            return null;
        }
        avatar.ReleaseComponent();
        GTResourceManager.Instance.DestroyObj(avatar.RootObj);
        return null;
    }

    public void SetCharacterParent(Character cc)
    {
        cc.CacheTransform.parent = transform;
    }

    public void DelCharacters()
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            Character cc = Characters[i];
            cc.Release();
        }
        Characters.Clear();
        Main = null;
        Enem = null;
    }
}
