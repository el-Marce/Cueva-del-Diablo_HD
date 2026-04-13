using UnityEngine;
using System.Collections;
public class PlayerCameraEffects : MonoBehaviour
{
    Vector3 originalLocalPos;
    Coroutine currentRoutine;

    void Start()
    {
        originalLocalPos = transform.localPosition;
    }

    public void HitImpact(Vector3 hitDirection, float intensity, float duration)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(HitRoutine(hitDirection, intensity, duration));
    }

    IEnumerator HitRoutine(Vector3 hitDirection, float intensity, float duration)
    {
        float timer = 0f;

        Vector3 localDir = transform.InverseTransformDirection(hitDirection);
        localDir.y = 0f; 
        localDir.Normalize();

        Vector3 offset = localDir * intensity;

        float hitTime = duration * 0.3f;

        while (timer < hitTime)
        {
            float t = timer / hitTime;
            transform.localPosition = Vector3.Lerp(originalLocalPos, originalLocalPos + offset, t);

            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;
        float returnTime = duration * 0.7f;

        while (timer < returnTime)
        {
            float t = timer / returnTime;
            transform.localPosition = Vector3.Lerp(originalLocalPos + offset, originalLocalPos, t);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPos;
    }
}