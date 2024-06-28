using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Trap : NetworkBehaviour, ITraps
{


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkObject networkObject = other.GetComponent<NetworkObject>();
            if (networkObject != null && Object.HasStateAuthority)
            {
                RPC_HandlePlayerHit(networkObject);
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_HandlePlayerHit(NetworkObject networkObject)
    {
        Activate(networkObject);
    }

    public void Activate(NetworkObject networkObject){

        NewCharacterController controller = networkObject.GetComponent<NewCharacterController>();

        if (controller != null)
        {
            controller.Death();
        }

    }
}
