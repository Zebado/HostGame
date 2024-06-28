using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using System;


public class NewCharacterController : NetworkBehaviour
{
    public event Action<float> OnMovement = delegate { };
    public event Action OnJump = delegate { };
    public event Action OnDead = delegate { };
    public event Action OnTakeDamage = delegate { };
    public event Action<bool> OnFall = delegate { };

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpImpulse = 10f;
    [SerializeField] private float forceMagnitude = 10f;
    [SerializeField] private LayerMask groundLayer;
    private bool isDead = false;
    [SerializeField] private List<IActivable> activablesInRange = new List<IActivable>();

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData inputData))
        {
            if(isDead)
                return;
            Vector2 moveDirection = Vector2.right * inputData.movementInput;
            Move(moveDirection);
            if (inputData.networkButtons.IsSet(MyButtons.Jump))
            {
                Jump();
            }
            if (inputData.networkButtons.IsSet(MyButtons.Activate))
            {
                ActivateObjects(activablesInRange);
            }
        }

        CheckGroundStatus();
    }

    private void Move(Vector2 direction)
    {
        direction = direction.normalized;
        Vector2 move = direction * moveSpeed;
        rb.velocity = new Vector2(move.x, rb.velocity.y);
        OnMovement(move.magnitude);

        // Invertir el sprite si se mueve hacia la izquierda
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
            OnJump();
        }
    }

    private void CheckGroundStatus()
    {
        Vector2 originPosition = transform.position - new Vector3(0,0.3f,0);
        isGrounded = Physics2D.OverlapCircle(originPosition, 0.27f, groundLayer);
        OnFall(!isGrounded);
    }

    public void Death(){
        isDead = true;
        Move(Vector2.zero);
        OnDead?.Invoke();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        IActivable activable = other.GetComponent<IActivable>();
        if (activable != null)
        {
            activablesInRange.Add(activable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IActivable activable = other.GetComponent<IActivable>();
        if (activable != null)
        {
            activablesInRange.Remove(activable);
        }
    }

    public void ActivateObjects(List<IActivable> activablesInRange){
        foreach (var activable in activablesInRange)
        {
            activable.Activate();
        }
    }
}

