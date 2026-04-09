//using UnityEngine;

//// Esqueleto listo para implementar cuando lo necesites
//public class AltarCondition_RhythmChallenge : AltarCondition
//{
//    [TextArea] public string riddleText = "El ritmo de las campanas abre los caminos";
//    bool solved = false;

//    public override bool IsMet() => solved;

//    public override string GetStatusText() => riddleText;

//    public override void OnFulfill() { }

//    // Llama esto desde tu sistema de ritmo cuando el jugador complete el patrón
//    public void SolveChallenge()
//    {
//        solved = true;
//        Debug.Log("[Reto] Completado: " + riddleText);
//    }
//}

using UnityEngine;
using System.Collections.Generic;

public class AltarCondition_RhythmChallenge : AltarCondition
{
    [TextArea] public string riddleText = "Sigue el ritmo del más allá";

    public float bpm = 90f;
    public float tolerance = 0.15f;
    public float maxInterval = 1f;

    private List<float> pulseTimes = new List<float>();
    private float beatDuration;
    private float lastPulseTime = 0f;

    private float[] pattern = { 1f, 1f, 0.5f, 0.5f, 1f };

    bool solved = false;

    void Start()
    {
        beatDuration = 60f / bpm;
    }

    public override bool IsMet() => solved;

    public override string GetStatusText()
    {
        return solved ? "Ritual completado" : riddleText;
    }

    public override void OnFulfill() { }

    public void RegisterPulse()
    {
        if (solved) return;

        float now = Time.time;

        if (lastPulseTime > 0 && now - lastPulseTime > maxInterval)
            pulseTimes.Clear();

        pulseTimes.Add(now);
        lastPulseTime = now;

        if (CheckPattern())
        {
            solved = true;
            Debug.Log("[Ritmo] Patrón correcto");
        }
    }

    bool CheckPattern()
    {
        for (int start = 0; start <= pulseTimes.Count - (pattern.Length + 1); start++)
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