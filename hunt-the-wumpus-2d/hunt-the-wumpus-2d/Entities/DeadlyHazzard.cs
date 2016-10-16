namespace hunt_the_wumpus_2d.Entities
{
    public abstract class DeadlyHazard : Hazard
    {
        protected DeadlyHazard(int roomNumber) : base(roomNumber)
        {
        }

        public abstract EndState DetermineEndState(int playerRoomNumber);
    }
}