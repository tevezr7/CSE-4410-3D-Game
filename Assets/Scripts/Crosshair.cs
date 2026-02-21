using UnityEngine;


public class Crosshair : MonoBehaviour
{
    [SerializeField] Color crosshairColor = Color.white;
    [SerializeField] Color enemyColor = Color.red;
    [SerializeField] float crosshairSize = 10f;
    [SerializeField] float crosshairThickness = 2f;
    [SerializeField] float dynamicSpread = 50f;
    [SerializeField] float detectRange = 100f;
    [SerializeField] Camera cam;
    [SerializeField] float centerDotSize = 2f;
    [SerializeField] float baseGap = 5f;
    [SerializeField] float hitMarkerDuration = 0.5f;  
    [SerializeField] GameObject hitMarkerImage;

    private FPSInput input;
    private float currentSpread = 0f;
    private bool aimingAtEnemy = false;
    private float hitMarkerTimer = 0f;
    private Texture2D crosshairTex;

    void Start()
    {
        input = FindFirstObjectByType<FPSInput>();
        crosshairTex = new Texture2D(1, 1);
        crosshairTex.SetPixel(0, 0, Color.white);
        crosshairTex.Apply();
    }

    void Update()
    {
        if (hitMarkerTimer > 0f)
        {
            hitMarkerTimer -= Time.deltaTime;
            if (hitMarkerTimer <= 0f)
            {
                hitMarkerImage.SetActive(false);
            }
        }
        

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectRange))
            aimingAtEnemy = hit.collider.CompareTag("Enemy");
        else
            aimingAtEnemy = false;
    }

    public void ShowHitMarker()
    {
        hitMarkerTimer = hitMarkerDuration;
            if (hitMarkerImage != null)
            {
                hitMarkerImage.SetActive(true);
        }
    }

    private void OnGUI()
    {
        float targetSpread = input.isMoving ? dynamicSpread : 0f;
        currentSpread = Mathf.Lerp(currentSpread, targetSpread, Time.deltaTime * 10f);

        float cx = Screen.width / 2f;
        float cy = Screen.height / 2f;
        float half = crosshairThickness / 2f;
        float gap = baseGap + currentSpread;

        //lines 
        GUI.color = hitMarkerTimer > 0f ? enemyColor : crosshairColor; //swap with the dot one if you want the dot to be red on hit instead of the lines
        GUI.DrawTexture(new Rect(cx - half, cy - crosshairSize - gap, crosshairThickness, crosshairSize), crosshairTex); //top
        GUI.DrawTexture(new Rect(cx - half, cy + gap, crosshairThickness, crosshairSize), crosshairTex);                 //bottom
        GUI.DrawTexture(new Rect(cx - crosshairSize - gap, cy - half, crosshairSize, crosshairThickness), crosshairTex); //left
        GUI.DrawTexture(new Rect(cx + gap, cy - half, crosshairSize, crosshairThickness), crosshairTex);                 //right

        //dot 
        GUI.color = (aimingAtEnemy || hitMarkerTimer > 0f) ? enemyColor : crosshairColor;
        GUI.DrawTexture(new Rect(cx - centerDotSize / 2f, cy - centerDotSize / 2f, centerDotSize, centerDotSize), crosshairTex);

    }
    
}
//lengthy crosshair script, using gui.drawtexture and color to create dynamic crosshair that can be edited within the inspector. changes color of dot when hovering over enemy, when attack lands crosshair flashes red. hitmarker image is used for extra effect when hit lands although it didnt look as good as i thought, maybe it needs sound! feel free to ask if you want me to explain any part of it!