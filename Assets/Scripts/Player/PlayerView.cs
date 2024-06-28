using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerView : NetworkBehaviour
{
    // Start is called before the first frame update
    private NetworkMecanimAnimator _mecanim;
    public override void Spawned()
    {

        _mecanim = GetComponentInChildren<NetworkMecanimAnimator>();
            
        var m = GetComponentInParent<NewCharacterController>();

        if (!m || !_mecanim) return;

        m.OnMovement += MoveAnimation;
        m.OnJump += JumpAnimation;
        m.OnFall += FallAnimation;
        m.OnTakeDamage += TakeDamageAnimation;
        m.OnDead += DeadAnimation;
        //m.OnReset += ResetAnimation;
        //m.OnShoot += ShootAnimation;
        //m.OnShooting += RPC_TriggerShootingParticles;

    }
    void MoveAnimation(float xAxi)
    {
        _mecanim.Animator.SetFloat("axiX", Mathf.Abs(xAxi));
    }
    void DeadAnimation()
    {
        _mecanim.Animator.SetTrigger("isDead");
    }
    void ResetAnimation()
    {
        _mecanim.Animator.SetBool("isDead", false);
    }
    void ShootAnimation(bool state){
        _mecanim.Animator.SetBool("isShooting", state);
    }
    void JumpAnimation(){
        _mecanim.Animator.SetTrigger("isJumping");
    }
    void FallAnimation(bool state){
        _mecanim.Animator.SetBool("isFalling", state);
    }
    void TakeDamageAnimation(){
        _mecanim.Animator.SetTrigger("takeDamage");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_HandlePlayerDeath()
    {
        GameManager.Instance.ShowDefeatMenu();
    }
}
