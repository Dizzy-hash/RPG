﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Protocol;

public class UIPartner : GTWindow
{
    private GameObject     btnReturn;
    private PartnerElem[]  mPartnerDress;
    private PartnerItem[]  mPartnerArray;
    private UIScrollView   mScroll;
    private UIGrid mGrid;

    class PartnerElem
    {
        public UISprite   quality;
        public UISprite   icon;
        public UILabel    name;
        public GameObject btn;
        public UISprite   type;
    }

    class PartnerItem
    {
        public PartnerElem elem = new PartnerElem();
        public UILabel type1;
        public UILabel type2;
        public UILabel type3;
        public GameObject btnBattle;
        public int id;
        public GameObject btn;
    }

    public UIPartner()
    {
        Type = EWindowType.WINDOW;
        mResident = false;
        mResPath = "Partner/UIPartner";
    }

    protected override void OnAwake()
    {
        btnReturn = transform.Find("Top/Btn_Return").gameObject;
        Transform transDress = transform.Find("PartnerDress");
        Transform transList  = transform.Find("PartnerList");
        mPartnerDress = new PartnerElem[2];
        for (int i = 1; i <= 2; i++)
        {
            Transform trans = transDress.Find("Partner"+i);
            PartnerElem tab = new PartnerElem();
            tab.icon = trans.Find("Icon").GetComponent<UISprite>();
            tab.quality = trans.Find("Quality").GetComponent<UISprite>();
            tab.name = trans.Find("Name").GetComponent<UILabel>();
            tab.btn = trans.gameObject;
            tab.type= trans.Find("Type").GetComponent<UISprite>();
            mPartnerDress[i - 1] = tab;
        }

        Dictionary<int, DPartner> dict = ReadCfgPartner.Dict;
        Dictionary<int, DPartner>.Enumerator em = dict.GetEnumerator();
        mPartnerArray = new PartnerItem[dict.Count];

        mScroll = transList.Find("View").GetComponent<UIScrollView>();
        mGrid   = transList.Find("View/Grid").GetComponent<UIGrid>();
        GameObject temp = transList.Find("View/Temp").gameObject;
        temp.SetActive(false);
        int a = 0;
        while (em.MoveNext())
        {
            GameObject go = NGUITools.AddChild(mGrid.gameObject, temp);
            go.SetActive(true);
            Transform trans = go.transform;
            PartnerItem tab = new PartnerItem();
            tab.elem.icon = trans.Find("Icon").GetComponent<UISprite>();
            tab.elem.quality = trans.Find("Quality").GetComponent<UISprite>();
            tab.elem.name = trans.Find("Name").GetComponent<UILabel>();
            tab.elem.btn = trans.gameObject;
            tab.elem.type = trans.Find("Type").GetComponent<UISprite>();
            tab.id = em.Current.Key;
            tab.type1 = trans.Find("Type1/Value").GetComponent<UILabel>();
            tab.type2 = trans.Find("Type2/Value").GetComponent<UILabel>();
            tab.type3 = trans.Find("Type3/Value").GetComponent<UILabel>();
            tab.btnBattle = trans.Find("Btn_Battle").gameObject;
            tab.btn = trans.gameObject;
            mPartnerArray[a] = tab;
            a++;
        }
        em.Dispose();
        
    }

