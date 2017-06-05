using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

public class PartnerCtrl : GTSingleton<PartnerCtrl> ,ICtrl
{
    public void DelEventListeners()
    {

    }


    public void AddEventListeners()
    {
        NetworkManager.AddListener(MessageID.MSG_ACK_CHANGE_PARTNER,  OnAck_ChangePartner);
        NetworkManager.AddListener(MessageID.MSG_ACK_UPGRADE_PARTNER, OnAck_UpgradePartner);
        NetworkManager.AddListener(MessageID.MSG_ACK_ADVANVE_PARTNER, OnAck_AdvancePartner);
    }

    private void OnAck_AdvancePartner(MessageRecv obj, MessageRetCode retCode)
    {
        System.IO.MemoryStream ms = new System.IO.MemoryStream(obj.Packet.Data);
        AckAdvancePartner ack = Serializer.Deserialize<AckAdvancePartner>(ms);

        XPartner partner = DataDBSPartner.GetDataById(ack.ID);
        if (partner == null)
        {
            partner = new XPartner();
            partner.Id = ack.ID;
            partner.Advance = 1;
        }
        else
        {
            partner.Advance++;
        }
        DataDBSPartner.Update(ack.ID, partner);

        GTEventCenter.FireEvent(GTEventID.TYPE_ADVANCE_PARTNER);
        GTEventCenter.FireEvent(GTEventID.TYPE_CHANGE_FIGHTVALUE);
    }

    private void OnAck_UpgradePartner(MessageRecv obj, MessageRetCode retCode)
    {
        System.IO.MemoryStream ms = new System.IO.MemoryStream(obj.Packet.Data);
        AckUpgradePartner ack = Serializer.Deserialize<AckUpgradePartner>(ms);

        XPartner partner = DataDBSPartner.GetDataById(ack.ID);
        if (partner == null)
        {
            partner = new XPartner();
            partner.Id = ack.ID;
            partner.Level = 1;
        }
        else
        {
            partner.Level++;
        }
        DataDBSPartner.Update(ack.ID, partner);

        GTEventCenter.FireEvent(GTEventID.TYPE_UPGRADE_PET);
        GTEventCenter.FireEvent(GTEventID.TYPE_CHANGE_FIGHTVALUE);
    }

    private void OnAck_ChangePartner(MessageRecv obj, MessageRetCode retCode)
    {
        System.IO.MemoryStream ms = new System.IO.MemoryStream(obj.Packet.Data);
        AckChangePartner ack = Serializer.Deserialize<AckChangePartner>(ms);

        XCharacter role = RoleModule.Instance.GetCurPlayer();
        switch (ack.Pos)
        {
            case 1:
                role.Partner1 = ack.ID;
                break;
            case 2:
                role.Partner2 = ack.ID;
                break;
        }
        DataDBSRole.Update(role.Id, role);

        GTEventCenter.FireEvent(GTEventID.TYPE_CHANGE_PARTNER, ack.Pos, ack.ID);
        GTEventCenter.FireEvent(GTEventID.TYPE_CHANGE_FIGHTVALUE);
    }
}
