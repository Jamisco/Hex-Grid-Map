using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System;
using System.Text;

public class DebugMenu
{
    private StringBuilder elapsedTime = new StringBuilder();

    private Stopwatch timer = new Stopwatch();

    [MenuItem("Debug/Print Global Position")]
    public static void PrintGlobalPosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // its a 2d map

        UnityEngine.Debug.Log("Mouse is at " + mouseWorldPos.ToString());
    }

    public void StartTimer()
    {
        timer.Reset();

        timer.Start();
    }

    public void Restart()
    {
        timer.Restart();
    }

    public void StopTimer(string name)
    {
        timer.Stop();

        elapsedTime.Clear();

        elapsedTime.AppendLine(String.Format(name + ": " + "{0:00}:{1:00}:{2:00}",
            timer.Elapsed.Minutes, timer.Elapsed.Seconds,
             timer.Elapsed.Milliseconds));

        UnityEngine.Debug.Log(elapsedTime);
    }

    public void LogTime(string name)
    {
       // elapsedTime.Clear();

        string elapsedTime = "";

        elapsedTime = (String.Format(name + ": " + "{0:00}:{1:00}:{2:00}",
            timer.Elapsed.Minutes, timer.Elapsed.Seconds,
             timer.Elapsed.Milliseconds));

        UnityEngine.Debug.Log(elapsedTime);
    }

}