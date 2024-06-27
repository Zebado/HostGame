using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Acid : NetworkBehaviour, ITraps
{


    private void OnTriggerEnter(Collider other)
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

        PlayerHealth playerHealth = networkObject.GetComponent<PlayerHealth>();
        NetworkCharacterControllerCustom controller = networkObject.GetComponent<NetworkCharacterControllerCustom>();

        if (playerHealth != null)
        {
            if(playerHealth.health > 0)
                playerHealth.Death();
        }

    }
}
