using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Spikes : NetworkBehaviour
{
    public int damage = 1; // Cantidad de vida a reducir
    public float pushForce = 5f; // Fuerza con la que se empuja al jugador

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
        PlayerController playerController = networkObject.GetComponent<PlayerController>();
        NetworkCharacterControllerCustom controller = networkObject.GetComponent<NetworkCharacterControllerCustom>();

        if (playerController != null)
        {
            if(playerController.vulnerable && playerController.lives > 0)
                playerController.TakeDamage();
        }

        if (controller != null && playerController.lives > 0)
        {
            Vector3 pushDirection = (networkObject.transform.position - transform.position).normalized;
            controller.TakeDamage(pushDirection);
        }
    }
}
