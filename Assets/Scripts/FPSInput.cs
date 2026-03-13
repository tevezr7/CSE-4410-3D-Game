using UnityEngine;
using UnityEngine.InputSystem;

public class FPSInput : MonoBehaviour
{
    private float originalSpeed;
    public float gravity = 9.8f;
    private CharacterController controller;
    private PlayerCharacter playerCharacter;
    private Vector2 moveInput;
    public const float baseSpeed = 5f;
    private float speedMultiplier = 1f;
    private float speed => baseSpeed * speedMultiplier * (isSprinting ? 1.5f : 1f);

    public float jumpForce = 5.0f;
    private float verticalVelocity = 0f;
    //added jumping and sprinting mechanics, with stamina drain for sprinting and regeneration when not sprinting. also added a boolean to track if the player is currently sprinting or moving, which can be used for animations in the future! :D

    public bool isSprinting = false;
    public bool isMoving = false;

    private void OnEnable()
    {
        GameEvents.OnSpeedChanged += OnSpeedChanged;
    }
    
    private void OnDisable()
    {
        GameEvents.OnSpeedChanged -= OnSpeedChanged;
    }

    private void OnSpeedChanged(float value)
    {
        speedMultiplier = value;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCharacter = GetComponent<PlayerCharacter>(); 
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(playerCharacter != null && playerCharacter.currentStamina <= 0)
            {
                Debug.Log("Not enough stamina to sprint!");
                return;
            }
            isSprinting = true;
        }
        else if (context.canceled)
        {
            isSprinting = false;

        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
        
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;  

        verticalVelocity -= gravity * Time.deltaTime;
        movement.y = verticalVelocity * Time.deltaTime;

        controller.Move(movement);
        if (isSprinting)
        {
            playerCharacter.StaminaDrain();

            if (playerCharacter.currentStamina <= 0) 
            {
                isSprinting = false;
                Debug.Log("Out of stamina!");
            }
        }
        if(playerCharacter.currentStamina >= playerCharacter.maxStamina)
        {
            playerCharacter.currentStamina = playerCharacter.maxStamina;
        }
        else if(!isSprinting && playerCharacter.currentStamina < playerCharacter.maxStamina)
        {
            playerCharacter.StaminaRegen();
        }
        isMoving = moveInput.magnitude > 0;
    }
}
