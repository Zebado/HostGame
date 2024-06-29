using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class Interruptor : NetworkBehaviour, IActivable
{
    [SerializeField] private Door door;
    [SerializeField] private Sprite activeSprite;
    private SpriteRenderer myRend;
    //private NetworkMecanimAnimator _mecanim;
    public override void Spawned() {
        //_mecanim = GetComponent<NetworkMecanimAnimator>();
        myRend = GetComponent<SpriteRenderer>();
    }

    public void Activate()
    {
        // Implementa la lógica de activación aquí
        door.ChangeToActive();
        RPC_ChangeSprite();
        GetComponent<BoxCollider2D>().enabled = false;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ChangeSprite(){
        myRend.sprite = activeSprite;
    }
}

