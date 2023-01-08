using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JungleAdventure.Player
{
    internal class Player
    {
        public int playerX { get; set; }
        public int playerY { get; set; }
        
        public int playerWidth = 32;
        public int playerHeight = 32;

        public Texture2D spriteSheet;
        public Rectangle r;
        public Rectangle textureCoordinates;

        //Constructor
        public Player() { }
        public Player(int x, int y, Texture2D spriteSheet, Rectangle textureCoordinates)
        {
            playerX = x;
            playerY = y;

            r = new Rectangle(x, y, playerWidth, playerHeight);

            this.spriteSheet = spriteSheet;
            this.textureCoordinates = textureCoordinates;
        }

        public void DrawBlock(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.spriteSheet, r, this.textureCoordinates, Color.White);
        }
    }
}
