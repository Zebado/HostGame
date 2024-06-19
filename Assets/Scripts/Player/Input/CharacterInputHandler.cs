using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    private NetworkInputData _inputData;

    private bool _isJumpPressed;
    public bool _isActivatePressed;
    private bool _isNegativePolarity;
    private bool _isPositivePolarity;


    void Start()
    {
        _inputData = new NetworkInputData();
    }

    void Update()
    {
        _inputData.movementInput = Input.GetAxis("Horizontal");

        _isJumpPressed |= Input.GetKeyDown(KeyCode.W);

        _isActivatePressed |= Input.GetKeyDown(KeyCode.E);

        if (Input.GetMouseButtonDown(0))
        {
            _isNegativePolarity = true;
        }
        if (Input.GetMouseButtonDown(1))
        {
            _isPositivePolarity = true;
        }
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
}
