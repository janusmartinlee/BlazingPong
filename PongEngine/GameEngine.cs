using System.Numerics;

namespace PongEngine;

public abstract class GameEngine : IGameEngine
{
    private readonly Arena _arena = new(1000, 500);

    private Player _playerOne;

    private Player _playerTwo;

    private Ball _ball;

    private bool _isGameOver;

    private const int _scoreToWin = 10;

    public GameEngine()
    {
        _playerOne = new Player(new Vector2(-450, 0), 7f, 10f, 60f, "Player 1");
        _playerTwo = new Player(new Vector2(450, 0), 7f, 10f, 60f, "Player 2");
        _ball = new Ball(Vector2.Zero, new Vector2(10f, 0f), 5f, null);
    }

    public virtual bool Update(InputState input)
    {
        _ball = _ball.UpdatePosition();
        _playerOne = _playerOne.UpdatePosition(_arena, input.PlayerOneMovementState);
        _playerTwo = _playerTwo.UpdatePosition(_arena, input.PlayerTwoMovementState);

        if (IsBallCollidingWithLeft(_ball, _arena))
        {
            _ball = _ball.ResetPosition().ResetSize().ResetVelocity();
            _playerTwo = _playerTwo.ScorePoint().ResetPosition().ResetSize();
            _playerOne = _playerOne.ResetPosition().ResetSize();
            _isGameOver = _playerTwo.Score >= _scoreToWin;
            return _isGameOver;
        }

        if (IsBallCollidingWithRigth(_ball, _arena))
        {
            _ball = _ball.ResetPosition().ResetSize().ResetVelocity();
            _playerOne = _playerOne.ScorePoint().ResetPosition().ResetSize();
            _playerTwo = _playerTwo.ResetPosition().ResetSize();
            _isGameOver = _playerOne.Score >= _scoreToWin;
            return _isGameOver;
        }

        if (IsBallCollidingWithTop(_ball, _arena) || IsBallCollidingWithBottom(_ball, _arena))
        {
            _ball = _ball.FlipVitically();
        }

        if (IsBallCollidingWithPaddle(_ball, _playerOne)) ApplyPlayerImpact(_playerOne);
        else if (IsBallCollidingWithPaddle(_ball, _playerTwo)) ApplyPlayerImpact(_playerTwo);

        return _isGameOver;
    }

    private void ApplyPlayerImpact(Player player)
    {
        var newVelocity = CalculatePaddleInpact(_ball, player);
        _ball = _ball with { Velocity = newVelocity, Hitter = player };
    }

    private static Vector2 CalculatePaddleInpact(Ball ball, Player paddle)
    {
        // Calculate hit position relative to the paddle center (-1 to 1)
        float relativeHitPosition = (ball.Position.Y - paddle.Position.Y) / (paddle.Height / 2f);

        // Adjust ball's velocity based on hit position
        return new Vector2(-ball.Velocity.X, ball.Velocity.Y + relativeHitPosition * 2f);
    }

    private static bool IsBallCollidingWithPaddle(Ball ball, Player paddle)
    {
        var ballLeft = ball.Position.X - ball.Radius;
        var ballRight = ball.Position.X + ball.Radius;
        var ballTop = ball.Position.Y - ball.Radius;
        var ballBottom = ball.Position.Y + ball.Radius;

        var paddleLeft = paddle.Position.X - paddle.Width / 2f;
        var paddleRight = paddle.Position.X + paddle.Width / 2f;
        var paddleTop = paddle.Position.Y - paddle.Height / 2f;
        var paddleBottom = paddle.Position.Y + paddle.Height / 2f;

        return ballRight >= paddleLeft &&
               ballLeft <= paddleRight &&
               ballBottom >= paddleTop &&
               ballTop <= paddleBottom;
    }

    private static bool IsBallCollidingWithTop(Ball ball, Arena surfaceSize)
    {
        return ball.Position.Y - ball.Radius <= -surfaceSize.Height / 2f;
    }

    private static bool IsBallCollidingWithBottom(Ball ball, Arena surfaceSize)
    {
        return ball.Position.Y + ball.Radius >= surfaceSize.Height / 2f;
    }

    private static bool IsBallCollidingWithLeft(Ball ball, Arena surfaceSize)
    {
        return ball.Position.X - ball.Radius <= -surfaceSize.Width / 2f;
    }

    private static bool IsBallCollidingWithRigth(Ball ball, Arena surfaceSize)
    {
        return ball.Position.X + ball.Radius >= surfaceSize.Width / 2f;
    }

    public GameState GetState() => new(_arena, _playerOne, _playerTwo, _ball, _isGameOver);
}

public record GameState(Arena Arena, Player PlayerOne, Player PlayerTwo, Ball Ball, bool IsGameOver)
{
    public static GameState DefaultState => new(new Arena(1000, 500), new Player(Vector2.Zero, 0f, 0f, 0f, "Player 1"), new Player(Vector2.Zero, 0f, 0f, 0f, "Player 2"), new Ball(Vector2.Zero, Vector2.Zero, 0f, null), false);
}

public readonly record struct Ball(Vector2 Position, Vector2 Velocity, float Radius, Player? Hitter)
{
    internal Ball UpdatePosition()
    {
        return this with { Position = Position + Velocity };
    }

    internal Ball FlipVitically()
    {
        return this with { Velocity = new Vector2(Velocity.X, -Velocity.Y) };
    }

    internal Ball ResetPosition()
    {
        return this with { Position = Vector2.Zero };
    }

    internal Ball ResetSize()
    {
        return this with { Radius = 5f };
    }

    internal Ball ResetVelocity()
    {
        return this with { Velocity = new Vector2(10f, 0f) };
    }
}

public readonly record struct Player(Vector2 Position, float MovementSpeed, float Width, float Height, string Name)
{
    private const float PaddleWidth = 10f;
    private const float DefaultHeight = 60f;

    public readonly int Score { get; init; }

    internal Player ResetSize()
    {
        return this with { Width = PaddleWidth, Height = DefaultHeight };
    }

    internal Player ResetPosition()
    {
        return this with { Position = new Vector2(Position.X, 0) };
    }
    
    internal Player ScorePoint()
    {
        return this with { Score = Score + 1 };
    }

    internal Player UpdatePosition(Arena arena, MovementState state)
    {
        if (state is MovementState.MovingUp)
        {
            return this with
            {
                Position = new Vector2(
                    Position.X,
                    Math.Max(Position.Y - MovementSpeed, -arena.Height / 2f + Height / 2f)
                ),
            };
        }
        else if (state is MovementState.MovingDown)
        {
            return this with
            {
                Position = new Vector2(
                    Position.X,
                    Math.Min(Position.Y + MovementSpeed, arena.Height / 2f - Height / 2f)
                ),
            };
        }
        else
        {
            return this;
        }
    }
}

public readonly record struct Arena(float Width, float Height);