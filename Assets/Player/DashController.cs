using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

The Dash Controller contains code for the player character to dash in all directions.

*/

public class DashController : MonoBehaviour
{

    /* References and Variables */
    [Header("References")]
    public Transform transformm;
    public Rigidbody Hitbox;
    public Transform playerCam;

    [Header("DashVariables")]
    public float dashForce;
    public float dashUpWardForce;
    public float dashDuration;

    [Header("Settings")]
    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;




    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //playerScript = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(dashkey))
        {
            Dash();
        }*/
    }

    private void Dash()
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
            forwardT = playerCam;
        else
            forwardT = transformm;

        Vector3 direction = GetDirection(forwardT);
        Vector3 forcetoApply = direction * dashForce + transformm.up * dashUpWardForce;
        if (disableGravity)
            Hitbox.useGravity = false;
        Hitbox.AddForce(forcetoApply, ForceMode.Impulse);
        Debug.Log("dashing!");
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
}