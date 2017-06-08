using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;
    public float     followDistance = 10;
    public float     followHeight = 15;
    public float     followRotationDamping = 5;
    public float     followHeightDamping = 5;
    public Vector3   followOffset;
    public bool      isShake    { get; set; }
    public bool      isAutoMove { get; set; }

    private Vector3 touchPos;
    private Vector3 pointAngels;
    private Vector3 mouseOverPos;
    private Vector3 pointOverAngels;
    private Camera  cam;



    public void SetTarget(Transform trans)
    {
        followTarget = trans;
        followOffset = new Vector3(0, followHeight, -followDistance);
        FollowTarget();
    }

    public void DoShake(float duration, float strength, int vibrato, float randomness, bool fadeout)
    {
        isShake = true;
        cam.DOShakePosition(duration, strength, vibrato, randomness, fadeout);
    }

    public void StopShake()
    {
        isShake = false;
    }

    void Start()
    {
        cam = this.GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (followTarget == null)
        {
            return;
        }
        if(isShake)
        {
            return;
        }
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                OnTouchControl();
                break;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                OnNormalControl();
                break;
        }
    }

    void OnNormalControl()
    {
        if (Input.GetKey(KeyCode.A))
        {
            FollowRotate(-20);
            return;
        }
        if (Input.GetKey(KeyCode.D))
        {
            FollowRotate(20);
            return;
        }
        FollowTarget();
    }

    void OnTouchControl()
    {
        switch (Input.touchCount)
        {
            case 0:
                {
                    FollowTarget();
                }
                break;
            case 1:
                {
                    Touch th = Input.GetTouch(0);
                    if (UICamera.Raycast(th.position))
                    {
                        FollowTarget();
                    }
                    else
                    {
                        if (th.phase == TouchPhase.Moved && th.position.x > Screen.width * 0.30)
                        {
                            float hOffset = th.position.x - this.touchPos.x > 0 ? 20 : -20;
                            this.touchPos = th.position;
                            FollowRotate(hOffset);
                        }
                        else
                        {
                            FollowTarget();
                        }
                    }
                }
                break;
            default:
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch th = Input.GetTouch(i);
                    if (UICamera.Raycast(th.position))
                    {
                        continue;
                    }
                    else
                    {
                        if (th.phase == TouchPhase.Moved && th.position.x > Screen.width * 0.30)
                        {
                            float hOffset = th.position.x - this.touchPos.x > 0 ? 20 : -20;
                            this.touchPos = th.position;
                            FollowRotate(hOffset);
                        }
                    }
                }
                FollowTarget();
                break;
        }
    }

    void FollowTarget()
    {
        transform.position = followTarget.position + followOffset;
        transform.LookAt(followTarget.position);
    }

    void FollowRotate(float hOffset)
    {
        float wantedRotationAngle = transform.eulerAngles.y + hOffset;
        float wantedHeight = followTarget.position.y + followHeight;
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, followRotationDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, followHeightDamping * Time.deltaTime);
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        Vector3 newPos = followTarget.position;
        newPos -= currentRotation * Vector3.forward * followDistance;
        newPos = new Vector3(newPos.x, currentHeight, newPos.z);
        transform.position = newPos;
        followOffset = transform.position - followTarget.position;
        transform.LookAt(followTarget.position);
    }
}
