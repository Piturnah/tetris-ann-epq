using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Clock : MonoBehaviour
{
    public static event Action clockTick;
    float _FRAME_RATE = 60.098813897441f;
    float timeMultiplier = 1;

    float lastCheckTime = 0;

    private void Start() {
        Time.timeScale = 1;
        //StartCoroutine(ClockTicker());
    }

    private void Update() {
        if (Time.time >= lastCheckTime + (1 / timeMultiplier) / _FRAME_RATE) {
            lastCheckTime = Time.time;
            clockTick?.Invoke();
        }
    }

/*    IEnumerator ClockTicker() {
        while (true) {
            yield return new WaitForSeconds((1/timeMultiplier)/_FRAME_RATE);
            clockTick?.Invoke();
        }
    }*/
}
