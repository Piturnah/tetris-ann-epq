using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Clock : MonoBehaviour
{
    public static event Action clockTick;
    float _FRAME_RATE = 60.098813897441f;
    float timeMultiplier = 1;

    private void Start() {
        Time.timeScale = 1;
        StartCoroutine(ClockTicker());
    }

    IEnumerator ClockTicker() {
        while (true) {
            //yield return new WaitForSeconds((1f/20)/_FRAME_RATE);
            yield return new WaitForSeconds((1/timeMultiplier)/_FRAME_RATE);
            clockTick?.Invoke();
        }
    }
}
