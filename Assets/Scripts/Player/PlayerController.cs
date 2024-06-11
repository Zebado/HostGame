using UnityEngine;
using Fusion;

//[RequireComponent(typeof(NetworkCharacterControllerCustom))]
public class PlayerController : NetworkBehaviour
{
    //private NetworkCharacterControllerCustom _myCharacterController;
    //private WeaponHandler _myWeaponHandler;
    
    private void Awake()
    {
        //_myCharacterController = GetComponent<NetworkCharacterControllerCustom>();
        //_myWeaponHandler = GetComponent<WeaponHandler>();

        //var lifeHandler = GetComponent<LifeHandler>();

        /*lifeHandler.OnDeadChange += (isDead) =>
        {
            _myCharacterController.Controller.enabled = !isDead;
            enabled = !isDead;
        };*/

        /*lifeHandler.OnRespawn += () =>
        {
            _myCharacterController.Teleport(transform.position);
        };*/
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData networkInputData)) return;
        
        //MOVIMIENTO

        /*Vector3 moveDirection = Vector3.forward * networkInputData.movementInput;
        _myCharacterController.Move(moveDirection);
        
        //SALTO

        if (networkInputData.networkButtons.IsSet(MyButtons.Jump))
        {
            _myCharacterController.Jump();
        }*/
        
        //DISPARO
        /*
        if (networkInputData.isFirePressed)
        {
            _myWeaponHandler.Fire();
        }*/
    }
}
