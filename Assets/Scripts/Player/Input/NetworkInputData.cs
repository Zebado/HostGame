using Fusion;

public struct NetworkInputData : INetworkInput
{
    public float movementInput;
    public NetworkBool _negativePolarity; 
    public NetworkBool _positivePolarity; 
    public NetworkButtons networkButtons;
}

enum MyButtons
{
    Jump = 0
}