using System.Numerics;

namespace PongEngine;

public class PlayerVsBotMode : IGameMode
{
    public void Update(PongGameState state, InputState input)
    {
        state.UpdateBallPosition();

        // Player 1
        state.UpdatePlayerOne(input.PlayerOneMovementState);

        // Player 2 (bot)
        state.UpdatePlayerTwo(0f);

        // Check for collision with the edges (left or right)
        if (IsBallCollidingWithLeft(state.Ball, state.Arena) ||
            IsBallCollidingWithRigth(state.Ball, state.Arena))
        {
            state.FlipBallHorisontalDirection();
        }

        // Check for top/bottom boundary collision
        if (IsBallCollidingWithTop(state.Ball, state.Arena) ||
            IsBallCollidingWithBottom(state.Ball, state.Arena))
        {
            state.FlipBallViticalDirection();
        }

        if (IsBallCollidingWithPaddle(state.Ball, state.PlayerOne))
        {
            state.ApplyPlayerImpact(state.PlayerOne);
        }
        else if (IsBallCollidingWithPaddle(state.Ball, state.PlayerTwo))
        {
            state.ApplyPlayerImpact(state.PlayerTwo);
        }
    }

    private bool IsBallCollidingWithPaddle(Ball ball, Paddle paddle)
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

    private bool IsBallCollidingWithTop(Ball ball, Arena surfaceSize)
    {
        return ball.Position.Y - ball.Radius <= -surfaceSize.Height / 2f;
    }

    private bool IsBallCollidingWithBottom(Ball ball, Arena surfaceSize)
    {
        return ball.Position.Y + ball.Radius >= surfaceSize.Height / 2f;
    }

    private bool IsBallCollidingWithLeft(Ball ball, Arena surfaceSize)
    {
        return ball.Position.X - ball.Radius <= -surfaceSize.Width / 2f;
    }

    private bool IsBallCollidingWithRigth(Ball ball, Arena surfaceSize)
    {
        return ball.Position.X + ball.Radius >= surfaceSize.Width / 2f;
    }
}
