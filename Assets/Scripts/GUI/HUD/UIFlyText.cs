using UnityEngine;
using System.Collections;
using TMPro;

public class UIFlyText : MonoBehaviour
{
    public AnimationCurve moveCurve;
    public AnimationCurve scaleCurve;
    public AnimationCurve alphaCurve;
    public TextMeshPro    textMesh;

    private float             mLifeTime  = 1;
    private float             mStartTime = 0;
    private Vector3           mStartPos  = Vector3.zero;
    private float             mStartFontSize = 1f;
    private bool              mIsPlaying = false;
    private float             mOffset    = 150;
    private float             mEnlarge   = 1;

    public string Text
    {
        get { return textMesh.text; }
        set { textMesh.text = value; }
    }

    public string Path
    {
        get; set;
    }

    public Color TextColor
    {
        get { return textMesh.color; }
        set { textMesh.color = value; }
    }

    public float TextAlpha
    {
        get { return textMesh.alpha; }
        set { textMesh.alpha = value; }
    }

    public float TextScale
    {
        get { return textMesh.fontSize; }
        set { textMesh.fontSize = value; }
    }

    public float TextEnlarge
    {
        get { return mEnlarge; }
        set { mEnlarge = value; }
    }


    public void Init(Vector3 pos)
    {
        mStartPos = pos;
        transform.localPosition = pos;
        mStartTime = Time.realtimeSinceStartup;
        Invoke("Release", mLifeTime);
        mIsPlaying = true;
    }

    void Awake()
    {
        mStartFontSize = TextScale;
    }

    void Update()
    {
        if (mIsPlaying == false)
        {
            return;
        }
        Keyframe[] offsets = moveCurve.keys;
        Keyframe[] alphas = alphaCurve.keys;
        Keyframe[] scales = scaleCurve.keys;
        float time = Time.realtimeSinceStartup;
        float offsetEnd = offsets[offsets.Length - 1].time;
        float alphaEnd = alphas[alphas.Length - 1].time;
        float scalesEnd = scales[scales.Length - 1].time;
        float totalEnd = Mathf.Max(scalesEnd, Mathf.Max(offsetEnd, alphaEnd));
        float currentTime = time - mStartTime;
        float o = moveCurve.Evaluate(currentTime);
        float a = alphaCurve.Evaluate(currentTime);
        float s = scaleCurve.Evaluate(currentTime);
        TextAlpha = a;
        TextScale = s * mStartFontSize * mEnlarge;
        transform.localPosition = mStartPos + new Vector3(0, o * mOffset, 0);
    }

    void Release()
    {
        GTPoolManager.Instance.ReleaseGo(Path, gameObject);
        mIsPlaying = false;
    }

    void OnDestroy()
    {
        CancelInvoke();
    }

}
