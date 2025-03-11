namespace PongEngine
{
    public interface IGameEngine
    {
        GameState GetState();
        bool Update(InputState input);
    }
}