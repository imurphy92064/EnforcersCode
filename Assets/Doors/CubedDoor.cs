using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubedDoor : MonoBehaviour
{
    public PickupType KeyColor;

    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        switch (KeyColor)
        {
            case PickupType.RedKey:
                meshRenderer.enabled = !Globals.hasRedKey;
                boxCollider.enabled = !Globals.hasRedKey;
                break;
            case PickupType.GreenKey:
                meshRenderer.enabled = !Globals.hasGreenKey;
                boxCollider.enabled = !Globals.hasGreenKey;
                break;
            case PickupType.BlueKey:
                meshRenderer.enabled = !Globals.hasBlueKey;
                boxCollider.enabled = !Globals.hasBlueKey;
                break;
        }
    }
}
