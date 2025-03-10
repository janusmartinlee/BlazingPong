using System.Numerics;

namespace PongEngine;

public class PongGameState
{
    public Arena Arena { get; } = new Arena(1200, 600);

    public Paddle PlayerOne { get; private set; } = new Paddle(Vector2.Zero, 0f, 0f, 0f);

    public Paddle PlayerTwo { get; private set; } = new Paddle(Vector2.Zero, 0f, 0f, 0f);

    public Ball Ball { get; private set; } = new Ball(Vector2.Zero, Vector2.Zero, 0f, null);

    public int PlayerOneScore { get; private set; }

    public int PlayerTwoScore { get; private set; }

    public bool IsGameOver { get; set; }

    internal static PongGameState CreateInitialState()
    {
        return new PongGameState
        {
            PlayerOne = new Paddle(new Vector2(-450, 0), 7f, 10f, 60f),
            PlayerTwo = new Paddle(new Vector2(450, 0), 7f, 10f, 60f),
            Ball = new Ball(Vector2.Zero, new Vector2(10f, 0f), 5f, null)
        };
    }

    public void UpdatePlayerOne(MovementState state)
    {
        PlayerOne = UpdatePlayerMovement(PlayerOne, state);
    }
    public void UpdatePlayerTwo(MovementState state)
    {
        PlayerTwo = UpdatePlayerMovement(PlayerTwo, state);
    }

    internal void ApplyPlayerImpact(Paddle player)
    {
        var newVelocity = CalculatePaddleInpact(Ball, player);
        Ball = Ball with { Velocity = newVelocity, Hitter = player };
    }

    private static Vector2 CalculatePaddleInpact(Ball ball, Paddle paddle)
    {
        // Calculate hit position relative to the paddle center (-1 to 1)
        float relativeHitPosition = (ball.Position.Y - paddle.Position.Y) / (paddle.Height / 2f);

        // Adjust ball's velocity based on hit position
        return new Vector2(-ball.Velocity.X, ball.Velocity.Y + relativeHitPosition * 2f);
    }

    internal void FlipBallHorisontalDirection()
    {
        Ball = Ball with { Velocity = new Vector2(-Ball.Velocity.X, Ball.Velocity.Y) };
    }

    internal void FlipBallViticalDirection()
    {
        Ball = Ball with { Velocity = new Vector2(Ball.Velocity.X, -Ball.Velocity.Y) };
    }

    public Paddle UpdatePlayerMovement(Paddle player, MovementState state)
    {
        if (state is MovementState.MovingUp)
        {
            player = player with
            {
                Position = new Vector2(
                    player.Position.X,
                    Math.Max(player.Position.Y - player.MovementSpeed, -Arena.Height / 2f + player.Height / 2f)
                ),
            };
        }
        else if (state is MovementState.MovingDown)
        {
            player = player with
            {
                Position = new Vector2(
                    player.Position.X,
                    Math.Min(player.Position.Y + player.MovementSpeed, Arena.Height / 2f - player.Height / 2f)
                ),
            };
        }

        return player;
    }

    internal void UpdateBallPosition()
    {
        Ball = Ball with { Position = Ball.Position + Ball.Velocity };
    }
}

public record Ball(Vector2 Position, Vector2 Velocity, float Radius, Paddle? Hitter);

public record Paddle(Vector2 Position, float MovementSpeed, float Width, float Height);

public record Arena(float Width, float Height);