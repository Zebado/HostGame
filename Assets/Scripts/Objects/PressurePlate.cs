using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PressurePlate : NetworkBehaviour
{
    [SerializeField] private PlatformWithPolarity platformToActivate;
    public PressurePlateType type;
    public enum PressurePlateType
    {
        ChangePolarity,
        SetPolarityPlus,
        SetPolarityMinus
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            switch (type)
            {
                case PressurePlateType.ChangePolarity:
                    platformToActivate.RPC_ChangePolarity();
                    break;
                case PressurePlateType.SetPolarityPlus:
                    platformToActivate.RPC_EnablePlus();
                    break;
                case PressurePlateType.SetPolarityMinus:
                    platformToActivate.RPC_EnableMinus();
                    break;  
                default:
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            switch (type)
            {
                case PressurePlateType.ChangePolarity:
                    platformToActivate.RPC_ChangePolarity();
                    break;
                case PressurePlateType.SetPolarityPlus:
                    platformToActivate.RPC_Disable();
                    break;
                case PressurePlateType.SetPolarityMinus:
                    platformToActivate.RPC_Disable();
                    break;  
                default:
                    break;
            }
        }
    }
}
