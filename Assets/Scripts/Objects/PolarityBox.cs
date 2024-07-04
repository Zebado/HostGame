using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PolarityBox : NetworkBehaviour, IPolarity {
    
    [Networked] [SerializeField] public bool polarityPlus { get; set; }
    [Networked] [SerializeField] public bool polarityMinus { get; set; }
    [Networked] [SerializeField] public bool isDisabled { get; set; }
    private Rigidbody2D myRb;
    [SerializeField] private Sprite spritePlus, spriteMinus, spriteDisabled;
    private SpriteRenderer myRend;

    [SerializeField] private float forceMagnitude;
    private void Awake() {
        myRend = GetComponent<SpriteRenderer>();
        myRb = GetComponent<Rigidbody2D> ();
    }
    public override void FixedUpdateNetwork() {
        RPC_SetSprite();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetSprite()
    {
        if (isDisabled)
        {
            myRend.sprite = spriteDisabled;
        }
        else if (polarityPlus && !polarityMinus)
        {
            myRend.sprite = spritePlus;
        }
        else if (!polarityPlus && polarityMinus)
        {
            myRend.sprite = spriteMinus;
        }
        else
        {
            myRend.sprite = spriteDisabled;
        }
    }

    public void ApplyPolarity(NewCharacterController player, bool attract){
        Vector3 forceDirection = (player.transform.position - transform.position).normalized;
        if (!attract)
        {
            forceDirection = -forceDirection;
            myRb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
        }
        else{
            myRb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
        }
        StartCoroutine(RestartPolarity());
    }

    private IEnumerator RestartPolarity(){
        isDisabled = true;
        yield return new WaitForSeconds(1);
        isDisabled = false;
    }
}
