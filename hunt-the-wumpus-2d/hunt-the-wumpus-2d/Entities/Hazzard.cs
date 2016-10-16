namespace hunt_the_wumpus_2d.Entities
{
    public abstract class Hazard : Entity
    {
        protected Hazard(int roomNumber) : base(roomNumber)
        {
        }

        public abstract void PrintHazardWarning();
    }
}