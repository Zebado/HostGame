using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlatformWithPolarity : NetworkBehaviour
{
    [Networked] [SerializeField] public bool polarityPlus { get; set; }
    [Networked] [SerializeField] public bool polarityMinus { get; set; }
    [Networked] [SerializeField] public bool isDisabled { get; set; }

    [SerializeField] private Sprite spritePlus, spriteMinus, spriteDisabled;
    private SpriteRenderer myRend;

    public override void Spawned()
    {
        myRend = GetComponent<SpriteRenderer>();
    }

    public override void FixedUpdateNetwork(){
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

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ChangePolarity()
    {
        polarityPlus = !polarityPlus;
        polarityMinus = !polarityMinus;
        RPC_SetSprite();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Disable()
    {
        isDisabled = true;
        polarityPlus = false;
        polarityMinus = false;
        RPC_SetSprite();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_EnablePlus()
    {
        isDisabled = false;
        polarityPlus = true;
        polarityMinus = false;
        RPC_SetSprite();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_EnableMinus()
    {
        isDisabled = false;
        polarityPlus = false;
        polarityMinus = true;
        RPC_SetSprite();
    }
}