using UnityEngine;
using System.Collections;
using System;
using Protocol;
using ProtoBuf;

public class MountCtrl : GTSingleton<MountCtrl>, ICtrl
{
    public void DelEventListeners()
    {

    }

    public void AddEventListeners()
    {
        NetworkManager.AddListener(MessageID.MSG_ACK_SETMOUNT, OnAck_SetMount);
    }

    private void OnAck_SetMount(MessageRecv obj, MessageRetCode retCode)
    {
        System.IO.MemoryStream ms = new System.IO.MemoryStream(obj.Packet.Data);
        AckSetMount ack = Serializer.Deserialize<AckSetMount>(ms);

        XCharacter role = RoleModule.Instance.GetCurPlayer();
        role.Mount = ack.MountID;
        DataDBSRole.Update(role.Id, role);
        GTEventCenter.FireEvent(GTEventID.TYPE_DRESS_MOUNT);
    }

}
