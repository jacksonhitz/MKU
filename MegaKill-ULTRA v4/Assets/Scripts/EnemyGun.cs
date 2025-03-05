    using UnityEngine;
    using System.Collections;

    public class EnemyGun : MonoBehaviour
    {
        public float reloadMag = 40f;
        public float reloadSpd = 2f;
        public float fireRate = 2f;
        public float bulletSpd = 50f;
        public float range = 1000f;

        public GameObject projectilePrefab;
        public Transform firePoint;
        public TrailRenderer tracerPrefab;

        private Vector3 rot = Vector3.zero;
        private SoundManager soundManager;
        private CamController cam;
        private Enemy enemy;
        private GameObject player;
        private bool isAttacking = false;
        private AudioSource sfx;
        public AudioClip gunshot;

        private Enemy enemyComponent;

        public Animator animator;
        public ParticleSystem muzzleFlash;

        void Start()
        {
            enemy = GetComponent<Enemy>();
            sfx = GetComponent<AudioSource>();
            player = GameObject.FindGameObjectWithTag("Player");
            cam = FindObjectOfType<CamController>();
            soundManager = FindObjectOfType<SoundManager>();

            fireRate = Random.Range(1f, 3f);
            enemyComponent = GetComponent<Enemy>();
        }

        void Update()
        {
            if (enemy.los && InRange() && !isAttacking && CanShootNow())
            {
                StartCoroutine(CallAttack());
            }
        }

        IEnumerator CallAttack()
        {
            isAttacking = true;
            yield return new WaitForSeconds(fireRate);
            
            if (CanShootNow())
            {
                Attack();
            }
            
            isAttacking = false;
        }

        public bool InRange()
        {
            return Vector3.Distance(transform.position, player.transform.position) <= range;
        }

        void Attack()
        {
            if (enemy.isDead || !CanShootNow())
            {
                return;
            }
        
            if (enemy.animator != null)
            {
                enemy.animator.SetTrigger("Shoot");
            }

            soundManager.EnemySFX(sfx, gunshot);

            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
                muzzleFlash.Emit(10);
            }
            else
            {
            }

            Collider playerCollider = player.GetComponent<Collider>();
            Vector3 targetPosition = player.transform.position;

            if (playerCollider != null)
            {
                targetPosition.y = playerCollider.bounds.max.y - 0.1f;
            }

            Vector3 targetDir = (targetPosition - firePoint.position).normalized;

            GameObject bulletObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            Rigidbody rb = bulletObj.GetComponent<Rigidbody>();

            rb.velocity = targetDir * bulletSpd;

            bullet.vel = bulletSpd;
            bullet.direction = targetDir;

            TrailRenderer tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
            StartCoroutine(HandleTracer(tracer, bulletObj));
        }

        IEnumerator HandleTracer(TrailRenderer tracer, GameObject bullet)
        {
            float startTime = Time.time;
            float maxDuration = 5f; // Maximum time to track the bullet
            
            while (bullet != null && Time.time - startTime < maxDuration)
            {
                tracer.transform.position = bullet.transform.position;
                yield return null;
            }

            yield return new WaitForSeconds(tracer.time);
            Destroy(tracer.gameObject);
        }

        bool CanShootNow()
        {
            if (enemyComponent != null && !enemyComponent.CanShoot)
            {
                return false;
            }
            return true;
        }
    }