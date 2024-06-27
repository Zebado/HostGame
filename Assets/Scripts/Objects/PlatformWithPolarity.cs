using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlatformWithPolarity : NetworkBehaviour
{
    [Networked] [SerializeField] private bool polarityPlus { get; set; }
    [Networked] [SerializeField] private bool polarityMinus { get; set; }
    [Networked] [SerializeField] private bool isDisabled { get; set; }

    [SerializeField] private Sprite spritePlus, spriteMinus, spriteActive, spriteDisabled;
    private SpriteRenderer myRend;
    [SerializeField] private bool previousPolarityPlus;
    [SerializeField] private bool previousPolarityMinus;
    [SerializeField] private bool previousIsDisabled;

    void Awake()
    {
        myRend = GetComponent<SpriteRenderer>();
    }


    private void SetSprite()
    {
        if (isDisabled)
        {
            myRend.sprite = spriteDisabled;
        }
        else if (polarityPlus && polarityMinus)
        {
            myRend.sprite = spriteActive;
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
        SetSprite();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Disable()
    {
        isDisabled = true;
        polarityPlus = false;
        polarityMinus = false;
        SetSprite();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Enable()
    {
        isDisabled = false;
        SetSprite();
    }
}