namespace PongEngine
{
    public interface IGameEngine
    {
        PongGameState GetState();
        void Update(InputState input);
    }
}