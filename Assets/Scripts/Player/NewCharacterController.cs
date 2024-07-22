using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using System;
using Fusion.Addons.Physics;
using static UnityEngine.EventSystems.EventTrigger;


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
    [SerializeField] private LayerMask groundLayer, playerLayer;
    private bool isDead = false;
    [SerializeField]private bool isWaitingForSpawn = false;
    [SerializeField] private bool isPlayer1;
    [SerializeField] private List<IActivable> activablesInRange = new List<IActivable>();

    private Rigidbody2D rb;
    private NetworkRigidbody2D netRb;
    private SpriteRenderer spriteRenderer;
    [Networked] public bool isGrounded { get; set; }

    [Header("LineRenderer")]
    [SerializeField] private LineRenderer myRend;
    [SerializeField] private float maxDistance = 2f; // La distancia máxima que deseas permitir
    [SerializeField] private Gradient defaultColor, polarityPlusColor, polarityNegativeColor;
    [SerializeField] private ParticleSystem particle;
    private Quaternion rotation;
    
    public RaycastHit2D hit;

    [Header("Polarity")]
    private bool polarityPlus, polarityMinus, reduceforceMagnitude, canMove;
    private int positiveForceCount = 0;
    private int negativeForceCount = 0;
    [SerializeField] private int MaxForceCount = 2;
    [SerializeField] private bool hasPositiveMagnet, hasNegativeMagnet;
    private IPolarity polarityObject;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody2D>();
        netRb = GetComponent<NetworkRigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        canMove = true;
        reduceforceMagnitude = false;
        myRend = GetComponent<LineRenderer>();
        myRend.enabled = !HasInputAuthority;
    }
    private void Update()
    {
        netRb.InterpolationTarget = transform;

    }
    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData inputData)) return;
        netRb.InterpolationTarget = transform;
        if (isWaitingForSpawn)
        {
            if (isPlayer1)
            {
                var spawnPoint = SpawnManager.Instance.GetSpawnPoint1();
                transform.position = spawnPoint.position;
            }
            else
            {
                var spawnPoint = SpawnManager.Instance.GetSpawnPoint2();
                transform.position = spawnPoint.position;
            }
            return;
        }
        if(isDead)return;
        if(canMove){
            Vector2 moveDirection = Vector2.right * inputData.movementInput;
            Move(moveDirection);
        }

        SetLineRenderer(inputData.mouseInput);

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
    public void SetNewLevelSpawn()
    {
        isWaitingForSpawn = true;
        Debug.Log("entro");
        StartCoroutine(WaitForPlayerInSpawn(0.1f));
    }
    IEnumerator WaitForPlayerInSpawn(float time)
    {
        isDead = false;
        yield return new WaitForSeconds(time);
        isWaitingForSpawn = false;
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
        Vector2 originPosition = transform.position - new Vector3(0, 0.3f, 0);
        isGrounded = Physics2D.OverlapCircle(originPosition, 0.27f, groundLayer) || Physics2D.OverlapCircle(originPosition, 0.27f, playerLayer);
        OnFall(!isGrounded);
    }

    private void HandlePolarity(NetworkInputData inputData)
    {
        if (inputData._negativePolarity && hasNegativeMagnet)
        {
            if (polarityPlus && negativeForceCount > 0)
            {
                polarityObject.ApplyPolarity(this, true);
                StartCoroutine(ReduceForceCount(false, 0.2f));
            }
            else if (polarityMinus && negativeForceCount > 0)
            {
                polarityObject.ApplyPolarity(this, false);
                StartCoroutine(ReduceForceCount(false, 0.2f));
            }
        }

        if (inputData._positivePolarity && hasPositiveMagnet)
        {
            if (polarityPlus && positiveForceCount > 0)
            {
                polarityObject.ApplyPolarity(this, false);
                StartCoroutine(ReduceForceCount(true, 0.2f));
            }
            else if (polarityMinus && positiveForceCount > 0)
            {
                polarityObject.ApplyPolarity(this, true);
                StartCoroutine(ReduceForceCount(true, 0.2f));
            }
        }
    }

    IEnumerator ReduceForceCount(bool forceCounter, float time){
        reduceforceMagnitude = true;
        yield return new WaitForSeconds(time);
        reduceforceMagnitude = false;
        if(forceCounter)
            positiveForceCount--;
        else
            negativeForceCount--;
    }
    IEnumerator EnableCanMove(){
        canMove = false;
        yield return new WaitForSeconds(0.15f);
        canMove = true;  
    }

    public void ApplyForce(Vector3 targetPoint, bool attract)
    {
        particle.Play();
        Vector3 forceDirection = (targetPoint - transform.position).normalized;
        StartCoroutine(EnableCanMove());
        if (!attract)
        {
            forceDirection = -forceDirection;
            if(reduceforceMagnitude)
                rb.AddForce(forceDirection * (forceMagnitude * 0.8f), ForceMode2D.Impulse);
            else
                rb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
        }
        else{
            //rb.velocity = Vector2.zero;
            rb.AddForce(forceDirection * (forceMagnitude * 1.5f), ForceMode2D.Impulse);
        }

    }

    private void SetLineRenderer(Vector3 mousePosition)
    {

        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rotation.eulerAngles = new Vector3(-angle,90,0);
        particle.gameObject.transform.rotation = rotation;
        float distance = direction.magnitude;

        distance = Mathf.Clamp(distance,maxDistance, maxDistance);

        Vector3 clampedPosition = transform.position + direction.normalized * distance;

        hit = Physics2D.Raycast(transform.position, direction.normalized, distance, groundLayer); // Ensure the raycast checks the correct layer
        
        if (hit.collider != null)
        {
            polarityObject = hit.collider.GetComponent<IPolarity>();
            if (polarityObject != null)
            {
                if (polarityObject.isDisabled)
                {
                    myRend.colorGradient = defaultColor;
                    polarityPlus = false;
                    polarityMinus = false;
                }
                else if (polarityObject.polarityPlus && !polarityObject.polarityMinus)
                {
                    myRend.colorGradient = polarityPlusColor;
                    polarityPlus = true;
                    polarityMinus = false;
                }
                else if (!polarityObject.polarityPlus && polarityObject.polarityMinus)
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
        if (!HasInputAuthority) return;
        myRend.SetPosition(0, transform.position);
        myRend.SetPosition(1, clampedPosition);
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

