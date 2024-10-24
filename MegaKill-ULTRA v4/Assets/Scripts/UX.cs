using UnityEngine;
using TMPro; 

public class UX : MonoBehaviour
{
    public Camera cam; 
    public Vector3 offset; 
    public float camDistance = 2f;

    public TextMeshProUGUI textMeshPro; 

    void Update()
    {
        UpdateCanvas();
    }

    void Start()
    {
        WASD();
    }

    void UpdateCanvas()
    {
        transform.position = cam.transform.position + cam.transform.forward * camDistance + offset;
        transform.rotation = Quaternion.LookRotation(cam.transform.forward);

        float scaleFactor = Scale(cam);
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    float Scale(Camera cam)
    {
        float fov = cam.fieldOfView;
        float aspect = cam.aspect;
        float height = 2.0f * Mathf.Tan((fov / 2) * Mathf.Deg2Rad) * camDistance; 
        float width = height * aspect; 

        float desiredWidth = 1920; 
        float desiredHeight = 1080; 

        float scale = Mathf.Min(width / desiredWidth, height / desiredHeight);

        return scale;
    }
    
    public void Off()
    {
        
    }
    public void WASD()
    {
        textMeshPro.text = "WASD TO MOVE"; 
    }

    public void Kill()
    {
        textMeshPro.text = "MOUSE TO KILL"; 
    }

    public void Reload()
    {
        textMeshPro.text = "R TO RELOAD";
    }

    public void Drive()
    {
        textMeshPro.text = "E TO DRIVE"; 
    }
    public void Bail()
    {
        textMeshPro.text = "Q TO BAIL"; 
    }
}
