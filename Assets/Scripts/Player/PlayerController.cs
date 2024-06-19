using UnityEngine;
using Fusion;
using System;

[RequireComponent(typeof(NetworkCharacterControllerCustom))]
public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterControllerCustom _myCharacterController;
    [SerializeField] private LineRenderer myRend;
    public float maxDistance = 2f; // La distancia máxima que deseas permitir
    Camera cam;
    public Gradient defaultColor, polarityPlusColor, polarityNegativeColor;
    RaycastHit hit;
    private bool polarityPlus, polarityNegative;
    private void Awake()
    {
        cam = Camera.main;
        _myCharacterController = GetComponent<NetworkCharacterControllerCustom>();
        myRend = GetComponent<LineRenderer>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData networkInputData)) return;
        //MOVIMIENTO
        Vector3 moveDirection = Vector3.forward * networkInputData.movementInput;
        _myCharacterController.Move(moveDirection);

        SetLineRenderer();
        //SALTO

        if (networkInputData.networkButtons.IsSet(MyButtons.Jump))
        {
            _myCharacterController.Jump();
        }

        if (networkInputData.networkButtons.IsSet(MyButtons.Activate))
        {
            //_myCharacterController.Activate();
        }
        
        //Polarity
        if (networkInputData._negativePolarity)
        {
            if(polarityPlus)
                _myCharacterController.ApplyForce(hit.point, true);
            else if(polarityNegative)
                _myCharacterController.ApplyForce(hit.point, false);
        }
        if (networkInputData._positivePolarity)
        {
            if(polarityPlus)
                _myCharacterController.ApplyForce(hit.point, false);
            else if(polarityNegative)
                _myCharacterController.ApplyForce(hit.point, true);
        }

    }

    private void SetLineRenderer(){

        Vector3 currentPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        currentPoint.z = 0;

        // Calcular la dirección y la distancia desde el jugador hacia el mouse
        Vector3 direction = currentPoint - transform.position;
        float distance = direction.magnitude;

        // Clamp la distancia
        distance = Mathf.Min(maxDistance, maxDistance);

        // Calcular la posición final clamped
        Vector3 clampedPosition = transform.position + direction.normalized * distance;

        // Actualizar los puntos del LineRenderer
        myRend.SetPosition(0, transform.position); // El punto de inicio es la posición del jugador
        myRend.SetPosition(1, clampedPosition); // El punto final es la posición clamped

        // Realizar un Raycast desde el jugador hasta la posición clamped
        
        if (Physics.Raycast(transform.position, direction.normalized, out hit, distance))
        {
            if (hit.collider.CompareTag("Polarity +"))
            {
                myRend.colorGradient = polarityPlusColor;
                polarityPlus = true;
                polarityNegative = false;
            }
            else if (hit.collider.CompareTag("Polarity -"))
            {
                myRend.colorGradient = polarityNegativeColor;
                polarityPlus = false;
                polarityNegative = true;
            }
            else
            {
                myRend.colorGradient = defaultColor;
                polarityPlus = false;
                polarityNegative = false;
            }
        }
        else
        {
            myRend.colorGradient = defaultColor;
            polarityPlus = false;
            polarityNegative = false;
        }
    }
}
