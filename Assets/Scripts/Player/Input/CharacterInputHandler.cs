using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    private NetworkInputData _inputData;

    private bool _isJumpPressed;
    public bool _isActivatePressed;
    private bool _isNegativePolarity;
    private bool _isPositivePolarity;
    private Camera cam;

    void Start()
    {
        _inputData = new NetworkInputData();
    }

    void Update()
    {
        _inputData.movementInput = Input.GetAxisRaw("Horizontal");
        if(cam != null){
            Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            _inputData.mouseInput = mousePosition;
        }

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
    private void FixedUpdate() {
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
}
