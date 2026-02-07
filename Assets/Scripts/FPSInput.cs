using UnityEngine;
using UnityEngine.InputSystem;

public class FPSInput : MonoBehaviour
{
    public float speed = 5.0f;
    public float gravity = 9.8f;
    private CharacterController controller;
    private Vector2 moveInput;
     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
        movement = transform.TransformDirection(movement);
        movement *= speed * Time.deltaTime;
        movement.y = -gravity * Time.deltaTime;
        controller.Move(movement);
    }
}
