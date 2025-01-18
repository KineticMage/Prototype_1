namespace CarScraper.Environment
{
    public interface IDynamicObstacle
    {
        void TickUpdate(float time, float delta);
        void TickFixedUpdate(float delta);
    }
}
