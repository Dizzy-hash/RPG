    using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GTCameraManager : GTMonoSingleton<GTCameraManager>
{
    private UIRoot    mRoot = null;
    private Transform mAnchor = null;

    public Camera           MainCamera { get; private set; }
    public Camera           NGUICamera { get; private set; }
    public CameraController CameraCtrl { get; private set; }

    public const int DEPTH_CAM_MAIN       = 0;
    public const int DEPTH_CAM_2DUICAMERA = 6;

    public override void SetDontDestroyOnLoad(Transform parent)
    {
        base.SetDontDestroyOnLoad(parent);
        this.CreateMainCamera();
        this.RevertMainCamera();
        this.AddRoot();
    }

    public Camera CreateCamera(string name)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        return go.AddComponent<Camera>();
    }

    public void FollowPlayer(Transform trans)
    {
        CameraCtrl = MainCamera.gameObject.GET<CameraController>();
        CameraCtrl.SetTarget(trans);
    }

    public void CreateMainCamera()
    {
        MainCamera = Camera.main;
        if (MainCamera == null)
        {
            GameObject c = new GameObject("MainCamera");
            MainCamera = c.AddComponent<Camera>();
            GTTools.SetTag(MainCamera.gameObject, GTTools.Tags.MainCamera);
            MainCamera.gameObject.GET<AudioListener>();
        }
        MainCamera.transform.parent = transform;
    }

    public void RevertMainCamera()
    {
        if (MainCamera == null)
        {
            return;
        }
        MainCamera.fieldOfView = 60;
        MainCamera.renderingPath = RenderingPath.Forward;
        MainCamera.depth = DEPTH_CAM_MAIN;
    }

    public void AddUI(GameObject go)
    {
        if (go == null)
        {
            return;
        }
        go.transform.parent = mAnchor;
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;
        NGUITools.SetLayer(go, mRoot.gameObject.layer);
    }

    void AddRoot()
    {
        if (UIRoot.list.Count > 0)
        {
            GameObject.Destroy(UIRoot.list[0].gameObject);
            UIRoot.list.Clear();
            UIPanel.list.Clear();
        }
        NGUITools.CreateUI(false);
        mRoot = UIRoot.list[0];
        NGUITools.SetLayer(mRoot.gameObject, GTLayer.LAYER_UI);

        mRoot.scalingStyle = UIRoot.Scaling.Flexible;
        mRoot.minimumHeight = 320;
        mRoot.maximumHeight = 4096;
        NGUICamera = UICamera.eventHandler.cachedCamera;
        NGUICamera.clearFlags = CameraClearFlags.Depth;
        NGUICamera.depth = DEPTH_CAM_2DUICAMERA;
        NGUICamera.nearClipPlane = -10;
        NGUICamera.farClipPlane = 1200;
        DontDestroyOnLoad(mRoot);

        List<Transform> childs = new List<Transform>();
        for (int i = 0; i < mRoot.transform.childCount; i++)
        {
            Transform cc = mRoot.transform.GetChild(i);
            if (cc != NGUICamera.transform)
            {
                childs.Add(cc);
            }
        }
        for (int i = 0; i < NGUICamera.transform.childCount; i++)
        {
            Transform cc = NGUICamera.transform.GetChild(i);
            childs.Add(cc);
        }
        while (childs.Count > 0)
        {
            Transform child = childs[0];
            childs.Remove(child.transform);
            GameObject.Destroy(child.gameObject);
        }

        mAnchor = mRoot.gameObject.AddChild().transform;
        mAnchor.gameObject.name = "Anchor";
        mAnchor.localPosition = Vector3.zero;
        mAnchor.localScale = Vector3.one;
        mAnchor.localRotation = Quaternion.identity;
    }
}