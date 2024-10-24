using UnityEngine;

public class Gun : MonoBehaviour
{
    float mag = 75f;
    float spd = 5f;
    Vector3 rot = Vector3.zero;
    Vector3 originalRot; 
    Vector3 originalPos;

    public PlayerController player;

    public SoundManager soundManager;

    float bullets = 6f;

    public float reloadBackAmount = 0.2f; 
    public float reloadSpeed = 2f; 

    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
    }

    void Start()
    {
        originalRot = transform.localEulerAngles;
        originalPos = transform.localPosition; 
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

            if (Physics.Raycast(ray, out hit, player.range))
            {
                Debug.Log("Hit: " + hit.transform.name);
                if (hit.transform.CompareTag("NPC") || hit.transform.CompareTag("Civilian"))
                {
                    NPC npc = hit.transform.GetComponent<NPC>();
                    npc.Hit();
                }
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
}
