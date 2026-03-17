using UnityEngine;
using UnityEngine.InputSystem;

public class TowerPlacer : MonoBehaviour
{
    public GameObject turretPrefab;
    public GameObject minePrefab;
    public GameObject barbedPrefab;

    public Material validMaterial;
    public Material invalidMaterial;

    public float maxPlacementRange = 20f;

    private Camera cam;
    private GameObject ghostObject;
    private Renderer[] ghostRenderers;
    private bool isPlacing = false;
    private bool isValid = false;
    private bool lastValid = false;

    private enum TowerType { None, Turret, Mine, BarbedWire }
    private TowerType selectedType = TowerType.None;

    public int turretCount = 0;
    public int mineCount = 0;
    public int barbedCount = 0;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame) TrySelect(TowerType.Turret);
        if (Keyboard.current.zKey.wasPressedThisFrame) TrySelect(TowerType.Mine);
        if (Keyboard.current.xKey.wasPressedThisFrame) TrySelect(TowerType.BarbedWire);

        if(Keyboard.current.escapeKey.wasPressedThisFrame) CancelPlacement();

        if (!isPlacing) return;

        UpdateGhost();

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (isPlacing && scroll != 0)
            ghostObject.transform.Rotate(Vector3.up, scroll > 0 ? 45f : -45f);
        if (Mouse.current.leftButton.wasPressedThisFrame && isValid || Keyboard.current.fKey.wasPressedThisFrame)
        {
            PlaceTower();
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelPlacement();
        }
    }

    private void TrySelect(TowerType type)
    {
        if (GetCount(type) <= 0) return;
        if (isPlacing) CancelPlacement();

        selectedType = type;
        ghostObject = Instantiate(GetPrefab(type));

        foreach (Collider c in ghostObject.GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }

        ghostRenderers = ghostObject.GetComponentsInChildren<Renderer>();
        isPlacing = true;
    }

    private void UpdateGhost()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        int mask = ~LayerMask.GetMask("Player", "Gun", "Arms", "BulletTrail");

        if(Physics.Raycast(ray, out RaycastHit hit, maxPlacementRange, mask))
        {
            bool flatSurface = Vector3.Angle(hit.normal, Vector3.up) < 30f;
            bool spaceClear = !Physics.SphereCast(
                hit.point, 0.5f, Vector3.up, out RaycastHit _, 1f, mask);

            isValid = flatSurface && spaceClear;
            ghostObject.transform.position = hit.point;
            ghostObject.transform.up = hit.normal;

            if (isValid != lastValid)
            {
                foreach (Renderer r in ghostRenderers)
                {
                    r.material = isValid ? validMaterial : invalidMaterial;
                }
                lastValid = isValid;
            }
        }
        else
        {
            isValid = false;
        }
    }

    private void PlaceTower()
    {
        Instantiate(GetPrefab(selectedType), ghostObject.transform.position, ghostObject.transform.rotation);
        DecreaseCount(selectedType);
        CancelPlacement();
    }

    private void CancelPlacement()
    {
        Destroy(ghostObject);
        ghostObject = null;
        ghostRenderers = null;
        selectedType = TowerType.None;
        isPlacing = false;
        isValid = false;
        lastValid = false;
    }

    private GameObject GetPrefab(TowerType type) => type switch
    {
        TowerType.Turret => turretPrefab,
        TowerType.Mine => minePrefab,
        TowerType.BarbedWire => barbedPrefab,
        _ => null
    };

    private int GetCount(TowerType type) => type switch
    {
        TowerType.Turret => turretCount,
        TowerType.Mine => mineCount,
        TowerType.BarbedWire => barbedCount,
        _ => 0
    };

    private void DecreaseCount(TowerType type)
    {
        if (type == TowerType.Turret) turretCount--;
        else if (type == TowerType.Mine) mineCount--;
        else if (type == TowerType.BarbedWire) barbedCount--;
    }

    public void AddTower(string type)
    {
        if (type == "Turret") turretCount++;
        else if (type == "Mine") mineCount++;
        else if (type == "BarbedWire") barbedCount++;
    }
}
