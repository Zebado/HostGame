using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Fusion;
using System;

[RequireComponent(typeof(NetworkCharacterControllerCustom))]
public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterControllerCustom _myCharacterController;
    [SerializeField] public int lives = 3;
    [SerializeField] public bool vulnerable = true;
    private List<IActivable> activablesInRange = new List<IActivable>();
    [Header("LineRenderer")]
    [SerializeField] private LineRenderer myRend;
    [SerializeField] private float maxDistance = 2f; // La distancia máxima que deseas permitir
    Camera cam;
    [SerializeField] private Gradient defaultColor, polarityPlusColor, polarityNegativeColor;
    RaycastHit hit;

    [Header("Polarity")]
    private bool polarityPlus, polarityNegative;
    // Variables para rastrear cuántas veces la fuerza ha sido aplicada
    private int positiveForceCount = 0;
    private int negativeForceCount = 0;
    [SerializeField] private int MaxForceCount = 2;

    [SerializeField] private bool hasPositiveMagnet, hasNegativeMagnet;
    private void Awake()
    {
        cam = Camera.main;
        _myCharacterController = GetComponent<NetworkCharacterControllerCustom>();
        myRend = GetComponent<LineRenderer>();
        // Activar o desactivar el LineRenderer basado en la autoridad de entrada
        myRend.enabled = !HasInputAuthority;
    }

    public override void FixedUpdateNetwork()
    {
        if(lives <= 0)
            return;
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
            _myCharacterController.ActivateObjects(activablesInRange);
        }
        
        //Polarity
        if (networkInputData._negativePolarity && hasNegativeMagnet)
        {
            if(polarityPlus && negativeForceCount > 0){
                _myCharacterController.ApplyForce(hit.point, true);
                negativeForceCount--;
            }
            else if(polarityNegative && negativeForceCount > 0){
                _myCharacterController.ApplyForce(hit.point, false);
                negativeForceCount--;
            }
        }
        if (networkInputData._positivePolarity  && hasPositiveMagnet)
        {
            if(polarityPlus && positiveForceCount > 0){
                _myCharacterController.ApplyForce(hit.point, false);
                positiveForceCount--;
            }
            else if(polarityNegative && positiveForceCount > 0){
                _myCharacterController.ApplyForce(hit.point, true);
                positiveForceCount--;
            }
        }
        // Resetear los contadores cuando toca el suelo
        if (_myCharacterController.Grounded)
        {
            positiveForceCount = MaxForceCount;
            negativeForceCount = MaxForceCount;
        }

    }

    public void TakeDamage(){
        lives--;
        if(lives <= 0)
            Debug.Log("death");
        else{
            StartCoroutine(SetInvulnerable(1));
        }
    }
    private IEnumerator SetInvulnerable(float duration)
    {
        vulnerable = false;
        yield return new WaitForSeconds(duration);
        vulnerable = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        IActivable activable = other.GetComponent<IActivable>();
        if (activable != null)
        {
            activablesInRange.Add(activable);
        }
    }



    private void OnTriggerExit(Collider other)
    {
        IActivable activable = other.GetComponent<IActivable>();
        if (activable != null)
        {
            activablesInRange.Remove(activable);
        }
    }

    private void SetLineRenderer(){
        if(!HasInputAuthority)
            return;

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
