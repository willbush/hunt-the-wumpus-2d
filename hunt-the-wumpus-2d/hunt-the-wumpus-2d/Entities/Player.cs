using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d.Entities
{
    public class Player : Entity
    {
        private const int MaxNumberOfArrows = 5;
        private static readonly Logger Log = Logger.Instance;
        private static readonly InputManager Input = InputManager.Instance;
        private readonly int _initialRoomNum;

        public Player(int roomNumber, TextureRegion2D texture, Vector2 position) : base(roomNumber, texture, position)
        {
            _initialRoomNum = roomNumber;
        }

        public int MaxArrows { get; } = MaxNumberOfArrows;
        public int CrookedArrowCount { get; private set; } = MaxNumberOfArrows;

        /// <summary>
        ///     Requests where the player wants to move to, validates the input, and moves the player.
        /// </summary>
        public void Move()
        {
            Log.Write("Where to? (type the number and hit enter");
            WumpusGame.State = WumpusGame.GameState.PlayerMove;

            Input.AddTypedActionPrompt(response =>
            {
                int adjacentRoom;
                if (!int.TryParse(response, out adjacentRoom) || !Map.IsAdjacent(RoomNumber, adjacentRoom))
                {
                    Log.Write($"{response} Not Possible - Where to? ");
                }
                else
                {
                    RoomNumber = adjacentRoom;
                    WumpusGame.State = WumpusGame.GameState.Playing;
                }
            });
        }

        internal void Move(int roomNumber)
        {
            RoomNumber = roomNumber;
        }

        /// <summary>
        ///     Requests the player how many and what rooms the arrow should traverse.
        ///     The arrow traverses the traversable rooms or randomly selected adjacent ones and the
        ///     arrows traversal path is checked for a hit to determine end game state.
        /// </summary>
        /// <param name="wumpusRoomNumber">current wumpus room number</param>
        /// <returns>game end state result</returns>
        public void ShootArrow(int wumpusRoomNumber)
        {
            PromptForRoomsToTraverse(wumpusRoomNumber);
        }

        // Traverses the given rooms or randomly selected adjacent rooms if the given rooms are not traversable.
        // Checks if the arrow hit the player, wumpus, or was a miss, and game state is set accordingly.
        private void ShootArrow(IReadOnlyCollection<int> roomsToTraverse, int wumpusRoomNum)
        {
            CrookedArrowCount = CrookedArrowCount - 1;
            var endstate = Traverse(roomsToTraverse).Select(r => HitTarget(r, wumpusRoomNum))
                .FirstOrDefault(e => e.IsGameOver);

            if (endstate != null && endstate.IsGameOver)
            {
                endstate.Print();
                Log.Write(Message.PlayPrompt);
                WumpusGame.State = WumpusGame.GameState.GameOver;
                return;
            }

            Log.Write(Message.Missed);
            if (CrookedArrowCount == 0)
            {
                Log.Write($"{Message.OutOfArrows}\n{Message.LoseMessage}");
                WumpusGame.State = WumpusGame.GameState.GameOver;
            }
            else
            {
                WumpusGame.State = WumpusGame.GameState.Playing;
            }
        }

        private EndState HitTarget(int currentRoom, int wumpusRoomNum)
        {
            Log.Write(currentRoom.ToString());
            EndState endState;
            if (RoomNumber == currentRoom)
            {
                endState = new EndState(true,
                    $"{Message.ArrowGotYou}\n{Message.LoseMessage}");
            }
            else if (wumpusRoomNum == currentRoom)
            {
                endState = new EndState(true, Message.WinMessage);
            }
            else
            {
                endState = new EndState();
            }
            return endState;
        }

        // Attempts to traverse the requested rooms to traverse, but as soon as one
        // requested room is not adjacent to the current room, it starts traversing rooms randomly.
        private IEnumerable<int> Traverse(IReadOnlyCollection<int> roomsToTraverse)
        {
            int currentRoom = RoomNumber;

            ICollection<int> traversedRooms = roomsToTraverse.TakeWhile(
                nextRoom =>
                {
                    var adjacentRooms = Map.Rooms[currentRoom];
                    if (!adjacentRooms.Contains(nextRoom)) return false;
                    currentRoom = nextRoom;
                    return true;
                }).ToList();

            int numLeftToTraverse = roomsToTraverse.Count - traversedRooms.Count;
            RandomlyTraverse(traversedRooms, currentRoom, numLeftToTraverse);
            return traversedRooms;
        }

        // Adds to the given list of traversed rooms a randomly selected next adjacent room where
        // said selected room is not the previously traversed room (preventing U-turns).
        private static void RandomlyTraverse(
            ICollection<int> traversedRooms,
            int currentRoom,
            int numberToTraverse)
        {
            int previousRoom;

            // if no traversed rooms, randomly select an adjacent next room and set the previous to the room the player is in.
            if (!traversedRooms.Any())
            {
                var rooms = Map.Rooms[currentRoom];
                int firstRoom = rooms.ElementAt(new Random().Next(rooms.Count));

                previousRoom = currentRoom;
                currentRoom = firstRoom;
            }
            else
            {
                previousRoom = currentRoom;
            }

            // while we need more rooms, get a randomly selected adjacent room that is not the previously traversed room.
            for (var traversed = 0; traversed < numberToTraverse; ++traversed)
            {
                var rooms = Map.Rooms[currentRoom].Where(r => r != previousRoom).ToArray();
                int nextRoom = rooms.ElementAt(new Random().Next(rooms.Length));

                traversedRooms.Add(currentRoom);
                previousRoom = currentRoom;
                currentRoom = nextRoom;
            }
        }

        // Requests the player to give the list of rooms they want the arrow to traverse.
        private void PromptForRoomsToTraverse(int wumpusRoomNum)
        {
            Log.Write("Please enter a space separated list of rooms");
            Log.Write("(e.g. 1 2 3 4 5) in the range of [0, 5] rooms.");

            Input.AddTypedActionPrompt(response =>
            {
                var rooms = new List<int>();

                string[] elements = response.Split(' ');

                if (elements.Length < 0 || elements.Length > 5) return;

                foreach (string element in elements)
                {
                    int roomNumber;
                    if (!int.TryParse(element, out roomNumber) || roomNumber < 0 || roomNumber > Map.NumOfRooms)
                    {
                        Log.Write($"{element} is a Bad number - try again:");
                        return;
                    }
                    if (IsTooCrooked(roomNumber, rooms))
                    {
                        Log.Write(Message.TooCrooked);
                        return;
                    }
                    rooms.Add(roomNumber);
                }
                ShootArrow(rooms, wumpusRoomNum);
            });
        }

        // A requested room number is too crooked for an arrow to go into when:
        // The requested room is the same as the previously requested room
        // (essentially asking the arrow to stay in the same room).
        // The requested room is the same as request before last.
        // (essentially asking for the arrow to make a U-turn).
        private static bool IsTooCrooked(int roomNumber, IReadOnlyList<int> rooms)
        {
            return (rooms.Count > 0 && rooms.Last() == roomNumber) ||
                   (rooms.Count > 1 && rooms[rooms.Count - 2] == roomNumber);
        }

        public override void Update(GameTime time)
        {
            throw new NotImplementedException();
        }

        public override void PrintLocation()
        {
            Log.Write($"You are in room {RoomNumber}");
            Map.PrintAdjacentRoomNumbers(RoomNumber);
        }

        /// <summary>
        ///     Resets player to initial state.
        /// </summary>
        public void Reset()
        {
            RoomNumber = _initialRoomNum;
            CrookedArrowCount = MaxArrows;
        }
    }
}