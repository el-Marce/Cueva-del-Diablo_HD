using UnityEngine;
using System.Collections;

public class EnteMuerte : MonoBehaviour
{
    public GameObject shockwaveEffect;
    public float riseDuration = 1.8f;
    public float riseHeight = 4f;

    void Start()
    {
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * riseHeight;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        while (elapsed < riseDuration)
        {
            float t = elapsed / riseDuration;

            transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));

            foreach (Renderer r in renderers)
                foreach (Material mat in r.materials)
                    if (mat.HasProperty("_BaseColor"))
                    {
                        Color c = mat.GetColor("_BaseColor");
                        c.a = Mathf.Lerp(1f, 0f, t);
                        mat.SetColor("_BaseColor", c);
                    }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (shockwaveEffect != null)
        {
            Renderer main = GetComponentInChildren<Renderer>();
            Vector3 spawnPos = main != null ? main.bounds.center : transform.position;
            Instantiate(shockwaveEffect, spawnPos, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}