using UnityEngine;

public class RecoilSystem : MonoBehaviour
{
    private Vector3 currentRecoil;
    private Vector3 targetRecoil;
    private Vector3 recoilVelocity;
    private FPSInput fpsInput;

    [SerializeField] private float recoilSnappiness = 20f;
    [SerializeField] private float recoilReturnSpeed = 6f;
    void Start()
    {
        fpsInput = FindFirstObjectByType<FPSInput>();
    }

    public void ApplyRecoil(float vertical, float horizontal)
    {
        float mult = (fpsInput.isADS && fpsInput.isCrouching) ? 0.7f : (fpsInput.isADS ? 0.8f : (fpsInput.isCrouching ? 0.9f : 1f));

        recoilVelocity += new Vector3(-vertical * mult, Random.Range(-horizontal, horizontal) * mult, 0);
    }

    void Update()
    {
        targetRecoil += recoilVelocity * Time.deltaTime;
        recoilVelocity = Vector3.Lerp(recoilVelocity, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
        currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, recoilSnappiness * Time.deltaTime);

        transform.localEulerAngles = currentRecoil;
    }
}
