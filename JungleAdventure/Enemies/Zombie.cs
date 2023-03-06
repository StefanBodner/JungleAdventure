using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public int ZombieMovementSpeed(Rectangle block, int worldOffsetX)
        {
            //Check if Zombie touches Ground
            Rectangle groundCheck = new Rectangle(zombieX + zombieWidth / 2 - worldOffsetX, zombieY + zombieHeight, 3, 10);

            if (r.Intersects(block) || !groundCheck.Intersects(block))
            {
                zombieSpeed = -zombieSpeed;
            }

            return zombieSpeed;
        }

        public void DrawZombie(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.spriteSheet, r, this.textureCoordinates, Color.White);
        }
    }
}
