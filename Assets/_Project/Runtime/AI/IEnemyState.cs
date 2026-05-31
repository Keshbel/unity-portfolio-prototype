namespace ExtractionRoom.AI
{
    public interface IEnemyState
    {
        void Enter();

        void Tick(float deltaTime);
    }
}
