using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CharacterAvatar : ICharacterComponent
{
    private Transform                   mRoot;
    private Transform                   mShadow;
    private GameObject                  mWeapon1;
    private GameObject                  mWeapon2;
    private TrailRenderer               mWeaponTrail1;
    private TrailRenderer               mWeaponTrail2;
    private Material                    mWeaponMat1;
    private Material                    mWeaponMat2;
    private Shader                      mWeaponShader1;
    private Shader                      mWeaponShader2;
    private SkinnedMeshRenderer         mSkinRenderer;
    private Material                    mSkinMaterial;
    private Shader                      mSkinShader;
    private Animator                    mAnimator;

    public GameObject                   RootObj             { get; private set; }
    public Transform                    Hand1               { get; private set; }
    public Transform                    Hand2               { get; private set; }
    public Transform                    BodyTrans           { get; private set; }
    public Transform                    HeadTrans           { get; private set; }
    public Transform                    RidePoint           { get; private set; }
    public int[]                        EquipArray          { get; private set; }


    public CharacterAvatar(Transform root)
    {
        this.mRoot          = root;
    }

    public void StartupComponent()
    {
        this.RootObj        = mRoot.gameObject;
        this.RidePoint      = GTTools.GetBone(mRoot, "Bone026");
        this.Hand1          = GTTools.GetBone(mRoot, "Bip01 Prop1");
        this.Hand2          = GTTools.GetBone(mRoot, "Bip01 Prop2");
        this.BodyTrans      = GTTools.GetBone(mRoot, "BP_Spine");
        this.HeadTrans      = GTTools.GetBone(mRoot, "BP_Head");

        this.mAnimator      = RootObj.GetComponent<Animator>();
        this.mShadow        = mRoot.Find("Shadow");
        this.mSkinRenderer  = mRoot.GetComponentInChildren<SkinnedMeshRenderer>();
        this.mSkinMaterial  = this.mSkinRenderer == null ? null : this.mSkinRenderer.material;
        this.mSkinShader    = this.mSkinMaterial == null ? null : this.mSkinMaterial.shader;
        this.EquipArray     = new int[8];
    }

    public void ExecuteComponent()
    {

    }

    public void ReleaseComponent()
    {
        GTResourceManager.Instance.DestroyObj(mWeaponMat1);
        GTResourceManager.Instance.DestroyObj(mWeaponMat2);
        GTResourceManager.Instance.DestroyObj(mWeapon1);
        GTResourceManager.Instance.DestroyObj(mWeapon2);
    }

    public void SetWeaponActive(bool active)
    {
        if (mWeapon1 != null)
        {
            mWeapon1.SetActive(active);
        }
        if (mWeapon2 != null)
        {
            mWeapon2.SetActive(active);
        }
    }

    public void SetShadowActive(bool active)
    {
        if (mShadow != null)
        {
            mShadow.gameObject.SetActive(active);
        }
    }

    public void SetTransparentVertLitOn()
    {
        Shader newSkinShader = GTShader.ChangeShader(mSkinMaterial, "MyMobile/Monster/Transparent_VertLit");
        Shader newWeaponShader1 = GTShader.ChangeShader(mWeaponMat1, "MyMobile/Monster/Transparent_VertLit");
        Shader newWeaponShader2 = GTShader.ChangeShader(mWeaponMat2, "MyMobile/Monster/Transparent_VertLit");
        if (mSkinMaterial != null && newSkinShader != null)
        {
            mSkinMaterial.SetFloat("_AlphaCon", 0.2f);
        }
        if (mWeaponMat1 != null && newWeaponShader1 != null)
        {
            mWeaponMat1.SetFloat("_AlphaCon", 0.2f);
        }
        if (mWeaponMat2 != null && newWeaponShader2 != null)
        {
            mWeaponMat2.SetFloat("_AlphaCon", 0.2f);
        }
    }

    public void SetTransparentVertLitOff()
    {
        GTShader.ChangeShader(mSkinMaterial, mSkinShader);
        GTShader.ChangeShader(mWeaponMat1,   mWeaponShader1);
        GTShader.ChangeShader(mWeaponMat2,   mWeaponShader2);
    }

    public void ChangeAvatar(int pos, int id)
    {
        if (EquipArray[pos - 1] == id)
        {
            return;
        }
        switch (pos)
        {
            case 1:
                ChangeHelmet(id);
                break;
            case 2:
                ChangeNecklace(id);
                break;
            case 3:
                ChangeArmor(id);
                break;
            case 4:
                ChangeShoes(id);
                break;
            case 5:
                ChangeWrist(id);
                break;
            case 6:
                ChangeRing(id);
                break;
            case 7:
                ChangeTalisman(id);
                break;
            case 8:
                ChangeWeapon(id);
                break;
        }
        EquipArray[pos - 1] = id;
    }

    public void PlayAnim(string animName, Callback onFinish)
    {
        GTAction.Get(mAnimator).Play(animName, onFinish);
    }

    public Transform GetBindTransform(EBind bind)
    {
        switch(bind)
        {
            case EBind.Head:
                return this.HeadTrans;
            case EBind.Body:
                return this.BodyTrans;
            case EBind.Foot:
                return this.mRoot;
            case EBind.LHand:
                return this.Hand1;
            case EBind.RHand:
                return this.Hand2;
            default:
                return null;
        }
    }

    public Vector3   GetBindPosition(EBind bind)
    {
        switch (bind)
        {
            case EBind.Head:
                return this.HeadTrans == null ? this.mRoot.position + new Vector3(0, 2, 0) : this.HeadTrans.position;
            case EBind.Body:
                return this.BodyTrans == null ? this.mRoot.position + new Vector3(0, 1, 0) : this.BodyTrans.position;
            case EBind.Foot:
                return this.mRoot.position;
            case EBind.LHand:
                return this.Hand1 == null ? this.mRoot.position + new Vector3(0, 1, 0) : this.Hand1.position;
            case EBind.RHand:
                return this.Hand2 == null ? this.mRoot.position + new Vector3(0, 1, 0) : this.Hand2.position;
            default:
                return Vector3.zero;
        }
    }

    void ChangeHelmet(int id)
    {

    }

    void ChangeNecklace(int id)
    {

    }

    void ChangeArmor(int id)
    {

    }

    void ChangeShoes(int id)
    {

    }

    void ChangeWrist(int id)
    {

    }

    void ChangeRing(int id)
    {

    }

    void ChangeTalisman(int id)
    {

    }

    void ChangeWeapon(int id)
    {
        DItem itemDB = ReadCfgItem.GetDataById(id);
        GTResourceManager.Instance.DestroyObj(mWeaponMat1);
        GTResourceManager.Instance.DestroyObj(mWeaponMat2);
        GTResourceManager.Instance.DestroyObj(mWeapon1);
        GTResourceManager.Instance.DestroyObj(mWeapon2);   
        if (Hand1 != null && !string.IsNullOrEmpty(itemDB.Model_R))
        {
            mWeapon1 = GTResourceManager.Instance.Load<GameObject>(itemDB.Model_R, true);
            if (mWeapon1 != null)
            {
                NGUITools.SetLayer(mWeapon1, mRoot.gameObject.layer);
                GTTools.ResetLocalTransform(mWeapon1.transform, Hand1);
                MeshRenderer renderer = mWeapon1.GetComponent<MeshRenderer>();
                if(renderer==null)
                {
                    renderer = mWeapon1.GetComponentInChildren<MeshRenderer>();
                }
                mWeaponMat1 = renderer == null ? null : renderer.material;
                mWeaponShader1 = mWeaponMat1 == null ? null : mWeaponMat1.shader;
            }
        }
        if (Hand2 != null && !string.IsNullOrEmpty(itemDB.Model_L))
        {
            mWeapon2 = GTResourceManager.Instance.Load<GameObject>(itemDB.Model_L, true);
            if (mWeapon2 != null)
            {
                NGUITools.SetLayer(mWeapon2, mRoot.gameObject.layer);
                GTTools.ResetLocalTransform(mWeapon2.transform, Hand2);
                MeshRenderer renderer = mWeapon2.GetComponent<MeshRenderer>();
                if (renderer == null)
                {
                    renderer = mWeapon2.GetComponentInChildren<MeshRenderer>();
                }
                mWeaponMat2 = renderer == null ? null : renderer.material;
                mWeaponShader2 = mWeaponMat2== null ? null : mWeaponMat2.shader;
            }
        }
    }
}