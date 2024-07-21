using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PolarityBox : NetworkBehaviour, IPolarity
{

    [Networked][HideInInspector] public bool polarityPlus { get; set; }
    [Networked][HideInInspector] public bool polarityMinus { get; set; }
    [Networked][HideInInspector] public bool isDisabled { get; set; }

    [SerializeField] private bool polarityPlusAux, polarityMinusAux, isDisabledAux;
    private Rigidbody2D myRb;
    [SerializeField] private Sprite spritePlus, spriteMinus, spriteDisabled;
    private SpriteRenderer myRend;

    [SerializeField] private float forceMagnitude;
    public override void Spawned()
    {
        myRend = GetComponent<SpriteRenderer>();
        myRb = GetComponent<Rigidbody2D>();
        polarityPlus = polarityPlusAux;
        polarityMinus = polarityMinusAux;
        isDisabled = isDisabledAux;
        RPC_SetSprite();
        StartCoroutine(CallToSetSprite());
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetSprite()
    {
        if (myRend != null)
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
    }

    public void ApplyPolarity(NewCharacterController player, bool attract)
    {
        Vector3 forceDirection = (player.transform.position - transform.position).normalized;
        if (!attract)
        {
            forceDirection = -forceDirection;
            myRb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
        }
        else
        {
            myRb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
        }
        StartCoroutine(RestartPolarity());
    }

    private IEnumerator RestartPolarity()
    {
        isDisabled = true;
        yield return new WaitForSeconds(1);
        isDisabled = false;
    }
    private IEnumerator CallToSetSprite()
    {
        yield return new WaitForSeconds(1);
        RPC_SetSprite();
    }
}
