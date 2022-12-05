using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //Grabbed References
    private Transform Feet;
    private Transform CameraRot;
    private Rigidbody Hitbox;
    private Canvas PlayerUI;
    private RectTransform HPBarFillRect;
    private RectTransform DashBarFillRect;
    private RectTransform ShieldMask;
    private TextMeshProUGUI ShieldText;
    private TextMeshProUGUI AmmoText;
    private Image HPBarFill;
    private Image DashBarFill;
    private Image RedKey;
    private Image GreenKey;
    private Image BlueKey;
    private LayerMask GroundLayer;
    public PlayerHP playerHP;
    public WeaponSwitching weaponSwitching;

    //Assigned References
    public AudioClip audioClipDash;
    public GameObject prefabBullet;

    //Player
    private const float ConstSpeed = 10f;
    private const float ConstJumpHeight = 3f;
    private const float ConstDashDistance = 8f;
    private const int ConstMaxJumps = 2;
    private int jumpCount = ConstMaxJumps;
    private bool isTouchingGroundLastFrame = false;
    private bool isTouchingGroundThisFrame = false;
    private bool isShooting = false;
    private EnforcersControls controls;
    private Vector2 movement;
    private Vector2 mousePos;

    //Rotation
    private Quaternion zeroQuat = Quaternion.Euler(0f, 0f, 0f);
    private float QuakeSourceSens;
    private float DegreesPerPixel;
    private float lastXCoord;
    private float lastYCoord;
    private float lastRotYaw;
    private float lastRotPitch;

    //UI Consts
    private readonly Color CooldownGray = new Color(172f / 255f, 164f / 255f, 104f / 255f);
    private readonly Color ChargedYellow = new Color(255f / 255f, 227f / 255f, 0f / 255f);

    //DashVariables
    [Header("DashVariables")]
    public float dashForce;
    public float dashUpWardForce;
    public float dashDuration;

    //Settings
    [Header("Settings")]
    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    void Awake()
    {
        //Grab sens
        QuakeSourceSens = Sensitivity.getSensitivity();
        DegreesPerPixel = QuakeSourceSens * 0.022f;

    //Controls
    controls = new EnforcersControls();

        //References
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform currTransform in transforms)
        {
            switch (currTransform.name)
            {
                case "Feet":
                    Feet = currTransform;
                    break;
                case "Player":
                    currTransform.GetComponent<MeshRenderer>().enabled = false;
                    break;
                case "CameraRot":
                    CameraRot = currTransform;
                    break;
                case "PlayerUI":
                    PlayerUI = currTransform.GetComponent<Canvas>();
                    break;
                case "HPBarFill":
                    HPBarFill = currTransform.GetComponent<Image>();
                    HPBarFillRect = currTransform.GetComponent<RectTransform>();
                    break;
                case "DashBarFill":
                    DashBarFill = currTransform.GetComponent<Image>();
                    DashBarFillRect = currTransform.GetComponent<RectTransform>();
                    break;
                case "RedKey":
                    RedKey = currTransform.GetComponent<Image>();
                    break;
                case "GreenKey":
                    GreenKey = currTransform.GetComponent<Image>();
                    break;
                case "BlueKey":
                    BlueKey = currTransform.GetComponent<Image>();
                    break;
                case "ShieldText":
                    ShieldText = currTransform.GetComponent<TextMeshProUGUI>();
                    break;
                case "ShieldMask":
                    ShieldMask = currTransform.GetComponent<RectTransform>();
                    break;
                case "WeaponHolder":
                    weaponSwitching = currTransform.GetComponent<WeaponSwitching>();
                    break;
            }
        }
        //PlayerHP
        playerHP = GetComponent<PlayerHP>();
        //Hitbox
        Hitbox = GetComponent<Rigidbody>();
        //GroundLayer
        GroundLayer = LayerMask.GetMask("Ground");
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Gameplay.WASD.performed += OnWASD;
        controls.Gameplay.WASD.canceled += OnWASD;
        controls.Gameplay.Shoot.performed += OnShoot;
        controls.Gameplay.Shoot.canceled += OnShoot;
        controls.Gameplay.MouseMove.performed += OnMouseMove;
        controls.Gameplay.Reload.performed += OnReload;
        controls.Gameplay.Jump.performed += OnJump;
        controls.Gameplay.Dash.performed += OnDash;
    }

    public void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        //WASD movement
        Vector3 direction = new Vector3(movement.x, 0f, movement.y);
        direction = direction.normalized;
        if (direction.x != 0)
        {
            Hitbox.MovePosition(Hitbox.position + transform.right * direction.x * ConstSpeed * Time.deltaTime);
        }
        if (direction.z != 0)
        {
            Hitbox.MovePosition(Hitbox.position + transform.forward * direction.z * ConstSpeed * Time.deltaTime);
        }

        //Lock and unlock cursor
        if (Input.GetKeyDown("escape"))
        {
            UnlockCursor();
        }
        if (Input.GetKeyDown("tab"))
        {
            LockCursor();
        }

        //Camera and player rotation
        float currX = mousePos.x;
        float currY = mousePos.y;
        float deltaX = currX - lastXCoord;
        float deltaY = currY - lastYCoord;
        float newRotYaw = deltaX * DegreesPerPixel + lastRotYaw;
        float newRotPitch = deltaY * DegreesPerPixel + lastRotPitch;
        newRotPitch = newRotPitch > 90f ? 90f : newRotPitch;
        newRotPitch = newRotPitch < -90f ? -90f : newRotPitch;
        Quaternion horizontalRotation = Quaternion.Euler(0f, newRotYaw, 0f);
        Quaternion verticalRotation = Quaternion.Euler(-newRotPitch, 0f, 0f);
        transform.localRotation = horizontalRotation;
        CameraRot.localRotation = verticalRotation;
        lastXCoord = currX;
        lastYCoord = currY;
        lastRotYaw = newRotYaw;
        lastRotPitch = newRotPitch;

        //Dash
        Globals.PlayerDash += Time.deltaTime / 1f;
        Globals.PlayerDash = Globals.PlayerDash > 1f ? 1f : Globals.PlayerDash;

        //Jump
        isTouchingGroundThisFrame = Physics.CheckSphere(Feet.position, 0.5f, GroundLayer, QueryTriggerInteraction.Ignore);
        if (isTouchingGroundThisFrame&&!isTouchingGroundLastFrame)
        {
            jumpCount = ConstMaxJumps;
        }
        isTouchingGroundLastFrame = isTouchingGroundThisFrame;

        //Fire
        if (isShooting)
        {
            weaponSwitching.gunSystems[weaponSwitching.selectedWeapon].TryShoot();
        }

        //Update UI
        DashBarFillRect.sizeDelta = new Vector2(200f * Globals.PlayerDash, 20f);
        DashBarFill.color = Globals.PlayerDash == 1f ? ChargedYellow : CooldownGray;
        HPBarFillRect.sizeDelta = new Vector2(200f * ((float)playerHP.health / 100f), 20f);
        ShieldText.text = playerHP.shield.ToString();
        ShieldMask.offsetMin = new Vector2(ShieldMask.offsetMin.x, -46f * (playerHP.shield / 100f));
        RedKey.color = new Color(1f, 1f, 1f, Globals.hasRedKey ? 1f : 0f);
        GreenKey.color = new Color(1f, 1f, 1f, Globals.hasGreenKey ? 1f : 0f);
        BlueKey.color = new Color(1f, 1f, 1f, Globals.hasBlueKey ? 1f : 0f);

        //Check for death
        if (playerHP.didHandleDeath)
        {
            controls.Disable();
        }
    }

    private void OnWASD(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        mousePos.x += delta.x;
        mousePos.y += delta.y;
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        isShooting = !context.canceled;
    }

    private void OnReload(InputAction.CallbackContext context)
    {
        weaponSwitching.gunSystems[weaponSwitching.selectedWeapon].TryReload();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (jumpCount > 0)
        {
            jumpCount--;
            Vector3 oldVelocity = Hitbox.velocity;
            Hitbox.velocity = new Vector3(oldVelocity.x, 0f, oldVelocity.z);
            Hitbox.AddForce(Vector3.up * Mathf.Sqrt(ConstJumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
    }
    private void OnDash(InputAction.CallbackContext context)
    {
        //Bail if not enough charge
        if (Globals.PlayerDash < 1.0f)
        {
            return;
        }

        //Subtract charge
        Globals.PlayerDash -= 1.0f;

        Transform forwardT;
        if (useCameraForward)
        {
            forwardT = CameraRot;
        }
        else
        {
            forwardT = transform;
        }
        Vector3 direction = GetDirection(forwardT);
        Vector3 forcetoApply = direction * dashForce + transform.up * dashUpWardForce;
        Vector3 nerfedVerticalForce = new Vector3(forcetoApply.x, forcetoApply.y * 0.5f, forcetoApply.z);
        if (disableGravity)
        {
            Hitbox.useGravity = false;
        }
        Hitbox.AddForce(nerfedVerticalForce, ForceMode.Impulse);
        Invoke(nameof(ResetDash), dashDuration);
    }

    private void ResetDash()
    {
        Hitbox.useGravity = true;
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3();
        if (allowAllDirections)
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        else
            direction = forwardT.forward;

        if (verticalInput == 0 && horizontalInput == 0)
            direction = forwardT.forward;
        return direction.normalized;
    }

    public static void LockCursor()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void UnlockCursor()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}