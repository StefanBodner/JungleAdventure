using JungleAdventure.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace JungleAdventure.Enemies
{
    internal class Zombie
    {
        public int zombieX { get; set; }
        public int zombieY { get; set; }

        public int zombieWidth = 32;
        public int zombieHeight = 32;
        public int zombieSpeed = 2;

        public Texture2D spriteSheet;
        public Rectangle r;
        public Rectangle textureCoordinates;

        //Constructor
        public Zombie() { }
        public Zombie(int x, int y, Texture2D spriteSheet, Rectangle textureCoordinates)
        {
            zombieX = x;
            zombieY = y;

            r = new Rectangle(x, y, zombieWidth, zombieHeight);

            this.spriteSheet = spriteSheet;
            this.textureCoordinates = textureCoordinates;
        }

        public void DrawZombie(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.spriteSheet, r, this.textureCoordinates, Color.White);
        }
    }
}
