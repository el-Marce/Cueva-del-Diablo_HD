using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text tmp;
    public float outlineThickness = 0.25f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tmp == null) return;
        tmp.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineThickness);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tmp == null) return;
        tmp.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f);
    }
        public IEnumerator BlinkOutline(int times, float speed)
    {
        for (int i = 0; i < times; i++)
        {
            tmp.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineThickness);
            yield return new WaitForSeconds(speed);
            tmp.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f);
            yield return new WaitForSeconds(speed);
        }
    }
}