using UnityEngine;
using System.Collections;

public class GTCtrlManager : GTSingleton<GTCtrlManager>
{
    public void AddAllCtrls()
    {
        AdventureCtrl. Instance.AddEventListeners();
        AwardCtrl.     Instance.AddEventListeners();
        BagCtrl.       Instance.AddEventListeners();
        CharacterCtrl. Instance.AddEventListeners();
        EquipCtrl.     Instance.AddEventListeners();
        GemCtrl.       Instance.AddEventListeners();
        LoginCtrl.     Instance.AddEventListeners();
        MountCtrl.     Instance.AddEventListeners();
        PartnerCtrl.   Instance.AddEventListeners();
        PetCtrl.       Instance.AddEventListeners();
        RelicsCtrl.    Instance.AddEventListeners();
        RaidCtrl.      Instance.AddEventListeners();
        RoleCtrl.      Instance.AddEventListeners();
        SkillCtrl.     Instance.AddEventListeners();
        StoreCtrl.     Instance.AddEventListeners();
        TaskCtrl.      Instance.AddEventListeners();

    }

    public void DelAllCtrls()
    {
        AdventureCtrl. Instance.DelEventListeners();
        AwardCtrl.     Instance.DelEventListeners();
        BagCtrl.       Instance.DelEventListeners();
        CharacterCtrl. Instance.DelEventListeners();
        EquipCtrl.     Instance.DelEventListeners();
        GemCtrl.       Instance.DelEventListeners();
        LoginCtrl.     Instance.DelEventListeners();
        MountCtrl.     Instance.DelEventListeners();
        PartnerCtrl.   Instance.DelEventListeners();
        PetCtrl.       Instance.DelEventListeners();
        RelicsCtrl.    Instance.DelEventListeners();
        RaidCtrl.      Instance.DelEventListeners();
        RoleCtrl.      Instance.DelEventListeners();
        SkillCtrl.     Instance.DelEventListeners();
        StoreCtrl.     Instance.DelEventListeners();
        TaskCtrl.      Instance.DelEventListeners();
    }
}
