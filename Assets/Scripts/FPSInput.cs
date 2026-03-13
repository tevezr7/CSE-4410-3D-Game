using UnityEngine;
using UnityEngine.InputSystem;

public class FPSInput : MonoBehaviour
{
    public float gravity = 9.8f;
    private CharacterController controller;
    private PlayerCharacter playerCharacter;
    private SettingsPopup settingsPopup;
    private Camera cam;
    private Vector2 moveInput;
    public const float baseSpeed = 5f;
    private float speedMultiplier = 1f;
    private float speed => baseSpeed * speedMultiplier * (isSprinting ? 1.5f : 1f) * (isCrouching ? 0.5f : 1f);

    public float jumpForce = 5.0f;
    private float verticalVelocity = 0f;
    private float targetLeanAngle = 0f;
    private float currentLeanAngle = 0f;
    //added jumping and sprinting mechanics, with stamina drain for sprinting and regeneration when not sprinting. also added a boolean to track if the player is currently sprinting or moving, which can be used for animations in the future! :D

    public bool isSprinting = false;
    public bool isMoving = false;
    public bool isCrouching = false;
    public bool isJumping = false;

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
        settingsPopup = FindFirstObjectByType<SettingsPopup>();
        cam = GetComponentInChildren<Camera>();
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

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Physics.SphereCast(transform.position, 0.5f, Vector3.up, out RaycastHit hit, 1f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);
        if (!isCrouching)
        {
            isCrouching = true;
            gameObject.transform.localScale = new Vector3(1, 0.65f, 1);
        }
        else
        if (hit.collider != null)
        {
            Debug.Log("Cannot stand up, something is above the player!");
            gameObject.transform.localScale = new Vector3(1, 0.65f, 1); //keep crouched if something is above
        }
        else
        {
            isCrouching = false;
            gameObject.transform.localScale = Vector3.one; //stand up if nothing is above
        }
    }

    //public void OnSlide(InputAction.CallbackContext context)
    //{
    //    
    //}

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        
    }

    public void OnRightLean(InputAction.CallbackContext context)
    {
        if (context.performed) targetLeanAngle = -15f;
        else if (context.canceled) targetLeanAngle = 0f;
    }

    public void OnLeftLean(InputAction.CallbackContext context)
    {
        if (context.performed) targetLeanAngle = 15f;
        else if (context.canceled) targetLeanAngle = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        currentLeanAngle = Mathf.LerpAngle(currentLeanAngle, targetLeanAngle, Time.deltaTime * 10f);
        cam.transform.localEulerAngles = new Vector3(
            cam.transform.localEulerAngles.x,
            cam.transform.localEulerAngles.y,
            currentLeanAngle
        );
        isJumping = !controller.isGrounded;
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingsPopup.gameObject.activeSelf)
            {
                settingsPopup.Close();
                Time.timeScale = 1f; //unpause game when closing settings
            }
            else
            {
                settingsPopup.Open();
                Time.timeScale = 0f; //pause game when opening settings
            }
        }
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
