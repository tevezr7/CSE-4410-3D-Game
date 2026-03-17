using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class FPSInput : MonoBehaviour
{
    public float gravity = 9.8f;
    private CharacterController controller;
    private PlayerCharacter playerCharacter;
    private Animator gunAnimator;
    private SettingsPopup settingsPopup;
    private Camera cam;
    private BaseGun activeGun;
    private WeaponSwitcher weaponSwitcher;
    private MeleeAttack knife;

    private Vector2 moveInput;
    public const float baseSpeed = 5f;
    private float speedMultiplier = 1f;
    private float speed => baseSpeed * speedMultiplier * (isSprinting ? 1.5f : 1f) * (isCrouching ? 0.5f : 1f) * (isADS ? 0.3f : 1f);

    public float jumpForce = 5.0f;
    private float verticalVelocity = 0f;
    private float targetLeanAngle = 0f;
    private float currentLeanAngle = 0f;
    private float targetLeanOffset = 0f;
    private float currentLeanOffset = 0f;
    private int leanDirection = 0;
    public float Fov = 90f;
    private float slideTimer = 0f;
    public float slideDuration = 0.5f;
    private Vector3 slideDirection;
    [SerializeField] private float slideSpeed = 8f;
    [SerializeField] private float slideStaminaCost = 15f;
    [SerializeField] private float knifeCooldown = 0.5f;
    private float lastKnifeTime = -Mathf.Infinity;

    public bool isSprinting = false;
    public bool isMoving = false;
    public bool isCrouching = false;
    public bool isJumping = false;
    public bool isADS = false;
    public bool isReloading = false;
    public bool isSliding = false;
    public bool isQuickKnifing = false;

    private int previousWeapon = 0;

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
        gunAnimator = GetComponentInChildren<Animator>();
        weaponSwitcher = FindFirstObjectByType<WeaponSwitcher>();
        knife = FindFirstObjectByType<MeleeAttack>();
        Debug.Log($"Gun animator found: {gunAnimator.gameObject.name}");

    }

    public void SetActiveGun(BaseGun gun)
    {
        activeGun = gun;
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

    public void OnReload(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isSprinting) return;
        if (activeGun == null) return;
        if (activeGun.currentAmmo == activeGun.magSize) return;
        if (activeGun.reserveAmmo <= 0) return;
        gunAnimator.ResetTrigger("Reload");
        gunAnimator.SetTrigger("Reload");

    }

    public void OnMelee(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isSprinting) return;
        if (isQuickKnifing) return;
        if (isSliding) return;
        if (activeGun != null && activeGun.isReloading) return;
        if (Time.time < lastKnifeTime + knifeCooldown) return;
        lastKnifeTime = Time.time;
        isQuickKnifing = true;
        StartCoroutine(QuickKnife());
    }

    private IEnumerator QuickKnife()
    {
        isQuickKnifing = true;
        previousWeapon = weaponSwitcher.currentWeapon;
        weaponSwitcher.SwitchTo(5);
        yield return new WaitForSeconds(0.2f);
        knife = weaponSwitcher.weapons[5].GetComponent<MeleeAttack>();
        if (knife != null) knife.Slash();
        yield return new WaitForSeconds(0.3f);
        if (gunAnimator != null)
        {
            gunAnimator.ResetTrigger("Fire");
            gunAnimator.ResetTrigger("Reload");
            gunAnimator.SetBool("isADS", false);
        }
        weaponSwitcher.SwitchTo(previousWeapon); 
        isQuickKnifing = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    public void OnADS(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isSliding) return;
            isADS = true;
            isSprinting = false; 
        }

        else if (context.canceled)
        {
            isADS = false;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (isSprinting && !isSliding)
        {
            StartSlide();
            return;
        }

        if (isSliding)
        {
            StopSlide();
            return;
        }

        Physics.SphereCast(transform.position, 0.5f, Vector3.up, out RaycastHit hit, 1f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);
        if (!isCrouching)
        {
            isCrouching = true;
            isSprinting = false; 
        }
        else
        if (hit.collider != null)
        {
            Debug.Log("Cannot stand up, something is above the player!");
        }
        else
        {
            isCrouching = false;
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        isCrouching = true;
        isSprinting = false;
        slideTimer = slideDuration;
        slideDirection = transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y)).normalized;
        playerCharacter.currentStamina -= slideStaminaCost;
    }

    private void StopSlide()
    {
        isSliding = false;
        isCrouching = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        
    }

    public void OnRightLean(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        leanDirection = leanDirection == 1 ? 0 : 1; // toggle off if already right
    }

    public void OnLeftLean(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        leanDirection = leanDirection == -1 ? 0 : -1; // toggle off if already left
    }


    // Update is called once per frame
    void Update()
    {
        //lean
        targetLeanAngle = leanDirection * -15f;
        targetLeanOffset = leanDirection * 0.5f;
        currentLeanOffset = Mathf.Lerp(currentLeanOffset, targetLeanOffset, Time.deltaTime * 10f); 
        cam.transform.localPosition = new Vector3(currentLeanOffset, cam.transform.localPosition.y, cam.transform.localPosition.z);
        currentLeanAngle = Mathf.LerpAngle(currentLeanAngle, targetLeanAngle, Time.deltaTime * 10f);
        cam.transform.localEulerAngles = new Vector3(
            cam.transform.localEulerAngles.x,
            cam.transform.localEulerAngles.y,
            currentLeanAngle
        );
        // end of lean

        //jumping
        isJumping = !controller.isGrounded;
        //end of jumping

        //crouching
        // replace with:
        float targetCamY = isCrouching ? 0.5f : 0.9f;
        cam.transform.localPosition = new Vector3(
            currentLeanOffset,
            Mathf.Lerp(cam.transform.localPosition.y, targetCamY, Time.deltaTime * 10f),
            cam.transform.localPosition.z
        );
        float targetHeight = isCrouching ? 1.3f : 2f;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * 10f);
        //end of crouching

        //ads
        float targetFov = isADS ? Fov * 0.9f : Fov;
        gunAnimator.SetBool("isADS", isADS);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * 10f);
        //end of ads   

        //movement anim
        if (gunAnimator != null && gunAnimator.isActiveAndEnabled)
        {
            gunAnimator.SetBool("isADS", isADS);
            gunAnimator.SetBool("isMoving", isMoving);
            gunAnimator.SetBool("isSprinting", isSprinting);
            gunAnimator.SetBool("isSliding", isSliding);
            Debug.Log($"isSliding: {isSliding}");

        }
        //end of movement anim

        //settings menu

        // if (Keyboard.current.escapeKey.wasPressedThisFrame)
        // {
            // if (settingsPopup.gameObject.activeSelf)
            // {
                // settingsPopup.Close();
                // Time.timeScale = 1f; //unpause game when closing settings
            // }
            // else
            // {
                // settingsPopup.Open();
                // Time.timeScale = 0f; //pause game when opening settings
            // }
        // }

        //end of settings menu

        //movement
        //sliding
        Vector3 movement;
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            float slideSpeedCurrent = Mathf.Lerp(slideSpeed, speed, 1 - (slideTimer / slideDuration));
            movement = slideDirection * slideSpeedCurrent * Time.deltaTime;

            if (slideTimer <= 0 || playerCharacter.currentStamina <= 0)
            {
                isSliding = false;
                isCrouching = true;
            }
        }
        else
        {
            movement = new Vector3(moveInput.x, 0, moveInput.y);
            movement = transform.TransformDirection(movement);
            movement *= speed * Time.deltaTime;
        }

        //end of sliding

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

    public void SetActiveAnimator(Animator animator)
    {
        gunAnimator = animator;
    }
}