    protected override void OnAddButtonListener()
    {
        UIEventListener.Get(btnReturn).onClick = OnReturnClick;
        for (int i = 0; i < mPartnerDress.Length; i++)
        {
            int index = i;
            PartnerElem tab = mPartnerDress[i];
            UIEventListener.Get(tab.btn).onClick = (GameObject go) =>
            {
                GTAudioManager.Instance.PlayEffectAudio(GTAudioKey.SOUND_UI_CLICK);
                XCharacter role = RoleModule.Instance.GetCurPlayer();
                int[] array = { role.Partner1, role.Partner2};
                if (array[index] == 0)
                {
                    return;
                }
                UIPartnerStrength window = (UIPartnerStrength)GTWindowManager.Instance.OpenWindow(EWindowID.UI_PARTNERSTRENGH);
                window.SetID(array[index]);
            };
        }
        for (int i = 0; i < mPartnerArray.Length; i++)
        {
            PartnerItem tab = mPartnerArray[i];
            UIEventListener.Get(tab.btnBattle).onClick = (GameObject go) =>
            {
                GTAudioManager.Instance.PlayEffectAudio(GTAudioKey.SOUND_UI_CLICK);
                UIPartnerBattle window=(UIPartnerBattle)GTWindowManager.Instance.OpenWindow(EWindowID.UI_PARTNERBATTLE);
                window.SetID(tab.id);

            };
            UIEventListener.Get(tab.btn).onClick = (GameObject go) =>
            {
                GTAudioManager.Instance.PlayEffectAudio(GTAudioKey.SOUND_UI_CLICK);
                UIPartnerStrength window = (UIPartnerStrength)GTWindowManager.Instance.OpenWindow(EWindowID.UI_PARTNERSTRENGH);
                window.SetID(tab.id);
            };
        }
    }

    protected override void OnAddHandler()
    {
        GTEventCenter.AddHandler<int, int>(GTEventID.TYPE_CHANGE_PARTNER, OnRecvChangePartner);
    }

    protected override void OnDelHandler()
    {
        GTEventCenter.DelHandler<int, int>(GTEventID.TYPE_CHANGE_PARTNER, OnRecvChangePartner);
    }

    protected override void OnEnable()
    {
        ShowPartnerDressView();
        ShowPartnerArrayView();
    }

    protected override void OnClose()
    {

    }

    private void ShowPartnerDressView()
    {
        XCharacter role = RoleModule.Instance.GetCurPlayer();
        int[] array = { role.Partner1, role.Partner2};
        for (int i=0;i<mPartnerDress.Length;i++)
        {
            PartnerElem tab = mPartnerDress[i];
            int id = array[i];
            tab.icon.enabled = id > 0;
            tab.name.enabled = id > 0;
            if (id > 0)
            {
                DActor db = ReadCfgActor.GetDataById(id);
                GTItemHelper.ShowQualityText(tab.name, db.Name,db.Quality);
                GTItemHelper.ShowActorQuality(tab.quality, id);
                tab.icon.spriteName = db.Icon;
            }
            else
            {
                GTItemHelper.ShowActorQuality(tab.quality, 0);
            }
        }
    }

    private void ShowPartnerArrayView()
    {
        XCharacter role = RoleModule.Instance.GetCurPlayer();
        for (int i=0;i<mPartnerArray.Length;i++)
        {
            PartnerItem tab = mPartnerArray[i];
            DActor db = ReadCfgActor.GetDataById(tab.id);
            GTItemHelper.ShowQualityText(tab.elem.name, db.Name, db.Quality);
            GTItemHelper.ShowActorQuality(tab.elem.quality, tab.id);
            tab.elem.icon.spriteName = db.Icon;
            XPartner data = DataDBSPartner.GetDataById(tab.id);
            tab.type1.text = data == null ? "1" : data.Level.ToString();
            tab.type2.text = data == null ? "0" : data.Star.ToString();
            tab.type3.text = data == null ? "0" : data.Advance.ToString();
            bool isBattle = tab.id == role.Partner1 || tab.id == role.Partner2;
            tab.btnBattle.SetActive(!isBattle);
        }
    }

    private void OnReturnClick(GameObject go)
    {
        GTAudioManager.Instance.PlayEffectAudio(GTAudioKey.SOUND_UI_CLICK);
        Hide();
    }

    private void OnRecvChangePartner(int arg1, int arg2)
    {
        ShowPartnerDressView();
        ShowPartnerArrayView();
    }
}