using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PantallaInicio : MonoBehaviour
{
    public TMP_Text promptText; // "Presiona cualquier tecla"
    public float blinkInterval = 0.8f;

    void Start()
    {
        StartCoroutine(BlinkText());
    }

    void Update()
    {
        if (Input.anyKeyDown)
            SceneManager.LoadScene("MenuPrincipal");
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            promptText.enabled = !promptText.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}