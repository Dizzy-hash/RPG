using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EJoystick : MonoBehaviour
{
    public delegate void JoystickEventHandler(EJoystick joystick);
    public event JoystickEventHandler On_JoystickMove;
    public event JoystickEventHandler On_JoystickMoveEnd;
    public event JoystickEventHandler On_JoystickTouchClick;
    public event JoystickEventHandler On_JoystickTouchDrag;
    public event JoystickEventHandler On_JoystickTouchDragEnd;

    private int mRadius = 100;
    private float mMinAlpha = 0.3f;
    private Vector3 mOriPos = Vector3.zero;
    private bool mIsTouch = false;

    public Vector2 joystickAxis = Vector2.zero;


    private UIWidget root;
    private UISprite area;
    private UISprite touch;

    void Awake()
    {
        root = this.GetComponent<UIWidget>();
        area = transform.Find("Area").GetComponent<UISprite>();
        touch = transform.Find("Touch").GetComponent<UISprite>();
        Init();
        UIEventListener.Get(touch.gameObject).onDrag = OnTouchDrag;
        UIEventListener.Get(touch.gameObject).onDragEnd = OnTouchDragEnd;
        UIEventListener.Get(touch.gameObject).onClick = OnTouchClick;
    }


    void Init()
    {
        area.transform.localPosition = Vector3.zero;
        touch.transform.localPosition = Vector3.zero;
        mOriPos = touch.transform.localPosition;
        Lighting(mMinAlpha);
    }

    void Update()
    {
        if (touch == null)
        {
            return;
        }
        if (Vector3.Magnitude(touch.transform.localPosition - mOriPos) > 0.01f)
        {
            Lighting(1f);
            Vector3 offset = touch.transform.localPosition - mOriPos;
            if (offset.magnitude > mRadius)
            {
                offset = offset.normalized * mRadius;
            }
            joystickAxis = new Vector2(offset.x / mRadius, offset.y / mRadius);
            if(mIsTouch)
            {

                if (On_JoystickTouchDrag != null)
                {
                    On_JoystickTouchDrag(this);
                }
            }
            else
            {
                if (On_JoystickMove != null)
                {
                    On_JoystickMove(this);
                }
            }
        }
    }

    void OnPress(bool isPressed)
    {
        if (mIsTouch)
        {
            return;
        }
        if (isPressed)
        {
            Lighting(1f);
            CalculateJoystickAxis();
        }
        else
        {
            CalculateJoystickAxis();
            if (On_JoystickMoveEnd != null)
            {
                On_JoystickMoveEnd(this);
            }
            touch.transform.localPosition = Vector3.zero;
            FadeOut(1f, mMinAlpha);
            if (On_JoystickMoveEnd != null)
            {
                On_JoystickMoveEnd(this);
            }
        }
    }

    void OnDrag(Vector2 delta)
    {
        if(mIsTouch)
        {
            return;
        }
        Lighting(1f);
        CalculateJoystickAxis();
    }

    void OnTouchDrag(GameObject go, Vector2 delta)
    {
        mIsTouch = true;
        Lighting(1f);
        CalculateJoystickAxis();
    }

    void OnTouchDragEnd(GameObject go)
    {
        touch.transform.localPosition = Vector3.zero;
        FadeOut(1f, mMinAlpha);
        if (On_JoystickTouchDragEnd != null)
        {
            On_JoystickTouchDragEnd(this);
        }
        mIsTouch = false;
    }

    void OnTouchClick(GameObject go)
    {
        if (On_JoystickTouchClick != null)
        {
            On_JoystickTouchClick(this);
        }
    }

    void CalculateJoystickAxis()
    {
        Vector3 offset = ScreenPos_to_NGUIPos(UICamera.currentTouch.pos);
        offset -= transform.localPosition;
        CalculateJoystickAxis(offset);
    }

    void CalculateJoystickAxis(Vector3 offset)
    {
        if (offset.magnitude > mRadius)
        {
            offset = offset.normalized * mRadius;
        }
        touch.transform.localPosition = offset;
        joystickAxis = new Vector2(offset.x / mRadius, offset.y / mRadius);
    }

    public float Axis2Angle(bool inDegree = true)
    {
        float angle = Mathf.Atan2(joystickAxis.x, joystickAxis.y);
        if (inDegree)
        {
            return angle * Mathf.Rad2Deg;
        }
        else
        {
            return angle;
        }
    }

    public float Axis2Angle(Vector2 axis, bool inDegree = true)
    {
        float angle = Mathf.Atan2(axis.x, axis.y);

        if (inDegree)
        {
            return angle * Mathf.Rad2Deg;
        }
        else
        {
            return angle;
        }
    }

    Vector3 ScreenPos_to_NGUIPos(Vector3 screenPos)
    {
        Vector3 uiPos = UICamera.currentCamera.ScreenToWorldPoint(screenPos);
        uiPos = UICamera.currentCamera.transform.InverseTransformPoint(uiPos);
        return uiPos;
    }

    Vector3 ScreenPos_to_NGUIPos(Vector2 screenPos)
    {
        return ScreenPos_to_NGUIPos(new Vector3(screenPos.x, screenPos.y, 0f));
    }

    void Lighting(float alpha)
    {
        root.alpha = alpha;
    }

    void FadeOut(float fromAlpha, float toAlpha)
    {
        TweenAlpha.Begin(gameObject, 0.2f, 0.3f);
    }

    void OnFadeOutTween(float value)
    {
        root.alpha = value;
    }
}