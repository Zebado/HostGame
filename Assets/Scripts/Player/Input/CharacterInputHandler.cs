using Fusion;
using UnityEngine;

public class CharacterInputHandler : NetworkBehaviour
{
    private NetworkInputData _inputData;

    private bool _isJumpPressed;
    public bool _isActivatePressed;
    private bool _isNegativePolarity;
    private bool _isPositivePolarity;
    private Camera cam;

    AudioSource _audioSource;
    public AudioClip magneticClip;
    void Start()
    {
        _inputData = new NetworkInputData();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        _inputData.movementInput = Input.GetAxisRaw("Horizontal");
        if (cam != null)
        {
            Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            _inputData.mouseInput = mousePosition;
        }

        _isJumpPressed |= Input.GetKeyDown(KeyCode.W);

        _isActivatePressed |= Input.GetKeyDown(KeyCode.E);

        if (Input.GetMouseButtonDown(0))
        {
            _isNegativePolarity = true;
            if (Object.HasStateAuthority)
                RpcPlayMagneticSound();
        }
        if (Input.GetMouseButtonDown(1))
        {
            _isPositivePolarity = true;
            if (Object.HasStateAuthority)
                RpcPlayMagneticSound();
        }
    }
    private void FixedUpdate()
    {
        cam = Camera.main;
    }

    public NetworkInputData GetLocalInputs()
    {
        _inputData._negativePolarity = _isNegativePolarity;
        _isNegativePolarity = false;

        _inputData._positivePolarity = _isPositivePolarity;
        _isPositivePolarity = false;

        _inputData.networkButtons.Set(MyButtons.Jump, _isJumpPressed);
        _isJumpPressed = false;

        _inputData.networkButtons.Set(MyButtons.Activate, _isActivatePressed);
        _isActivatePressed = false;

        return _inputData;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcPlayMagneticSound()
    {
        if (_audioSource != null && magneticClip != null)
        {
            Debug.Log("plaaaay");
            _audioSource.PlayOneShot(magneticClip);
        }
    }
}
