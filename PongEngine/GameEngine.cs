namespace PongEngine;

public class GameEngine : IGameEngine
{
    private readonly IGameMode _gameMode;
    private readonly PongGameState _state;

    public GameEngine(IGameMode gameMode)
    {
        _gameMode = gameMode;
        _state = PongGameState.CreateInitialState();
    }

    public void Update(InputState input)
    {
        _gameMode.Update(_state, input);
    }

    public PongGameState GetState() => _state;
}
