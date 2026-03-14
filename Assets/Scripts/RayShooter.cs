using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class RayShooter : MonoBehaviour
{
    private Camera cam;
    public InputActionReference Fire;
    private Crosshair crosshair;
    private BaseGun activeGun;

    void Start()
    {
        cam = GetComponent<Camera>();
        crosshair = FindFirstObjectByType<Crosshair>();
        activeGun = FindFirstObjectByType<BaseGun>();
        Cursor.lockState = CursorLockMode.Locked; // comment this out if u want to always have cursor, we replacing with ESCAPE to open settings and pause game though
        Cursor.visible = false;
    }

    void Update()
    {
        if(Fire.action.WasPressedThisFrame() && !EventSystem.current.IsPointerOverGameObject()) //added check to prevent shooting, think it'll be removed soon
        {
            if (activeGun == null || !activeGun.CanShoot()) return;
            activeGun.Shoot();
            crosshair.OnShoot();

            Vector3 point = new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, 0);
            Ray ray = cam.ScreenPointToRay(point);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                GameObject hitObject = hit.transform.gameObject;
                ReactiveTarget target = hitObject.GetComponent<ReactiveTarget>();

                if(target != null)
                {
                    target.ReactToHit();
                    crosshair.ShowHitMarker(); //shows hit marker on crosshair when enemy is hit
                    GameEvents.EnemyHit(); //Messenger.Broadcast(GameEvent.Enemyhit) 
                }
                else //comment out when done debugging
                {
                    StartCoroutine(SphereIndicator(hit.point));
                }

            }
        }
    }

    private IEnumerator SphereIndicator(Vector3 pos) //comment out when done debugging
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = pos;
        yield return new WaitForSeconds(1);
        Destroy(sphere);
    }

    public void SetActiveGun(BaseGun gun)
    {
        activeGun = gun;
    }

}
