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

    public TrailRenderer tracerPrefab;  // Add this for tracer effect
    public Transform firePoint; // To use as the origin of the tracer

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

                // Spawn the tracer and make it follow the raycast
                TrailRenderer tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
                StartCoroutine(HandleTracer(tracer, hit.point));  
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

    IEnumerator HandleTracer(TrailRenderer tracer, Vector3 hitPoint)
    {
        tracer.transform.position = firePoint.position;
        
        Vector3 endPosition = hitPoint;

        tracer.transform.position = firePoint.position;
        tracer.transform.LookAt(endPosition);  

        float elapsedTime = 0f;
        while (elapsedTime < tracerDuration)
        {
            tracer.transform.position = Vector3.Lerp(firePoint.position, hitPoint, elapsedTime / tracerDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(tracer.time);

        Destroy(tracer.gameObject);
    }
}
