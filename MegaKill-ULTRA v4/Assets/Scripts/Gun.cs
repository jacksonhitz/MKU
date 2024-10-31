using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    float mag = 75f;
    float spd = 5f;
    Vector3 rot = Vector3.zero;
    Vector3 originalRot; 
    Vector3 originalPos;

    public PlayerController player;

    EnemyPatrol npc;
    public SoundManager soundManager;

    float bullets = 6f;

    public float reloadBackAmount = 0.2f; 
    public float reloadSpeed = 2f; 

    public float tracerDuration = 0.2f;

    public float scaredRad = 50f; 

    public LayerMask npcMask;

    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
    }

    void Start()
    {
        originalRot = transform.localEulerAngles;
        originalPos = transform.localPosition; 
        npc = FindObjectOfType<EnemyPatrol>();
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

    public void Shoot()
    {
        if (bullets >= 1)
        {
            Recoil();
            soundManager.Gunshot();

            RaycastHit hit;
            Ray ray = player.cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, scaredRad, npcMask);
            foreach (var collider in hitColliders)
            {
                if (collider.CompareTag("Civilian"))
                 {
                    Civilian civilian = collider.transform.parent.GetComponent<Civilian>();
                    if (civilian != null)
                    {
                        civilian.Scared(); 
                        npc.Flee();
                    }
                }
            }

            if (Physics.Raycast(ray, out hit, player.range))
            {
                Debug.Log("Hit: " + hit.transform.name);
                if (hit.transform.CompareTag("NPC") || hit.transform.CompareTag("Civilian"))
                {
                    NPC npc = hit.transform.GetComponent<NPC>();
                    npc.Hit();
                    soundManager.Squelch();
                }

                

                CreateBulletTracer(ray.origin, hit.point);
            }

            bullets--; 
        }
        else
        {
            soundManager.Empty();
        }
    }

    public void Reload()
    {
        soundManager.Reload();
        rot += new Vector3(mag, 0, 0f);
        bullets = 6f;
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
