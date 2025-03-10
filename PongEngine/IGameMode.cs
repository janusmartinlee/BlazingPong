namespace PongEngine;

public interface IGameMode
{
    void Update(PongGameState state, InputState input);
}

