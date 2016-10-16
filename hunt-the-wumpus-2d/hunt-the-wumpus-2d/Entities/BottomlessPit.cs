﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace hunt_the_wumpus_2d.Entities
{
    public class BottomlessPit : DeadlyHazard
    {
        public BottomlessPit(int roomNumber) : base(roomNumber)
        {
        }

        public override void Update(GameTime time)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch batch)
        {
            throw new NotImplementedException();
        }

        public override void PrintLocation()
        {
            Console.WriteLine($"Bottomless pit in room {RoomNumber}");
        }

        public override void PrintHazardWarning()
        {
            Console.WriteLine(Message.PitWarning);
        }

        public override EndState DetermineEndState(int playerRoomNumber)
        {
            return playerRoomNumber == RoomNumber
                ? new EndState(true, $"{Message.FellInPit}\n{Message.LoseMessage}")
                : new EndState();
        }
    }
}