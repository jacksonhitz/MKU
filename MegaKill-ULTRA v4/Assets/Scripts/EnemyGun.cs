using UnityEngine;
using System.Collections;

public class EnemyGun : MonoBehaviour
{
    public float mag = 75f;
    public float spd = 5f;
    public float fireRate = .5f; 
    Vector3 rot = Vector3.zero;
    Vector3 originalRot; 
    Vector3 originalPos;

    public SoundManager soundManager;

    CamController cam;

    public float reloadBackAmount = 0.2f; 
    public float reloadSpeed = 2f; 
    public LayerMask hitMask; 

    public float tracerDuration = 0.2f; 
    void Start()
    {
        originalRot = transform.localEulerAngles;
        originalPos = transform.localPosition; 

        cam = FindObjectOfType<CamController>();
    }

    void Update()
    {
        rot = Vector3.Lerp(rot, Vector3.zero, spd * Time.deltaTime);
        transform.localEulerAngles = originalRot + rot;
    }

    public void Recoil()
    {
        rot += new Vector3(-mag, 0, 0f);
    }

    public void Shoot(Transform player)
    {
        
        Recoil();
        

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, directionToPlayer);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitMask))
        {
            Debug.Log("Hit: " + hit.transform.name);
            if (hit.transform.CompareTag("Player"))
            {
                PlayerController playerController = hit.transform.GetComponent<PlayerController>();
                if (!playerController.isDead)
                {
                    playerController.Hit();
                    cam.Damaged(transform.position);
                }
            }
            
        }
        CreateBulletTracer(ray.origin, hit.point);
    }

    void CreateBulletTracer(Vector3 startPosition, Vector3 endPosition)
    {
        GameObject tracer = new GameObject("BulletTracer");
        LineRenderer lineRenderer = tracer.AddComponent<LineRenderer>();

        lineRenderer.startWidth = 0.1f;  
        lineRenderer.endWidth = 0.1f;   
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); 
        lineRenderer.startColor = Color.white; 
        lineRenderer.endColor = Color.white;   
        lineRenderer.positionCount = 2; 

        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        StartCoroutine(FadeOutTracer(tracer, tracerDuration));
    }

    IEnumerator FadeOutTracer(GameObject tracer, float duration)
    {
        float startTime = Time.time;
        LineRenderer lineRenderer = tracer.GetComponent<LineRenderer>();

        Color startColor = lineRenderer.startColor;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            Color currentColor = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0), t);
            lineRenderer.startColor = currentColor;
            lineRenderer.endColor = currentColor;
            yield return null;
        }
        Destroy(tracer);
    }
}
