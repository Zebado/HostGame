using Fusion;

public interface IPolarity
{
    [Networked] public bool polarityPlus { get; set; }
    [Networked] public bool polarityMinus { get; set; }
    [Networked] public bool isDisabled { get; set; }

    public void ApplyPolarity(NewCharacterController player, bool attract);
}
