using System;
using UnityEngine;

public class History
{
    public static int innovation = 0;
    public static int nodeInnovation = 0;

    public static int Innovate()
    {
        innovation++;
        return innovation;
    }

    public static int NodeInnovate() {
        nodeInnovation++;
        return nodeInnovation;
    }

    public static float Sigmoid(float x) {
        return 1f / (1f + Mathf.Exp(-4.9f*x));
        //return (float)Math.Tanh(x);
        //return (x > 2) ? 1 : ((x < -2) ? 0 : 0.5f);
    }
}
