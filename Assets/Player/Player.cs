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
    private Transform Camera;
    private Rigidbody Hitbox;
    private AudioSource SFXAudioSource;
    private Canvas PlayerUI;
    private RectTransform HPBarFill;
    private RectTransform DashBarFill;
    private TextMeshProUGUI AmmoText;
    private Image RedKey;
    private Image GreenKey;
    private Image BlueKey;
    private LayerMask GroundLayer;

    //Assigned References
    public AudioClip audioClipDash;
    public GameObject prefabBullet;

    //Player
    private const float ConstSpeed = 10f;
    private const float ConstJumpHeight = 3f;
    private const float ConstDashDistance = 8f;
    private const int ConstMaxJumps = 2;
    private float dashCharge = 1.0f;
    private int jumpCount = ConstMaxJumps;
    private EnforcersControls controls;
    private Vector2 movement;
    private Vector2 mousePos;

    //Rotation
    private Quaternion zeroQuat = Quaternion.Euler(0f, 0f, 0f);
    private static float QuakeSourceSens = 4.50f;
    private float DegreesPerPixel = QuakeSourceSens * 0.022f;
    private float lastXCoord;
    private float lastYCoord;
    private float lastRotYaw;
    private float lastRotPitch;

    //Weapons
    private int ammoCount = 25;

    void Awake()
    {
        controls = new EnforcersControls();
        Transform[] transforms = GetComponentsInChildren<Transform>();

        //Feet
        foreach (Transform transform in transforms)
        {
            if (transform.name == "Feet")
            {
                Feet = transform;
            }
        }
        //Camera
        foreach (Transform transform in transforms)
        {
            if (transform.name == "Camera")
            {
                Camera = transform;
            }
        }
        //PlayerUI
        foreach (Transform transform in transforms)
        {
            if (transform.name == "PlayerUI")
            {
                PlayerUI = transform.GetComponent<Canvas>();
            }
        }
        //HPBarFill
        foreach (Transform transform in transforms)
        {
            if (transform.name == "HPBarFill")
            {
                HPBarFill = transform.GetComponent<RectTransform>();
            }
        }
        //DashBarFill
        foreach (Transform transform in transforms)
        {
            if (transform.name == "DashBarFill")
            {
                DashBarFill = transform.GetComponent<RectTransform>();
            }
        }
        //RedKey
        foreach (Transform transform in transforms)
        {
            if (transform.name == "RedKey")
            {
                RedKey = transform.GetComponent<Image>();
            }
        }
        //GreenKey
        foreach (Transform transform in transforms)
        {
            if (transform.name == "GreenKey")
            {
                GreenKey = transform.GetComponent<Image>();
            }
        }
        //BlueKey
        foreach (Transform transform in transforms)
        {
            if (transform.name == "BlueKey")
            {
                BlueKey = transform.GetComponent<Image>();
            }
        }

        //Hitbox
        Hitbox = GetComponent<Rigidbody>();
        //SFXAudioSource
        SFXAudioSource = GetComponent<AudioSource>();

        //AmmoText

        //GroundLayer
        GroundLayer = LayerMask.GetMask("Ground");
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Gameplay.WASD.performed += OnWASD;
        controls.Gameplay.WASD.canceled += OnWASD;
        controls.Gameplay.MouseMove.performed += OnMouseMove;
        controls.Gameplay.Shoot.performed += OnShoot;
        controls.Gameplay.Reload.performed += OnReload;
        controls.Gameplay.Jump.performed += OnJump;
        controls.Gameplay.Dash.performed += OnDash;
    }

    private void OnDisable()
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

        //Lock cursor
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        //Camera and player rotation
        float currX = mousePos.x;
        float currY = mousePos.y;
        float deltaX = currX - lastXCoord;
        float deltaY = currY - lastYCoord;
        float newRotYaw = deltaX * DegreesPerPixel + lastRotYaw;
        float newRotPitch = deltaY * DegreesPerPixel + lastRotPitch;
        newRotPitch = newRotPitch > 90.0f ? 90.0f : newRotPitch;
        newRotPitch = newRotPitch < -90.0f ? -90.0f : newRotPitch;
        Quaternion horizontalRotation = Quaternion.Euler(0f, newRotYaw, 0f);
        Quaternion verticalRotation = Quaternion.Euler(-newRotPitch, 0f, 0f);
        transform.localRotation = horizontalRotation;
        Camera.localRotation = verticalRotation;
        lastXCoord = currX;
        lastYCoord = currY;
        lastRotYaw = newRotYaw;
        lastRotPitch = newRotPitch;

        //Dash
        dashCharge += Time.deltaTime / 4.0f;
        dashCharge = dashCharge > 1.0f ? 1.0f : dashCharge;
        DashBarFill.sizeDelta = new Vector2(200.0f * dashCharge, 20.0f);

        //Jump
        if (Physics.CheckSphere(Feet.position, 0.1f, GroundLayer, QueryTriggerInteraction.Ignore))
        {
            jumpCount = ConstMaxJumps;
        }

        RedKey.color = new Color(1.0f, 1.0f, 1.0f, Globals.hasRedKey ? 1.0f : 0.0f);
        GreenKey.color = new Color(1.0f, 1.0f, 1.0f, Globals.hasGreenKey ? 1.0f : 0.0f);
        BlueKey.color = new Color(1.0f, 1.0f, 1.0f, Globals.hasBlueKey ? 1.0f : 0.0f);
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
        //
    }
    private void OnReload(InputAction.CallbackContext context)
    {
        //
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        if (jumpCount > 0)
        {
            jumpCount--;
            Vector3 oldVelocity = Hitbox.velocity;
            Hitbox.velocity = new Vector3(oldVelocity.x, 0.0f, oldVelocity.z);
            Hitbox.AddForce(Vector3.up * Mathf.Sqrt(ConstJumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
    }
    private void OnDash(InputAction.CallbackContext context)
    {
        //If we are not pressing WASD
        if (movement.x == 0 && movement.y == 0)
        {
            return;
        }

        //If we have enough charge
        if (dashCharge >= (1.0f / 3.0f))
        {
            dashCharge -= (1.0f / 3.0f);

            //Dash
            Vector3 direction = new Vector3(movement.x, 0f, movement.y);
            direction = direction.normalized;
            Hitbox.MovePosition(Hitbox.position + transform.right * direction.x * ConstDashDistance);
            Hitbox.MovePosition(Hitbox.position + transform.forward * direction.z * ConstDashDistance);

            //Play SFX
            SFXAudioSource.PlayOneShot(audioClipDash);
        }
    }
}