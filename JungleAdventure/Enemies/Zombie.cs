using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JungleAdventure.Enemies
{
    internal class Zombie
    {
        public int zBaseX { get; set; }
        public int zBaseY { get; set; }
        public int awayFromBaseXCoordinate { get; set; }

        
        public int zombieWidth = 32;
        public int zombieHeight = 42;
        public int zombieSpeed = 2;
        
        public Texture2D spriteSheet;
        public Rectangle r;
        public Rectangle textureCoordinates;

        //Constructor
        public Zombie() { }
        public Zombie(int x, int y, Texture2D spriteSheet, Rectangle textureCoordinates)
        {
            zBaseX = x;
            zBaseY = y;

            this.spriteSheet = spriteSheet;
            this.textureCoordinates = textureCoordinates;
        }

        public void DrawZombie(SpriteBatch spriteBatch, int worldOffsetX)
        {
            r = new Rectangle(zBaseX + awayFromBaseXCoordinate + worldOffsetX, zBaseY, zombieWidth, zombieHeight);

            if(zombieSpeed < 0)
            {
                //Draw Zombie mirrored
                spriteBatch.Draw(this.spriteSheet, r, this.textureCoordinates, Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            }
            else
            {
                spriteBatch.Draw(this.spriteSheet, r, this.textureCoordinates, Color.White);
            }

            
        }
    }
}
