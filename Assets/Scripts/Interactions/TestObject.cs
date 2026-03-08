using UnityEngine;

public class TestObject : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Objeto interactuado");
    }
}