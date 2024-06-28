using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlatformWithPolarity : NetworkBehaviour
{
    [Networked] [SerializeField] public bool polarityPlus { get; set; }
    [Networked] [SerializeField] public bool polarityMinus { get; set; }
    [Networked] [SerializeField] public bool isDisabled { get; set; }

    [SerializeField] private Sprite spritePlus, spriteMinus, spriteActive, spriteDisabled;
    private SpriteRenderer myRend;
    private bool startSetSprite = false;

    void Start()
    {
        myRend = GetComponent<SpriteRenderer>();
        StartCoroutine(InitializeSprites());
        
    }
    IEnumerator InitializeSprites(){
        yield return new WaitForSeconds(0.0001f);
        startSetSprite = true;
    }
    public override void FixedUpdateNetwork(){
        if (startSetSprite)
            RPC_SetSprite();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetSprite()
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