using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public float sensX = 100f; 
    public float sensY = 100f; 

    public Transform player;

    float xRotation; 
    float yRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        // Apply rotations
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        player.rotation = Quaternion.Euler(0f, yRotation, 0f);

        transform.position = player.position + new Vector3(0, 1.5f, 0);
    }
}
