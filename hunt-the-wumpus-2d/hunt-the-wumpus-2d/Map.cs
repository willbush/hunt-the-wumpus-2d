using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hunt_the_wumpus_2d.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d
{
    public class Map
    {
        public const int NumOfRooms = 20;
        private static readonly Logger Log = Logger.Instance;
        private readonly List<DeadlyHazard> _deadlyHazards;
        private readonly List<Entity> _entities;
        private readonly SpriteFont _font;
        private readonly List<Hazard> _hazards;
        private readonly HashSet<int> _roomsWithStaticHazards;
        private readonly Dictionary<int, Vector2> _roomToPosition;
        private readonly List<SuperBats> _superBats;

        public Map(bool isCheatMode, TiledMap tiledMap, SpriteFont font)
        {
            _font = font;
            IsCheatMode = isCheatMode;
            var occupiedRooms = new HashSet<int>();
            _hazards = new List<Hazard>();
            _deadlyHazards = new List<DeadlyHazard>();
            _roomsWithStaticHazards = new HashSet<int>();
            _superBats = new List<SuperBats>();
            _entities = new List<Entity>();
            _roomToPosition = CreateRoomToPosition(tiledMap);

            Player = CreatePlayer(tiledMap, occupiedRooms);
            _entities.Add(Player);

            Wumpus = CreateWumpus(tiledMap, occupiedRooms);
            _entities.Add(Wumpus);
            _hazards.Add(Wumpus);

            // initialize super bats
            InitializeSuperBats(tiledMap, occupiedRooms);

            // initialize bottomless pits
            InitializeBottomlessPits(tiledMap, occupiedRooms);

            if (IsCheatMode)
                PrintHazards();
        }

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

        private void InitializeBottomlessPits(TiledMap tiledMap, ISet<int> occupiedRooms)
        {
            var pitTexture = GetTexture(tiledMap, "pit");
            int pit1RoomNum = GetRandomAvailableRoom(occupiedRooms);
            var pit1 = new BottomlessPit(GetRandomAvailableRoom(occupiedRooms), pitTexture, _roomToPosition[pit1RoomNum]);
            int pit2RoomNum = GetRandomAvailableRoom(occupiedRooms);
            var pit2 = new BottomlessPit(GetRandomAvailableRoom(occupiedRooms), pitTexture, _roomToPosition[pit2RoomNum]);

            _hazards.Add(pit1);
            _hazards.Add(pit2);
            _deadlyHazards.Add(pit1);
            _deadlyHazards.Add(pit2);
            _roomsWithStaticHazards.Add(pit1.RoomNumber);
            _roomsWithStaticHazards.Add(pit2.RoomNumber);
            _entities.Add(pit1);
            _entities.Add(pit2);
        }

        private void InitializeSuperBats(TiledMap tiledMap, ISet<int> occupiedRooms)
        {
            var batTexture = GetTexture(tiledMap, "bat");
            int bat1RoomNum = GetRandomAvailableRoom(occupiedRooms);
            var superbats1 = new SuperBats(GetRandomAvailableRoom(occupiedRooms), batTexture,
                _roomToPosition[bat1RoomNum]);
            int bat2RoomNum = GetRandomAvailableRoom(occupiedRooms);
            var superbats2 = new SuperBats(GetRandomAvailableRoom(occupiedRooms), batTexture,
                _roomToPosition[bat2RoomNum]);

            _superBats.Add(superbats1);
            _superBats.Add(superbats2);
            _hazards.Add(superbats1);
            _hazards.Add(superbats2);
            _roomsWithStaticHazards.Add(superbats1.RoomNumber);
            _roomsWithStaticHazards.Add(superbats2.RoomNumber);
            _entities.Add(superbats1);
            _entities.Add(superbats2);
        }

        private Wumpus CreateWumpus(TiledMap tiledMap, ISet<int> occupiedRooms)
        {
            int wumpusRoomNum = GetRandomAvailableRoom(occupiedRooms);
            return new Wumpus(GetRandomAvailableRoom(occupiedRooms), GetTexture(tiledMap, "wumpus"),
                _roomToPosition[wumpusRoomNum]);
        }

        private Player CreatePlayer(TiledMap tiledMap, ISet<int> occupiedRooms)
        {
            int playerRoomNum = GetRandomAvailableRoom(occupiedRooms);
            return new Player(playerRoomNum, GetTexture(tiledMap, "player"),
                _roomToPosition[playerRoomNum]);
        }

        private Dictionary<int, Vector2> CreateRoomToPosition(TiledMap tiledMap)
        {
            var roomToPosition = new Dictionary<int, Vector2>();
            var rooms = tiledMap.GetObjectGroup("entities").Objects
                .Where(e => e.Type == "room");

            foreach (var room in rooms)
            {
                int key = int.Parse(room.Name);
                roomToPosition.Add(key, new Vector2(room.X, room.Y));
            }
            return roomToPosition;
        }

        private static TextureRegion2D GetTexture(TiledMap tiledMap, string textureType)
        {
            var playerTiledObject = tiledMap.GetObjectGroup("entities").Objects
                .SingleOrDefault(e => e.Type == textureType);

            return tiledMap.GetTileRegion(playerTiledObject?.Gid ?? default(int));
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
            var availableRooms = Enumerable.Range(1, NumOfRooms).Where(r => !occupiedRooms.Contains(r)).ToArray();
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
            Log.Write("");
            Wumpus.Update(this);
            var roomsAdjacentToPlayer = Rooms[Player.RoomNumber];
            _hazards.ForEach(
                h =>
                {
                    if (roomsAdjacentToPlayer.Contains(h.RoomNumber))
                        h.PrintHazardWarning();
                });
            foreach (var e in _entities)
                e.Position = _roomToPosition[e.RoomNumber];

            Player.PrintLocation();
        }

        /// <summary>
        ///     Gets a room number that is adjacent to the given number that's contains no hazards.
        /// </summary>
        public int GetSafeRoomNextTo(int roomNumber)
        {
            var safeAdjacentRooms = Rooms[roomNumber].Except(_roomsWithStaticHazards).ToArray();
            return safeAdjacentRooms.ElementAt(new Random().Next(safeAdjacentRooms.Length));
        }

        /// <summary>
        ///     Performs given command and returns the game end state depending on the results.
        /// </summary>
        /// <param name="command">player's input command</param>
        /// <returns>game end state</returns>
        public void PerformCommand(string command)
        {
            if (command == "M")
            {
                Player.Move();
                CheckPlayerMovement();
            }
            else if (command == "S")
            {
                Player.ShootArrow(Wumpus.RoomNumber);
            }
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

            Log.Write($"Tunnels lead to {sb}");
        }

        public static bool IsAdjacent(int currentRoom, int adjacentRoom)
        {
            return Rooms[currentRoom].Contains(adjacentRoom);
        }

        private void PrintHazards()
        {
            Log.Write("");
            _hazards.ForEach(h => h.PrintLocation());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _entities.ForEach(e => e.Draw(spriteBatch));
            var adjacentRooms = Rooms[Player.RoomNumber];
            foreach (int roomNum in adjacentRooms)
            {
                var position = _roomToPosition[roomNum];
                const int xOffset = 8;
                const int yOffset = 40;
                var v = new Vector2(position.X - xOffset, position.Y - yOffset);
                spriteBatch.DrawString(_font, roomNum.ToString(), v, Color.Red);
            }
        }
    }
}