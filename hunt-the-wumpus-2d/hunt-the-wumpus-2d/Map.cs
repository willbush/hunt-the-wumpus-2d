using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace hunt_the_wumpus_2d
{
    internal class Map : Entity
    {
        private const int NumOfRooms = 20;
        private readonly ContentManager _content;
        private readonly List<Room> _rooms;
        private Texture2D _rectangle;


        public Map(ContentManager content)
        {
            _content = content;
            _rooms = new List<Room>(NumOfRooms);
        }

        // Each key is the room number and its value is the set of adjacent rooms.
        // A dictionary of hash sets is definitely overkill given the constant number of elements, 
        // but with it comes a lot of Linq expression convenience. 
        internal static Dictionary<int, HashSet<int>> RoomMap { get; } = new Dictionary<int, HashSet<int>>
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

        public void LoadContent(GraphicsDevice device)
        {
            _rectangle = new Texture2D(device, 1, 1);
            _rectangle.SetData(new[] {Color.White});
            InitializeRooms();
        }

        private void InitializeRooms()
        {
            const int rows = 4;
            const int columns = 5;
            int roomNumber = 1;
            const int separationOffset = Room.SideLen + 20;
            const int xOffset = 500;
            const int yOffset = 200;

            for (int col = 0; col < columns; ++col)
            {
                for (int row = 0; row < rows; ++row)
                {
                    _rooms.Add(new Room.Builder
                    {
                        RoomNumber = roomNumber,
                        AdjacentRooms = RoomMap[roomNumber],
                        Content = _content,
                        Position = new Vector2(row * separationOffset + xOffset, col * separationOffset + yOffset),
                        Texture = _rectangle
                    }.Build());
                    ++roomNumber;
                }
            }
            _rooms.ForEach(r => r.LoadContent());
        }


        public override void UnloadContent()
        {
            _rectangle.Dispose();
        }

        public override void Update(GameTime time)
        {
        }

        public override void Draw(SpriteBatch batch)
        {
            _rooms.ForEach(r => r.Draw(batch));
        }
    }
}