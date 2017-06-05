using UnityEngine;
using System.Collections;
using System;

public class ActorCard
{
    public EActorRace Race;
    public ETargetCamp Camp;
    public EActorSex  Sex;
    public string Title = string.Empty;
    public string Name = string.Empty;
    public Int32   Group;
    public Int32   Level;
    public Int32   MountID;
    public Int32[] Partners = { 0, 0 ,0};
    public Actor   Owner;

    public ActorCard(Actor pOwner)
    {
        Owner = pOwner;
        this.SetName();
        this.SetLevel();
        if (Owner is ActorMainPlayer)
        {
            XRole role = GTDataManager.Instance.GetCurRole();
            this.SetPartnerByPos(1, role.PartnerID1);
            this.SetPartnerByPos(2, role.PartnerID2);
            this.SetPartnerByPos(3, role.PartnerID3);
        }
    }

    public void SetName(string pName = null)
    {
        if (!string.IsNullOrEmpty(pName))
        {
            this.Name = pName;
            return;
        }

        CfgActor db = GTConfigManager.Instance.RdCfgActor.GetCfgById(Owner.Id);
        if (db == null)
        {
            return;
        }
        if (Owner is ActorMainPlayer)
        {
            XRole role = GTDataManager.Instance.GetCurRole();
            this.Name = (role == null) ? string.Empty : role.Name;
        }
        else
        {
            this.Name = db.Name;
        }
    }

    public void SetLevel(int pLevel = -1)
    {
        if (pLevel > 0)
        {
            this.Level = pLevel;
            return;
        }
        if (Owner is ActorMainPlayer)
        {
            XRole role = GTDataManager.Instance.GetCurRole();
            this.Level = (role == null) ? 1 : role.Level;
        }
        else
        {
            CfgActor db = GTConfigManager.Instance.RdCfgActor.GetCfgById(Owner.Id);
            if (db == null)
            {
                return;
            }
            this.Level = db.Level;
        }
    }

    public void SetMount(int pMountID)
    {
        MountID = pMountID;
    }

    public void SetPartnerByPos(int pos, int id)
    {
        Partners[pos - 1] = id;
    }

    public void SetTitle(string pTitle)
    {
        Title = pTitle;
    }

    public Int32 GetMountID()
    {
        if (Owner is ActorMainPlayer)
        {
            MountID = GTDataManager.Instance.GetCurRole().MountID;
        }
        else
        {
            MountID = UnityEngine.Random.Range(100001, 100012);
        }
        return MountID;
    }
}
