using UnityEngine;
using Fusion;
using System;

[RequireComponent(typeof(NetworkCharacterControllerCustom))]
public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterControllerCustom _myCharacterController;
    private bool _isJumping, _canShoot,_isShooting;
    public event Action<float> OnMovement = delegate {  };
    public event Action<bool> OnJump = delegate { };
    
    private void Awake()
    {
        _myCharacterController = GetComponent<NetworkCharacterControllerCustom>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData networkInputData)) return;
        
        //MOVIMIENTO

        Vector3 moveDirection = Vector3.forward * networkInputData.movementInput;
        _myCharacterController.Move(moveDirection);
        OnMovement(moveDirection.x);
        
        //SALTO

        if (networkInputData.networkButtons.IsSet(MyButtons.Jump))
        {
            _myCharacterController.Jump();
            _isJumping = true;
            OnJump(_isJumping);
        }
        

    }
}
