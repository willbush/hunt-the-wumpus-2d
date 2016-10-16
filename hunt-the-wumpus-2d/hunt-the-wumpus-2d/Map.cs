using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hunt_the_wumpus_2d.Entities;

namespace hunt_the_wumpus_2d
{
    public class Map
    {
        public const int NumOfRooms = 20;
        private readonly List<DeadlyHazard> _deadlyHazards;
        private readonly List<Hazard> _hazards;
        private readonly HashSet<int> _roomsWithStaticHazards;
        private readonly List<SuperBats> _superBats;
        public bool IsCheatMode { get; }
        public Player Player { get; }
        private Wumpus Wumpus { get; }

        // Each key is the room number and its value is the set of adjacent rooms.
        // A dictionary of hash sets is definitely overkill given the constant number of elements, 
        // but with it comes a lot of Linq expression convenience. 
        internal static Dictionary<int, HashSet<int>> Rooms { get; } = new Dictionary<int, HashSet<int>>
        {
            {1, new HashSet<int> {2, 5, 8}},
            {2, new HashSet<int> {1, 3, 10}},
            {3, new HashSet<int> {2, 4, 12}},
            {4, new HashSet<int> {3, 5, 14}},
            {5, new HashSet<int> {1, 4, 6}},
            {6, new HashSet<int> {5, 7, 15}},
            {7, new HashSet<int> {6, 8, 17}},
            {8, new HashSet<int> {1, 7, 9}},
            {9, new HashSet<int> {8, 10, 18}},
            {10, new HashSet<int> {2, 9, 11}},
            {11, new HashSet<int> {10, 12, 19}},
            {12, new HashSet<int> {3, 11, 13}},
            {13, new HashSet<int> {12, 14, 20}},
            {14, new HashSet<int> {4, 13, 15}},
            {15, new HashSet<int> {6, 14, 16}},
            {16, new HashSet<int> {15, 17, 20}},
            {17, new HashSet<int> {7, 16, 18}},
            {18, new HashSet<int> {9, 17, 19}},
            {19, new HashSet<int> {11, 18, 20}},
            {20, new HashSet<int> {13, 16, 19}}
        };

        public Map(bool isCheatMode)
        {
            IsCheatMode = isCheatMode;
            var occupiedRooms = new HashSet<int>();

            Player = new Player(GetRandomAvailableRoom(occupiedRooms));
            Wumpus = new Wumpus(GetRandomAvailableRoom(occupiedRooms));

            var bottomlessPit1 = new BottomlessPit(GetRandomAvailableRoom(occupiedRooms));
            var bottomlessPit2 = new BottomlessPit(GetRandomAvailableRoom(occupiedRooms));
            var superbats1 = new SuperBats(GetRandomAvailableRoom(occupiedRooms));
            var superbats2 = new SuperBats(GetRandomAvailableRoom(occupiedRooms));

            _hazards = new List<Hazard> {Wumpus, bottomlessPit1, bottomlessPit2, superbats1, superbats2};
            _deadlyHazards = new List<DeadlyHazard> {Wumpus, bottomlessPit1, bottomlessPit2};
            _superBats = new List<SuperBats> {superbats1, superbats2};

            _roomsWithStaticHazards = new HashSet<int>
            {
                superbats1.RoomNumber,
                superbats2.RoomNumber,
                bottomlessPit1.RoomNumber,
                bottomlessPit2.RoomNumber
            };

            if (IsCheatMode)
                PrintHazards();
        }

        /// <summary>
        ///     Reset map state to its initial state.
        /// </summary>
        public void Reset()
        {
            Player.Reset();
            Wumpus.Reset();

            if (IsCheatMode)
                PrintHazards();
        }

        /// <summary>
        ///     Gets a random available room that's not a member of the give4n occupied rooms set.
        /// </summary>
        private static int GetRandomAvailableRoom(ISet<int> occupiedRooms)
        {
            int[] availableRooms = Enumerable.Range(1, NumOfRooms).Where(r => !occupiedRooms.Contains(r)).ToArray();
            if (availableRooms.Length == 0)
                throw new InvalidOperationException("All rooms are already occupied.");

            int index = new Random().Next(0, availableRooms.Length);
            int unoccupiedRoom = availableRooms[index];
            occupiedRooms.Add(unoccupiedRoom);
            return unoccupiedRoom;
        }

        public static int GetAnyRandomRoomNumber()
        {
            return new Random().Next(1, NumOfRooms + 1); // Random number in range [1, 20]
        }

        /// <summary>
        ///     Updates the state of the game on the map.
        /// </summary>
        public void Update()
        {
            Console.WriteLine();
            Wumpus.Update(this);

            HashSet<int> roomsAdjacentToPlayer = Rooms[Player.RoomNumber];
            _hazards.ForEach(
                h =>
                {
                    if (roomsAdjacentToPlayer.Contains(h.RoomNumber))
                        h.PrintHazardWarning();
                });
            Player.PrintLocation();
        }

        /// <summary>
        ///     Gets a room number that is adjacent to the given number that's contains no hazards.
        /// </summary>
        public int GetSafeRoomNextTo(int roomNumber)
        {
            int[] safeAdjacentRooms = Rooms[roomNumber].Except(_roomsWithStaticHazards).ToArray();
            return safeAdjacentRooms.ElementAt(new Random().Next(safeAdjacentRooms.Length));
        }

        /// <summary>
        ///     Performs given command and returns the game end state depending on the results.
        /// </summary>
        /// <param name="command">player's input command</param>
        /// <returns>game end state</returns>
        public EndState GetEndState(string command)
        {
            EndState endState;
            switch (command)
            {
                case "M":
                    Player.Move();
                    endState = CheckPlayerMovement();
                    break;
                case "S":
                    endState = Player.ShootArrow(Wumpus.RoomNumber);
                    break;
                case "Q":
                    endState = new EndState(true, "");
                    break;
                default:
                    endState = new EndState();
                    break;
            }
            return endState;
        }

        // Game is over if the player moves into a deadly room.
        // If the game's not over but the power got snatched, then loop and check again until the player
        // doesn't get snatched or gets killed.
        private EndState CheckPlayerMovement()
        {
            EndState endState;
            do
            {
                endState = _deadlyHazards
                    .Select(h => h.DetermineEndState(Player.RoomNumber))
                    .FirstOrDefault(s => s.IsGameOver) ?? new EndState();
            } while (!endState.IsGameOver && _superBats.Any(b => b.TrySnatch(Player)));
            return endState;
        }

        public static void PrintAdjacentRoomNumbers(int roomNum)
        {
            var sb = new StringBuilder();
            foreach (int room in Rooms[roomNum])
                sb.Append(room + " ");

            Console.WriteLine($"Tunnels lead to {sb}");
        }

        public static bool IsAdjacent(int currentRoom, int adjacentRoom)
        {
            return Rooms[currentRoom].Contains(adjacentRoom);
        }

        private void PrintHazards()
        {
            Console.WriteLine();
            _hazards.ForEach(h => h.PrintLocation());
        }
    }
}