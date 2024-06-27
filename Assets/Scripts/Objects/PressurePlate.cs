using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PressurePlate : NetworkBehaviour
{
    [SerializeField] private PlatformWithPolarity platformToActivate;
    [SerializeField] private bool playersOnPlate = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Arriba");
            platformToActivate.RPC_Enable();
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Afuera");
            platformToActivate.RPC_Disable();
        }
    }
}
