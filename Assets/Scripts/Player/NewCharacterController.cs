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

    [Header("LineRenderer")]
    [SerializeField] private LineRenderer myRend;
    [SerializeField] private float maxDistance = 2f; // La distancia máxima que deseas permitir
    [SerializeField] private Gradient defaultColor, polarityPlusColor, polarityNegativeColor;
    private Camera cam;
    RaycastHit2D hit;

    [Header("Polarity")]
    [SerializeField]private bool polarityPlus, polarityMinus;
    private int positiveForceCount = 0;
    private int negativeForceCount = 0;
    [SerializeField] private int MaxForceCount = 2;
    [SerializeField] private bool hasPositiveMagnet, hasNegativeMagnet;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        cam = Camera.main;
        myRend = GetComponent<LineRenderer>();
        myRend.enabled = !HasInputAuthority;
    }
    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData inputData)) return;
        if(isDead)return;

        Vector2 moveDirection = Vector2.right * inputData.movementInput;
        Move(moveDirection);

        SetLineRenderer();

        if (inputData.networkButtons.IsSet(MyButtons.Jump))
        {
            Jump();
        }
        if (inputData.networkButtons.IsSet(MyButtons.Activate))
        {
            ActivateObjects(activablesInRange);
        }
        HandlePolarity(inputData);

        CheckGroundStatus();
        if (isGrounded)
        {
            positiveForceCount = MaxForceCount;
            negativeForceCount = MaxForceCount;
        }
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

    private void HandlePolarity(NetworkInputData inputData)
    {
        if (inputData._negativePolarity && hasNegativeMagnet)
        {
            if (polarityPlus && negativeForceCount > 0)
            {
                ApplyForce(hit.point, true);
                StartCoroutine(ReduceForceCount(false, 0.1f));
            }
            else if (polarityMinus && negativeForceCount > 0)
            {
                ApplyForce(hit.point, false);
                StartCoroutine(ReduceForceCount(false, 0.1f));
            }
        }

        if (inputData._positivePolarity && hasPositiveMagnet)
        {
            if (polarityPlus && positiveForceCount > 0)
            {
                ApplyForce(hit.point, false);
                StartCoroutine(ReduceForceCount(true, 0.1f));
            }
            else if (polarityMinus && positiveForceCount > 0)
            {
                ApplyForce(hit.point, true);
                StartCoroutine(ReduceForceCount(true, 0.1f));
            }
        }
    }

    IEnumerator ReduceForceCount(bool forceCounter, float time){
        yield return new WaitForSeconds(time);
        if(forceCounter)
            positiveForceCount--;
        else
            negativeForceCount--;
    }

    public void ApplyForce(Vector3 targetPoint, bool attract)
    {
              
        Vector3 forceDirection = (targetPoint - transform.position).normalized;
        if (!attract)
        {
            forceDirection = -forceDirection;
        }
        rb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);

    }

    private void SetLineRenderer()
    {
        if (!HasInputAuthority) return;

        Vector3 currentPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        currentPoint.z = 0;

        Vector3 direction = currentPoint - transform.position;
        float distance = direction.magnitude;

        distance = Mathf.Clamp(distance,maxDistance, maxDistance);

        Vector3 clampedPosition = transform.position + direction.normalized * distance;

        myRend.SetPosition(0, transform.position);
        myRend.SetPosition(1, clampedPosition);

        hit = Physics2D.Raycast(transform.position, direction.normalized, distance, groundLayer); // Ensure the raycast checks the correct layer
        if (hit.collider != null)
        {
            PlatformWithPolarity platform = hit.collider.GetComponent<PlatformWithPolarity>();
            if (platform != null)
            {
                if (platform.isDisabled)
                {
                    myRend.colorGradient = defaultColor;
                    polarityPlus = false;
                    polarityMinus = false;
                }
                else if (platform.polarityPlus && platform.polarityMinus)
                {
                    myRend.colorGradient = polarityPlusColor; // or some other color to indicate both polarities
                    polarityPlus = true;
                    polarityMinus = true;
                }
                else if (platform.polarityPlus && !platform.polarityMinus)
                {
                    myRend.colorGradient = polarityPlusColor;
                    polarityPlus = true;
                    polarityMinus = false;
                }
                else if (!platform.polarityPlus && platform.polarityMinus)
                {
                    myRend.colorGradient = polarityNegativeColor;
                    polarityPlus = false;
                    polarityMinus = true;
                }
                else
                {
                    myRend.colorGradient = defaultColor;
                    polarityPlus = false;
                    polarityMinus = false;
                }
            }
            else
            {
                myRend.colorGradient = defaultColor;
                polarityPlus = false;
                polarityMinus = false;
            }
        }
        else
        {
            myRend.colorGradient = defaultColor;
            polarityPlus = false;
            polarityMinus = false;
        }
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
        // Usar una lista temporal para evitar la modificación de la colección durante la enumeración
        List<IActivable> tempActivables = new List<IActivable>(activablesInRange);
        foreach (var activable in tempActivables)
        {
            activable.Activate();
        }
    }
}

