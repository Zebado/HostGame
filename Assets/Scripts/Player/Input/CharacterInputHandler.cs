using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    private NetworkInputData _inputData;

    private bool _isJumpPressed;
    
    void Start()
    {
        _inputData = new NetworkInputData();
    }

    void Update()
    {
        _inputData.movementInput = Input.GetAxis("Horizontal");

        _isJumpPressed |= Input.GetKeyDown(KeyCode.W);

    }

    public NetworkInputData GetLocalInputs()
    {
        
        _inputData.networkButtons.Set(MyButtons.Jump, _isJumpPressed);
        _isJumpPressed = false;
        
        return _inputData;
    }
}
