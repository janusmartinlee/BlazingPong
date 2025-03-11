namespace PongEngine;

public class GameService
{
    private IGameEngine? _engine;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(16)); // ~60 FPS
    private InputState _input = new();
    private GameExecutionState _state = GameExecutionState.Uninitialized;
    private GameMode _gameMode;

    public event Action? OnStateChanged;

    public GameService()
    {
        
    }

    public void InitializeGame(GameMode gameMode)
    {
        _gameMode = gameMode;
        _engine = gameMode switch
        {
            GameMode.PlayerVsBot => new PlayerVsBotMode(),
            _ => throw new NotSupportedException("Unsupported game mode")
        };

        _state = GameExecutionState.Initialized;
    }

    public void StartGame()
    {
        if (_state != GameExecutionState.Initialized)
        {
            throw new InvalidOperationException("Game must be initialized before starting");
        }
        
        RunGameLoop();
    }

    public void RestartGame()
    {
        if (_state != GameExecutionState.GameOver)
        {
            throw new InvalidOperationException("Game must be over before restarting");
        }

        InitializeGame(_gameMode);
        StartGame();
    }

    public void UpdateInput(InputState input) => _input = input;

    public GameState GetState() => _engine?.GetState() ?? GameState.DefaultState;

    private async void RunGameLoop()
    {
        if (_state is not GameExecutionState.Initialized)
            throw new InvalidOperationException($"Game can not run from the current state: {_state}");

        if (_engine is null)
            throw new InvalidOperationException("Game engine is not initialized");

        _state = GameExecutionState.Running;

        try
        {
            while (_state is GameExecutionState.Running && await _timer.WaitForNextTickAsync())
            {
                var isGameOver = _engine.Update(_input); // TODO: Pass actual input
                if (isGameOver)
                {
                    _state = GameExecutionState.GameOver;
                }
                OnStateChanged?.Invoke();
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
}

