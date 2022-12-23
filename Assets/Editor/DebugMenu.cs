using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System;

public static class DebugMenu
{
    private static Stopwatch timer = new Stopwatch();
    public static void StartTimer()
    {
        timer.Reset();

        timer.Start();
    }

    public static void StopTimer()
    {
        string elapsedTime = "";

        timer.Stop();

        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            timer.Elapsed.Minutes, timer.Elapsed.Seconds,
             timer.Elapsed.Milliseconds / 10);


        UnityEngine.Debug.Log(elapsedTime);
    }
}