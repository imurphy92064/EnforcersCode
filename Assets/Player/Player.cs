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
    private RectTransform DashMeterBlue;
    private TextMeshProUGUI AmmoText;
    private LayerMask ground;

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
        Transform[] allDescendants = GetComponentsInChildren<Transform>();

        //Feet
        for (int i = 0; i < allDescendants.Length; i++)
        {
            if (allDescendants[i].name == "Feet")
            {
                Feet = allDescendants[i];
            }
        }
        //Camera
        for (int i = 0; i < allDescendants.Length; i++)
        {
            if (allDescendants[i].name == "Camera")
            {
                Camera = allDescendants[i];
            }
        }
        //PlayerUI
        for (int i = 0; i < allDescendants.Length; i++)
        {
            if (allDescendants[i].name == "PlayerUI")
            {
                PlayerUI = allDescendants[i].GetComponent<Canvas>();
            }
        }
        //DashMeterBlue
        for (int i = 0; i < allDescendants.Length; i++)
        {
            if (allDescendants[i].name == "DashMeterBlue")
            {
                DashMeterBlue = allDescendants[i].GetComponent<RectTransform>();
            }
        }
        //Hitbox
        Hitbox = GetComponent<Rigidbody>();
        //SFXAudioSource
        SFXAudioSource = GetComponent<AudioSource>();

        //AmmoText
        //Ground
        ground = LayerMask.GetMask("Ground");
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
        dashCharge += Time.deltaTime / 3.0f;
        dashCharge = dashCharge > 1.0f ? 1.0f : dashCharge;
        DashMeterBlue.sizeDelta = new Vector2(320.0f * dashCharge, 20.0f);

        //Jump
        if (Physics.CheckSphere(Feet.position, 0.1f, ground, QueryTriggerInteraction.Ignore))
        {
            jumpCount = ConstMaxJumps;
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
        if (dashCharge >= 0.5f)
        {
            dashCharge -= 0.5f;

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