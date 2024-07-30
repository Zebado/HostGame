using UnityEngine;
using Fusion;

public class AcidSound : NetworkBehaviour
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _acidClip;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            Rpc_PlayAcidSound();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_PlayAcidSound()
    {
        if (_audioSource != null)
        {
            Debug.Log("sonido de acido");
            _audioSource.PlayOneShot(_acidClip);
        }
    }
}
