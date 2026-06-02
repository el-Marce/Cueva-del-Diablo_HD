using UnityEngine;
using UnityEngine.UI;

public class AltarConditionIcon : MonoBehaviour
{
    public Image image;

    [Header("Sprites")]
    public Sprite inactiveSprite;
    public Sprite activeSprite;

    public void SetState(bool completed)
    {
        image.sprite = completed ? activeSprite : inactiveSprite;
    }
}