using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarCondition_RhythmChallenge : AltarCondition
{
    [Header("Patrón")]
    public float bpm = 90f;
    public float tolerance = 0.15f;
    public float resetInterval = 1.5f;
    public float[] pattern = { 1f, 1f, 0.5f, 0.5f, 1f };

    public event Action OnPulseRegistered;
    public event Action OnPatternFailed;
    public event Action OnPatternSolved;

    bool solved = false;
    float beatDuration;
    float lastPulseTime = 0f;
    List<float> pulseTimes = new List<float>();
    AltarRitual_Generic altar;
    bool onCooldown = false;

    public bool acceptingInput = false;

    void Start()
    {
        beatDuration = 60f / bpm;
        altar = GetComponent<AltarRitual_Generic>();
    }

    bool PreviousConditionsMet()
    {
        foreach (var c in altar.conditions)
        {
            if (c == this) break;
            if (!c.IsMet()) return false;
        }
        return true;
    }

    public override bool IsMet() => solved;

    public override string GetStatusText()
    {
        if (!PreviousConditionsMet())
            return "<color=#888888>Completa las condiciones anteriores primero</color>";
        if (solved)
            return "<color=#64DCA0>Ritmo completado </color>";
        return displayText;
    }

    public override void OnFulfill() { }

    //public void RegisterPulse()
    //{
    //    if (solved || !PreviousConditionsMet() || !acceptingInput || onCooldown) return;

    //    float now = Time.time;

    //    if (lastPulseTime > 0 && now - lastPulseTime > resetInterval)
    //    {
    //        if (pulseTimes.Count > 0)
    //            OnPatternFailed?.Invoke();
    //        pulseTimes.Clear();
    //    }

    //    pulseTimes.Add(now);
    //    lastPulseTime = now;

    //    if (pulseTimes.Count <= pattern.Length + 1)
    //        OnPulseRegistered?.Invoke();

    //    if (pulseTimes.Count == pattern.Length + 1)
    //    {
    //        if (CheckPattern())
    //        {
    //            solved = true;
    //            OnPatternSolved?.Invoke();
    //        }
    //        else
    //        {
    //            OnPatternFailed?.Invoke();
    //            pulseTimes.Clear();
    //        }
    //        return;
    //    }

    //    if (pulseTimes.Count > pattern.Length + 1)
    //    {
    //        OnPatternFailed?.Invoke();
    //        pulseTimes.Clear();
    //    }
    //}

    public void RegisterPulse()
    {
        if (solved || !PreviousConditionsMet() || !acceptingInput || onCooldown) return;

        pulseTimes.Add(Time.time);

        if (pulseTimes.Count <= pattern.Length + 1)
            OnPulseRegistered?.Invoke();

        if (pulseTimes.Count == pattern.Length + 1)
        {
            if (CheckPattern())
            {
                solved = true;
                OnPatternSolved?.Invoke();
            }
            else
            {
                OnPatternFailed?.Invoke();
                pulseTimes.Clear();
            }
            return;
        }

        if (pulseTimes.Count > pattern.Length + 1)
        {
            OnPatternFailed?.Invoke();
            pulseTimes.Clear();
        }
    }

    bool CheckPattern()
    {
        int required = pattern.Length + 1;
        if (pulseTimes.Count < required) return false;

        for (int start = 0; start <= pulseTimes.Count - required; start++)
        {
            bool match = true;
            for (int i = 0; i < pattern.Length; i++)
            {
                float expected = pattern[i] * beatDuration;
                float actual = pulseTimes[start + i + 1] - pulseTimes[start + i];
                if (Mathf.Abs(actual - expected) > tolerance)
                {
                    match = false;
                    break;
                }
            }
            if (match) return true;
        }
        return false;
    }

    public void StartCooldown(float duration)
    {
        StartCoroutine(CooldownRoutine(duration));
    }

    IEnumerator CooldownRoutine(float duration)
    {
        onCooldown = true;
        yield return new WaitForSeconds(duration);
        onCooldown = false;
        pulseTimes.Clear();
    }
}