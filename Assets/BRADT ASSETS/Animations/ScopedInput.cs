using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopedInput : MonoBehaviour
{
    public Animator animator;

    public Camera mainCamera;

    public float scopedFOV = 15f;
    private float unscopedFOV;

    //private bool isScoped = false;

    void Update()
    {
        // click to scope
        /*if (Input.GetButtonDown("Fire2"))
        {
            isScoped = !isScoped;

            animator.SetBool("Scoped", isScoped);
        }*/ 

        // hold to scope
        if (Input.GetButtonDown("Fire2"))
        {
            animator.SetBool("Scoped", true);
            
            // set unscopedFOV value to the normal FOV value as holder
            unscopedFOV = mainCamera.fieldOfView;
            // change FOV to zoomed FOV value
            mainCamera.fieldOfView = scopedFOV;
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            animator.SetBool("Scoped", false);

            // return FOV to unscoped FOV value
            mainCamera.fieldOfView = unscopedFOV;
        }
    }
}
