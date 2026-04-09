using UnityEngine;
using System;
using System.Collections.Generic;

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

    public void RegisterPulse()
    {
        if (solved || !PreviousConditionsMet() || !acceptingInput) return;

        float now = Time.time;

        if (lastPulseTime > 0 && now - lastPulseTime > resetInterval)
        {
            pulseTimes.Clear();
        }

        pulseTimes.Add(now);
        lastPulseTime = now;

        // Dispara pulso visual siempre que no se haya completado el patrón
        if (pulseTimes.Count <= pattern.Length + 1)
            OnPulseRegistered?.Invoke();

        // Evalúa solo cuando se completaron exactamente los pasos necesarios
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

        // Si hay más pulsos de los necesarios, resetea
        if (pulseTimes.Count > pattern.Length + 1)
        {
            OnPatternFailed?.Invoke();
            pulseTimes.Clear();
        }
    }

    //public void RegisterPulse()
    //{
    //    if (solved || !PreviousConditionsMet() || !acceptingInput) return;

    //    float now = Time.time;

    //    if (lastPulseTime > 0 && now - lastPulseTime > resetInterval)
    //    {
    //        if (pulseTimes.Count > 0)
    //            OnPatternFailed?.Invoke();
    //        pulseTimes.Clear();
    //    }

    //    pulseTimes.Add(now);
    //    lastPulseTime = now;

    //    if (pulseTimes.Count >= 2)
    //    {
    //        // Valida el último intervalo contra el paso correspondiente del patrón
    //        int step = pulseTimes.Count - 2; // índice del intervalo actual

    //        if (step < pattern.Length)
    //        {
    //            float expected = pattern[step] * beatDuration;
    //            float actual = pulseTimes[pulseTimes.Count - 1] - pulseTimes[pulseTimes.Count - 2];

    //            if (Mathf.Abs(actual - expected) > tolerance)
    //            {
    //                // Intervalo incorrecto — falla inmediatamente
    //                OnPatternFailed?.Invoke();
    //                pulseTimes.Clear();
    //                return;
    //            }
    //        }
    //    }

    //    if (CheckPattern())
    //    {
    //        solved = true;
    //        OnPatternSolved?.Invoke();
    //    }
    //    else
    //    {
    //        OnPulseRegistered?.Invoke(); // pulso correcto
    //    }
    //}

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
}