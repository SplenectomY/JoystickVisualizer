using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FrameRateLimiter 
{
    [RuntimeInitializeOnLoadMethod]
    private static void OnAppStart()
    {
        Application.targetFrameRate = 60;
    }
}
