using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //Grabbed References
    private Transform TransformFeet;
    private Transform TransformCamera;
    private Rigidbody Hitbox;
    private AudioSource SFXAudioSource;

    //Assigned References
    public AudioClip audioClipDash;
    public GameObject prefabBullet;
    public TextMeshProUGUI ammocnt;

    //Player
    private const float speed = 10f;
    private const float jumpHeight = 3f;
    private const float dashDistance = 8f;
    private EnforcersControls controls;
    private Vector2 movement;
    private Vector2 mousePos;
    private LayerMask ground;

    //Rotation
    private Quaternion zeroQuat = Quaternion.Euler(0f, 0f, 0f);
    private const float QuakeSourceSens = 4.50f;
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

    void Start()
    {
        //TransformFeet
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Feet")
            {
                TransformFeet = transform.GetChild(i);
            }
        }
        //TransformCamera
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Camera")
            {
                TransformCamera = transform.GetChild(i);
            }
        }
        //Hitbox
        Hitbox = GetComponent<Rigidbody>();
        //SFXAudioSource
        SFXAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //WASD movement
        Vector3 direction = new Vector3(movement.x, 0f, movement.y);
        direction = direction.normalized;
        if (direction.x != 0)
        {
            Hitbox.MovePosition(Hitbox.position + transform.right * direction.x * speed * Time.deltaTime);
        }
        if (direction.z != 0)
        {
            Hitbox.MovePosition(Hitbox.position + transform.forward * direction.z * speed * Time.deltaTime);
        }

        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;

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
        gameObject.transform.localRotation = horizontalRotation;
        TransformCamera.localRotation = verticalRotation;
        lastXCoord = currX;
        lastYCoord = currY;
        lastRotYaw = newRotYaw;
        lastRotPitch = newRotPitch;

        /*bool isGrounded = Physics.CheckSphere(feet.position, 0.1f, ground, QueryTriggerInteraction.Ignore);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            hitbox.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }*/
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
        //
    }
    private void OnDash(InputAction.CallbackContext context)
    {
        //If we are not pressing WASD
        if (movement.x == 0 && movement.y == 0)
        {
            return;
        }

        //Dash
        Vector3 direction = new Vector3(movement.x, 0f, movement.y);
        direction = direction.normalized;
        Hitbox.MovePosition(Hitbox.position + transform.right * direction.x * dashDistance);
        Hitbox.MovePosition(Hitbox.position + transform.forward * direction.z * dashDistance);

        //Play SFX
        SFXAudioSource.PlayOneShot(audioClipDash);
    }

}