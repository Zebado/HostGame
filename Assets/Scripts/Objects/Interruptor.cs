using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class Interruptor : MonoBehaviour, IActivable
{
    [SerializeField] private Door door;
    private NetworkMecanimAnimator _mecanim;
    private void Start() {
        _mecanim = GetComponent<NetworkMecanimAnimator>();
    }

    public void Activate()
    {
        // Implementa la lógica de activación aquí
        door.ChangeToActive();
        _mecanim.Animator.SetBool("Active", true);
        GetComponent<BoxCollider>().enabled = false;
    }
}

