using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    NetworkInputData _inputData;

    bool _isJumpPressed;

    void Start()
    {
        _inputData = new NetworkInputData();
    }

    void Update()
    {
        _inputData.movementinput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.W))
        {
            _isJumpPressed = true;
        }
    }
    public NetworkInputData GetLocalInputs()
    {

        _inputData.isJumpPressed = _isJumpPressed;
        _isJumpPressed = false;

        return _inputData;
    }
}
