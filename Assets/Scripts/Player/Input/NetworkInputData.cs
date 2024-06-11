using Fusion;

public struct NetworkInputData : INetworkInput
{
    public float movementinput;
    public NetworkBool isJumpPressed;
    public NetworkBool magnetPositive;
    public NetworkBool magnetNegative;
    public NetworkBool interact;
}
