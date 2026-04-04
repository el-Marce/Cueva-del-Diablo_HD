using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    float xRotation = 0f;

    void Update()
    {
        if (GameState.InMenu) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime / Screen.width * 1000f;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime / Screen.height * 1000f;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}