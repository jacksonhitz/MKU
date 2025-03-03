using System.Collections;
using UnityEngine;


public class Revolver : MonoBehaviour
{
   float mag = 75f;
   float spd = 5f;
   Vector3 rot = Vector3.zero;


   Vector3 originalRot;
   Vector3 originalPos;
   Vector3 stowedPos = new Vector3(-0.5f, -0.5f, 0f);
   Vector3 stowedRot = new Vector3(45f, 0f, 0f);


   PlayerController player;
   SoundManager soundManager;
   GameManager gameManager;


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


   private bool isStowing = false;
   private float switchSpeed = 5f;
   private bool canFire = true;
   float fireRate = .5f;


   //public ParticleSystem shotgunMuzzleFlash;
   public ParticleSystem revolverMuzzleFlash;
   public Light gunPointLight;


   void Awake()
   {
       soundManager = FindObjectOfType<SoundManager>();
       gameManager = FindObjectOfType<GameManager>();
       player = FindObjectOfType<PlayerController>();
   }


   void Start()
   {
        originalRot = transform.localEulerAngles;
        originalPos = transform.localPosition;
        
        if (gunPointLight == null)
        {
            gunPointLight = transform.Find("FirePoint/Particle System/Point Light").GetComponent<Light>();
        }
        
        if (gunPointLight) gunPointLight.enabled = false;
   }


   void Update()
   {
       rot = Vector3.Lerp(rot, Vector3.zero, spd * Time.deltaTime);
   }


   public void StowWeapon(bool stow)
   {
       isStowing = stow;
       StopAllCoroutines();
       if (stow) StartCoroutine(SmoothTransition(stowedPos, stowedRot));
       else StartCoroutine(SmoothTransition(originalPos, originalRot));
   }


   private IEnumerator SmoothTransition(Vector3 targetPos, Vector3 targetRot)
   {
       Vector3 startPos = transform.localPosition;
       Vector3 startRot = transform.localEulerAngles;
       float elapsedTime = 0f;
       while (elapsedTime < 1f)
       {
           transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime);
           transform.localEulerAngles = Vector3.Lerp(startRot, targetRot, elapsedTime);
           elapsedTime += Time.deltaTime * switchSpeed;
           yield return null;
       }
       transform.localPosition = targetPos;
       transform.localEulerAngles = targetRot;
   }


   public void Recoil()
   {
       rot += new Vector3(-mag, 0, 0f);
   }


   public void Use()
   {
       if (!canFire) return;
       if (bullets > 0)
           {
               StartCoroutine(FireCooldown());
               Recoil();
               soundManager.RevShot();
               revolverMuzzleFlash.Play();

               if (gunPointLight) gunPointLight.enabled = true;
               StartCoroutine(TurnOffLight(0.1f));

               bullets--;
               Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
               Hitscan(ray);
           }
           else soundManager.RevEmpty();
   }


    void Hitscan(Ray ray)
    {

        Debug.DrawRay(ray.origin, ray.direction * player.range, Color.red, 2f);
        
        if (Physics.Raycast(ray, out RaycastHit hit, player.range))
        {
            Debug.Log("Hit something: " + hit.transform.name + " with tag: " + hit.transform.tag);
            
            if (hit.transform.CompareTag("NPC")) 
            {
                Debug.Log("Attempting to call Hit() on " + hit.transform.name);
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Pass "gun" to indicate this is a revolver shot
                    enemy.Hit("gun");
                    Debug.Log("Successfully called Hit()");
                }
            }
            TrailRenderer tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
            StartCoroutine(HandleTracer(tracer, hit.point));
        }
        else
        {
            Debug.Log("Raycast didn't hit anything");
        }
    }

   IEnumerator FireCooldown()
   {
       canFire = false;
       yield return new WaitForSeconds(fireRate);
       canFire = true;
   }


   IEnumerator HandleTracer(TrailRenderer tracer, Vector3 hitPoint)
   {
       tracer.transform.position = firePoint.position;
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

   IEnumerator TurnOffLight(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gunPointLight) gunPointLight.enabled = false;
    }
}



