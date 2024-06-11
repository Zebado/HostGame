using UnityEngine;
using Fusion;

[RequireComponent(typeof(NetworkCharacterControllerCustom))]
public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterControllerCustom _myCharacterController;
    
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
        
        //SALTO

        if (networkInputData.networkButtons.IsSet(MyButtons.Jump))
        {
            _myCharacterController.Jump();
        }
        

    }
}
