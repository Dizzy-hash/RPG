using UnityEngine;
using System.Collections;

public class FlywordData
{
    public string        value;
    public Vector3       pos;
    public EFlyWordType  type;

    public FlywordData(string value, Vector3 pos, EFlyWordType type)
    {
        this.value = value;
        this.pos   = pos;
        this.type  = type;
    }
}