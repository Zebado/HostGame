using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using System;

public class NetworkCharacterControllerCustom : NetworkCharacterController
{
    public event Action<float> OnMovement = delegate {  };
    public event Action OnJump = delegate { };
    public event Action<bool> OnFall = delegate { };

    // Agregar un campo para la magnitud de la fuerza
    public float forceMagnitude = 10f;

    

    public override void Move(Vector3 direction)
    {
        var deltaTime = Runner.DeltaTime;
        var previousPos = transform.position;
        var moveVelocity = Velocity;
        

        direction = direction.normalized;

        if (Grounded && moveVelocity.y < 0)
        {
            moveVelocity.y = 0f;
        }
        else
            moveVelocity.y += gravity * Runner.DeltaTime;

        var horizontalVel = default(Vector3);
        horizontalVel.z = moveVelocity.x;

        if (direction == default)
        {
            horizontalVel = Vector3.Lerp(horizontalVel, default, braking * deltaTime);
        }
        else
        {
            horizontalVel = Vector3.ClampMagnitude(horizontalVel + direction * acceleration * deltaTime, maxSpeed);
            transform.rotation = Quaternion.Euler(Vector3.up * (Mathf.Sign(direction.z) < 0 ? 180 : 0));
        }

        moveVelocity.x = horizontalVel.z;

        Controller.Move(moveVelocity * deltaTime);
        OnMovement(moveVelocity.x);

        Velocity = (transform.position - previousPos) * Runner.TickRate;
        Grounded = Controller.isGrounded;
        OnFall(!Grounded);

        
    }
    public override void Jump(bool ignoreGrounded = false, float? overrideImpulse = null){
        if (Grounded || ignoreGrounded)
        {
            var newVel = Velocity;
            newVel.y += overrideImpulse ?? jumpImpulse;
            Velocity = newVel;
            OnJump();
        }
    }

    public void ApplyForce(Vector3 targetPoint, bool attract)
    {
               
        Vector3 forceDirection = (targetPoint - transform.position).normalized;
        if (!attract)
        {
            forceDirection = -forceDirection;
        }

        // Aplicar la fuerza a la velocidad del jugador
        var moveVelocity = Velocity;
        moveVelocity += forceDirection * forceMagnitude;

        Velocity = moveVelocity;

    }
    public void TakeDamage(Vector3 targetPoint){
        var moveVelocity = Velocity;
        moveVelocity += targetPoint * (2);
        Velocity = moveVelocity;
    }

    public void ActivateObjects(List<IActivable> activablesInRange){
        foreach (var activable in activablesInRange)
        {
            activable.Activate();
        }
    }
}
