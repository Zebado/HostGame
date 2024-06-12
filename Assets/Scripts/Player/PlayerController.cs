using UnityEngine;
using Fusion;
using System;

[RequireComponent(typeof(NetworkCharacterControllerCustom))]
public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterControllerCustom _myCharacterController;
    [SerializeField] private LayerMask walls;
    [SerializeField] private Vector3 wallBoxDimension;
    
    private void Awake()
    {
        _myCharacterController = GetComponent<NetworkCharacterControllerCustom>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData networkInputData)) return;
       // _myCharacterController.inWall = Physics2D.OverlapBox(transform.position + new Vector3(2,0,0), wallBoxDimension, 0f, walls);
        //MOVIMIENTO

        Vector3 moveDirection = Vector3.forward * networkInputData.movementInput;
        _myCharacterController.Move(moveDirection);
        
        
        //SALTO

        if (networkInputData.networkButtons.IsSet(MyButtons.Jump))
        {
            _myCharacterController.Jump();
        }
        

    }
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.gameObject.layer == walls){
            _myCharacterController.inWall = true;
        }
        //else
           // _myCharacterController.inWall = false;
    }
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.layer == walls){
            _myCharacterController.inWall = true;
        }
    }
}
