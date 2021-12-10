using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Timer : MonoBehaviour
{
    public Action timerCallback;
    public TimerPurpose purpose;
    public float timer;
    public int secs;

    public void SetTimer(float timer, Action timerCallback)
    {
        this.timer = timer;
        this.timerCallback = timerCallback;
    }

    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            Mathf.Clamp(timer, 0, timer);
            if (timerCallback != null && IsTimerComplete())
            {
                timerCallback();
                //Destroy(this);
            }
        }
        else if (timerCallback != null && timer == 0f)
        {
            timerCallback();
            timer -= 0.5f;
        }
        secs = GetSecondsLeft();
    }
    public bool IsTimerComplete()
    {
        return timer <= 0f;
    }

    public int GetSecondsLeft()
    {
        return Mathf.CeilToInt(timer);
    }
}

public enum TimerPurpose { cooldown, duration, active}

/*
#if UNITY_EDITOR

[CustomEditor(typeof(Timer))]
public class TimerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Timer.timerCallback)));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Timer.timer)));
        serializedObject.ApplyModifiedProperties();
    }
}

#endif
*/