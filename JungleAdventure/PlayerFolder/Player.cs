using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JungleAdventure.PlayerFolder
{
    internal class Player
    {
        public int spawnX { get; set; }
        public int spawnY { get; set; }

        public int speed = 3;
        public int height = 55;
        public int width = 32;
        public int jumpTicks = 0;
        public int minJumpTicks = 5;
        public int force = -17;
        public int gravity = 0;
        
        public bool isInvincible = false;
        public bool inAir = true;
        public bool headroom = true;

        public Player() { }
        
        //public Player(int spawnX, int spawnY, int x, int y, Texture2D spriteSheet, Rectangle textureCoordinates)
        //{
        //    zBaseX = x;
        //    zBaseY = y;

        //    this.spriteSheet = spriteSheet;
        //    this.textureCoordinates = textureCoordinates;
        //}
    }
}
