using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Door : MonoBehaviour, IActivable
{
    [SerializeField] Sprite doorActive;
    private NetworkMecanimAnimator _mecanim;
    public bool _active = false;

    private void Start() {
        _mecanim = GetComponent<NetworkMecanimAnimator>();
    }
    public void ChangeToActive(){
        GetComponent<SpriteRenderer>().sprite = doorActive;
        GetComponent<BoxCollider>().enabled = true;
    }
    public void Activate()
    {
        // Implementa la lógica de activación aquí
        _mecanim.Animator.SetTrigger("Open");
        if(_active){
            //Hacer el evento de que despawnee el player
        }
        Debug.Log("Object Activated!");
    }
}

