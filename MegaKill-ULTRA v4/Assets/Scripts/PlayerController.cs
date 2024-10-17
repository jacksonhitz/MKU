using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float horzInput;
    float vertInput;
    public float runSpd;
    public CharacterController controller; 
    Rigidbody rb;

    public bool isDriving;
    public Transform carPos;

    public Camera cam;
    public float range; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    void Update()
    {
        if (!isDriving)
        {
            horzInput = Input.GetAxisRaw("Horizontal");
            vertInput = Input.GetAxisRaw("Vertical");

            Vector3 move = transform.right * horzInput + transform.forward * vertInput;

            controller.Move(move * runSpd * Time.deltaTime);

        }
        else
        {
            transform.position = carPos.position;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
        }

    }
}