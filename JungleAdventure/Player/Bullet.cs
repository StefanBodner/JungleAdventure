using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JungleAdventure.Player
{
    internal class Bullet
    {
        public int x { get; set; }
        public int y { get; set; }


        public int bulletWidth = 8;
        public int bulletHeight = 2;
        public int bulletSpeed = 6;

        public Texture2D spriteSheet;
        public Rectangle r;
        public Rectangle textureCoordinates;

        //Constructor
        public Bullet() { }
        public Bullet(int x, int y, Texture2D spriteSheet, Rectangle textureCoordinates, bool shootingDirectionRight)
        {
            this.x = x;
            this.y = y;

            this.spriteSheet = spriteSheet;
            this.textureCoordinates = textureCoordinates;

            if (!shootingDirectionRight)
            {
                bulletSpeed = -bulletSpeed;
            }
        }

        public void DrawBullet(SpriteBatch spriteBatch, int worldOffsetX)
        {
            x += bulletSpeed;
            r = new Rectangle(x + worldOffsetX, y, bulletWidth, bulletHeight);
            spriteBatch.Draw(this.spriteSheet, r, this.textureCoordinates, Color.White);
        }
    }
}
