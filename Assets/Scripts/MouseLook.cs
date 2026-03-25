using UnityEngine;
using UnityEngine.InputSystem;


public class MouseLook : MonoBehaviour
{
    // SEB ADDED. DELETE IF NEEDED (Added to make FPSInput Public)
    public FPSInput fpsInput;
    // SEB END

    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;
    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;
    private float verticalRot = 0;

    private static Vector2 lookInput;

    public enum RotationAxes
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void Start()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
        {
            body.freezeRotation = true;
        }
        fpsInput = GetComponentInParent<FPSInput>();

    }
    void Update()
    {
        // SEB ADDED CAN DELETE IF NEEDED
        if (Time.timeScale == 0f) return;
        // SEB END

        if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, sensitivityHor * lookInput.x, 0);
        }
        else if (axes == RotationAxes.MouseY)
        {
            verticalRot -= sensitivityVert * lookInput.y;
            verticalRot = Mathf.Clamp(verticalRot, minimumVert, maximumVert);
            float horizontalRot = 0;
            // SEB ADDED. CAN DELETE IF NEEDED (0 at the end was changed to fpsInput != null? fpsInput.currentLeanAngle : 0)
            transform.localEulerAngles = new Vector3(verticalRot, horizontalRot, fpsInput != null ? fpsInput.currentLeanAngle : 0);
            // SEB END
        }
        else
        {
            verticalRot -= sensitivityVert * lookInput.y;
            verticalRot = Mathf.Clamp(verticalRot, minimumVert, maximumVert);
            float delta = lookInput.x * sensitivityHor;
            float horizontalRot = transform.localEulerAngles.y + delta;
            transform.localEulerAngles = new Vector3(verticalRot, horizontalRot, 0);
        }
    }
}
