using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[RequireComponent(typeof(NetworkCharacterControllerCustom))]
public class PlayerController : NetworkBehaviour
{

    NetworkCharacterControllerCustom _myCharacterController;
    private void Awake()
    {
        _myCharacterController = GetComponent<NetworkCharacterControllerCustom>();
    }

    public override void FixedUpdateNetwork()
    {
        //TOMA DE INPUTS

        if (!GetInput(out NetworkInputData networkinputData)) return;

        //MOVIMIENTO

        Vector2 moveDirection = Vector3.forward * networkinputData.movementinput;
        _myCharacterController.Move(moveDirection);

        //SALTO

        if (networkinputData.isJumpPressed)
        {

        }
    }
}     
