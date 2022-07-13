using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : HexNoise
{
    public override float GetNoise(float x, float y)
    {
        return Mathf.PerlinNoise(x, y);
    }

    public float GetNoise3D(float x1, float y1)
    {
        float x = Mathf.Cos(Mathf.PI * x1);

        return Mathf.PerlinNoise(x, y1);

    }
}