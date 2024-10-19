using UnityEngine;

public class Gun : MonoBehaviour
{
    float mag = 75f;
    float spd = 5f;
    Vector3 rot = Vector3.zero;   
    Vector3 original; 

    public PlayerController player;

    void Start()
    {
        original = transform.localEulerAngles;
    }

    void Update()
    {
        rot = Vector3.Lerp(rot, Vector3.zero, spd * Time.deltaTime);
        transform.localEulerAngles = original + rot;
    }
    public void Recoil()
    {
        rot += new Vector3(-mag, 0, 0f);
    }

    public void Shoot()
    {
        Recoil();
        
        RaycastHit hit;
        Ray ray = player.cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out hit, player.range))
        {
            Debug.Log("Hit: " + hit.transform.name);
            if (hit.transform.CompareTag("NPC"))
            {
                NPC npc = hit.transform.GetComponent<NPC>();
                npc.Hit();
            }
        }
    }
}
