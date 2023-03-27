using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JungleAdventure.Blocks
{
    internal class Portal
    {
        public int tileX { get; set; }
        public int tileY { get; set; }
        public int tileWidth = 32;
        public int tileHeight = 32;
        public Texture2D spriteSheet;
        public Rectangle r;
        public Rectangle textureCoordinates;

        //Constructor
        public Portal() { }
        public Portal(int x, int y, Texture2D spriteSheet, Rectangle textureCoordinates)
        {
            tileX = x;
            tileY = y;
            r = new Rectangle(x, y, tileWidth, tileHeight);
            this.spriteSheet = spriteSheet;
            this.textureCoordinates = textureCoordinates;
        }
        public void DrawBlock(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.spriteSheet, r, this.textureCoordinates, Color.White);
        }
        public void DrawBlockRotate(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.spriteSheet, r, this.textureCoordinates, Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
        }
    }
}
