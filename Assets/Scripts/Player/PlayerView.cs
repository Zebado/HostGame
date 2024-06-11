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
        if (!HasStateAuthority) return;

        _mecanim = GetComponentInChildren<NetworkMecanimAnimator>();
            
        var m = GetComponentInParent<PlayerController>();

        if (!m || !_mecanim) return;

        m.OnMovement += MoveAnimation;
        m.OnJump += JumpAnimation;
        //m.OnDead += DeadAnimation;
        //m.OnReset += ResetAnimation;
        //m.OnShoot += ShootAnimation;
        //m.OnShooting += RPC_TriggerShootingParticles;

    }
    void MoveAnimation(float xAxi)
    {
        _mecanim.Animator.SetFloat("xAxi", Mathf.Abs(xAxi));
    }
    void DeadAnimation()
    {
        _mecanim.Animator.SetBool("isDead", true);
    }
    void ResetAnimation()
    {
        _mecanim.Animator.SetBool("isDead", false);
    }
    void ShootAnimation(bool state){
        _mecanim.Animator.SetBool("isShooting", state);
    }
    void JumpAnimation(bool state){
        _mecanim.Animator.SetBool("isJumping", state);
    }
}
