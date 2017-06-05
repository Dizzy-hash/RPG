using UnityEngine;

public class DeadTime : MonoBehaviour
{
    public float deadTime = 0;

    void Awake()
    {
        Destroy(gameObject, deadTime);
    }
}