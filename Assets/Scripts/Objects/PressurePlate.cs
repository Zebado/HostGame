using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PressurePlate : NetworkBehaviour
{
    public enum PressurePlateType
    {
        ChangePolarity,
        SetPolarityPlus,
        SetPolarityMinus
    }
    [SerializeField] private List<PlatformWithPolarity> platformToActivate;
    public PressurePlateType type;
    [SerializeField] private bool isSwitch;
    private NetworkMecanimAnimator _mecanim;

    public override void Spawned(){
        _mecanim = GetComponentInChildren<NetworkMecanimAnimator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(_mecanim != null){
                _mecanim.Animator.SetBool("isPressed", true);
            }
            switch (type)
            {
                case PressurePlateType.ChangePolarity:
                    foreach (var platform in platformToActivate)
                    {
                        platform.RPC_ChangePolarity();
                    }
                    break;
                case PressurePlateType.SetPolarityPlus:
                    foreach (var platform in platformToActivate)
                    {
                        platform.RPC_EnablePlus();
                    }
                    break;
                case PressurePlateType.SetPolarityMinus:
                    foreach (var platform in platformToActivate)
                    {
                        platform.RPC_EnableMinus();
                    }
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
            if(_mecanim != null)
                _mecanim.Animator.SetBool("isPressed", false);
            if(isSwitch)
                return;
            switch (type)
            {
                case PressurePlateType.ChangePolarity:
                    foreach (var platform in platformToActivate)
                    {
                        platform.RPC_ChangePolarity();
                    }
                    break;
                case PressurePlateType.SetPolarityPlus:
                    foreach (var platform in platformToActivate)
                    {
                        platform.RPC_Disable();
                    }
                    break;
                case PressurePlateType.SetPolarityMinus:
                    foreach (var platform in platformToActivate)
                    {
                        platform.RPC_Disable();
                    }
                    break;  
                default:
                    break;
            }
        }
    }
}
