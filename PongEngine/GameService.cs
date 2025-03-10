namespace PongEngine;

public class GameService : IAsyncDisposable
{
    private readonly GameEngine _engine;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(16)); // ~60 FPS
    private bool _isRunning = true;
    private InputState _input = new();

    public event Action? OnStateChanged;

    public GameService()
    {
        _engine = new GameEngine(new PlayerVsBotMode());
        _ = RunGameLoop();
    }

    public void UpdateInput(InputState input) => _input = input;

    public PongGameState GetState() => _engine.GetState();

    private async Task RunGameLoop()
    {
        while (_isRunning && await _timer.WaitForNextTickAsync())
        {
            _engine.Update(_input); // TODO: Pass actual input
            OnStateChanged?.Invoke();
        }
    }

    public async ValueTask DisposeAsync()
    {
        _isRunning = false;
    }
}

