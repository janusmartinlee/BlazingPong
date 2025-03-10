using System.Numerics;

namespace PongEngine;

public record InputState
{
    public MovementState PlayerOneMovementState { get; set; }
    public MovementState PlayerTwoMovementState { get; set; }
}
