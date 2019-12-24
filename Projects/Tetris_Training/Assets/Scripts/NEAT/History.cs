using System;
using UnityEngine;

public class History
{
    public static int innovation = 0;

    public static int Innovate()
    {
        innovation++;
        return innovation;
    }

    public static float Sigmoid(float x) {
        return 1f / (1 + Mathf.Exp(-x));
    }
}
