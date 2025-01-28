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

    public float bullets = 6f;
    public float shells = 2f;

    public float reloadBackAmount = 0.2f; 
    public float reloadSpeed = 2f; 

    public float tracerDuration = 0.2f;

    public float scaredRad = 50f; 

    public LayerMask npcMask;

    public TrailRenderer tracerPrefab;  
    public Transform firePoint; 
    public int pellets = 12; 
    public float spreadAngle = 5f; 


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
        if (player.weapon == 2)
        {
            rot += new Vector3(mag, 0, 0f);
        }
        else
        {
            rot += new Vector3(-mag, 0, 0f);
        }
    }

    public void Revolver()
    {
        Recoil();
        soundManager.Gunshot();

        RaycastHit hit;
        Ray ray = player.cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out hit, player.range))
        {
            Debug.Log("Hit: " + hit.transform.name);
            if (hit.transform.CompareTag("NPC") || hit.transform.CompareTag("Civilian"))
            {
                NPC npc = hit.transform.GetComponent<NPC>();
                npc.Hit();
                soundManager.Squelch();
            }
            TrailRenderer tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
            StartCoroutine(HandleTracer(tracer, hit.point));  
        }
        bullets--; 
    }
    public void Shotgun()
    {
        Recoil();
        soundManager.Gunshot();

        for (int i = 0; i < pellets; i++)
        {
            Vector3 spread = new Vector3( Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f);

            Quaternion rotation = Quaternion.Euler(player.cam.transform.eulerAngles + spread);
            Ray ray = new Ray(firePoint.position, rotation * Vector3.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, player.range))
            {
                Debug.Log("Hit: " + hit.transform.name);
                if (hit.transform.CompareTag("NPC") || hit.transform.CompareTag("Civilian"))
                {
                    NPC npc = hit.transform.GetComponent<NPC>();
                    npc.Hit();
                    soundManager.Squelch();
                }
                TrailRenderer tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
                StartCoroutine(HandleTracer(tracer, hit.point));
            }
        }
        shells -= 1;
    }

    public void Reload()
    {
        soundManager.Reload();
        rot += new Vector3(mag, 0, 0f);

        if (player.weapon == 2)
        {
            shells = 2f;
        }
        else if (player.weapon == 1)
        {
            bullets = 6f;
        }
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
