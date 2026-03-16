using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static BaseGun;

public class RayShooter : MonoBehaviour
{
    private Camera cam;
    public InputActionReference Fire;
    private Crosshair crosshair;
    private BaseGun activeGun;
    private Animator gunAnimator;
    private FPSInput fpsInput;
    private RecoilSystem recoilSystem;

    // Bullet trail pooling
    [SerializeField] private BulletTrail bulletTrailPrefab;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject bloodSplatterPrefab;
    [SerializeField] private int poolSize = 10;
    private BulletTrail[] trailPool;
    private int poolIndex = 0;
    private float nextFireTime = 0f;

    void Start()
    {
        cam = GetComponent<Camera>();
        crosshair = FindFirstObjectByType<Crosshair>();
        activeGun = FindFirstObjectByType<BaseGun>();
        gunAnimator = GetComponentInChildren<Animator>();
        fpsInput = FindFirstObjectByType<FPSInput>();
        recoilSystem = FindFirstObjectByType<RecoilSystem>();

        Cursor.lockState = CursorLockMode.Locked; // comment this out if u want to always have cursor, we replacing with ESCAPE to open settings and pause game though
        Cursor.visible = false;

        // Initialize bullet trail pool
        trailPool = new BulletTrail[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            trailPool[i] = Instantiate(bulletTrailPrefab);
            trailPool[i].gameObject.SetActive(false);
        }
    }


    void Update()
    {
        if (activeGun == null || !activeGun.CanShoot() || activeGun.isReloading) return;
        bool canFire = activeGun.fireMode == BaseGun.FireMode.Auto
        ? Fire.action.IsPressed() : Fire.action.WasPressedThisFrame();

        if (canFire && Time.time >= nextFireTime && !EventSystem.current.IsPointerOverGameObject())
        {

            nextFireTime = Time.time + GetFireDelay();
            Debug.Log($"Next fire time: {nextFireTime}, delay: {GetFireDelay()}");
            gunAnimator.SetTrigger("Fire");
            if (activeGun == null || !activeGun.CanShoot() || fpsInput.isQuickKnifing) return;
            activeGun.Shoot();
            recoilSystem.ApplyRecoil(activeGun.recoilStrength, activeGun.recoilHorizontal);
            crosshair.OnShoot();

            Vector3 point = new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, 0);
            Ray ray = cam.ScreenPointToRay(point);
            Vector3 shootDir = ray.direction;
            if (!fpsInput.isADS)
            {
                float spread = crosshair.CurrentSpread * 0.002f;
                shootDir += new Vector3(
                    Random.Range(-spread * 1.1f, spread * 1.1F),
                    Random.Range(-spread, spread),
                    0
                );
                shootDir.Normalize();
            }

            Ray spreadRay = new Ray(ray.origin, shootDir);
            if (activeGun.gunName == "Shotgun")
                FireShotgun(spreadRay);
            else
            {
                RaycastHit hit;
                int layerMask = ~LayerMask.GetMask("Gun", "Arms", "BulletTrail", "Player");
                if (Physics.Raycast(spreadRay, out hit, Mathf.Infinity, layerMask))
                {
                    GameObject hitObject = hit.transform.gameObject;
                    ReactiveTarget target = hitObject.GetComponentInParent<ReactiveTarget>();
                    if (target != null)
                    {
                        float mult = hitObject.CompareTag("EnemyHead") ? 2f : 1f;
                        target.ReactToHit(activeGun.damage * mult);
                        ZombieAI zombie = hitObject.GetComponentInParent<ZombieAI>();
                        if (zombie != null) zombie.TriggerAggro();
                        crosshair.ShowHitMarker(); //shows hit marker on crosshair when enemy is hit

                        GameObject blood = Instantiate(bloodSplatterPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(blood, 0.5f);
                    }
                }

                Vector3 endPoint = hit.collider != null ? hit.point : spreadRay.GetPoint(100f);
                BulletTrail trail = GetTrail();
                trail.SpawnTrail(muzzlePoint.position, endPoint);
            }
        }
    }

    private void FireShotgun(Ray ray)
    {
        int pelletCount = 8;
        float pelletSpread = 0.2f;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 pelletDir = ray.direction + new Vector3(
                Random.Range(-pelletSpread, pelletSpread),
                Random.Range(-pelletSpread, pelletSpread),
                0
            );
            pelletDir.Normalize();

            Ray pelletRay = new Ray(ray.origin, pelletDir);
            RaycastHit hit;
            int layerMask = ~LayerMask.GetMask("Gun", "Arms", "BulletTrail", "Player");

            if (Physics.Raycast(pelletRay, out hit, Mathf.Infinity, layerMask))
            {
                GameObject hitObject = hit.transform.gameObject;
                ReactiveTarget target = hitObject.GetComponentInParent<ReactiveTarget>();
                if (target != null)
                {
                    float mult = hitObject.CompareTag("EnemyHead") ? 2f : 1f;
                    target.ReactToHit((activeGun.damage) * mult);
                    ZombieAI zombie = hitObject.GetComponentInParent<ZombieAI>();
                    if (zombie != null) zombie.TriggerAggro();
                    crosshair.ShowHitMarker();
                    GameObject blood = Instantiate(bloodSplatterPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(blood, 0.5f);
                }

                Vector3 endPoint = hit.collider != null ? hit.point : pelletRay.GetPoint(100f);
                BulletTrail trail = GetTrail();
                trail.SpawnTrail(muzzlePoint.position, endPoint);
            }
        }
    }

    private BulletTrail GetTrail()
    {
        BulletTrail trail = trailPool[poolIndex];
        poolIndex = (poolIndex + 1) % poolSize;
        trail.gameObject.SetActive(false);
        return trail;
    }

    public void SetActiveGun(BaseGun gun)
    {
        activeGun = gun;
    }

    public void SetActiveAnimator(Animator animator)
    {
        gunAnimator = animator;
    }

    public void SetMuzzlePoint(Transform muzzle)
    {
        muzzlePoint = muzzle;
    }

    private float GetFireDelay()
    {
        if (activeGun.fireMode == FireMode.Auto)
            return 60f / activeGun.fireRate; // fireRate = RPM, e.g. 600 RPM = 0.1s between shots
        else
        {
            if (activeGun.fireRate == 0f) return 2f;   // slow single fire
            if (activeGun.fireRate == 1f) return 0.5f; // medium single fire
            return 0f;                                   // uncapped single fire
        }
    }

}
