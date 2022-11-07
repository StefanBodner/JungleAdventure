using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JungleAdventure.Blocks
{
    public class BaseTile
    {
        public int tileX { get; set; }
        public int tileY { get; set; }
        public int tileWidth;
        public int tileHeight;
        public Texture2D spriteSheet;
        public Rectangle r;
        public Rectangle textureCoordinates;

        //Constructor
        public BaseTile() { }
        public BaseTile(int x, int y, Texture2D spriteSheet, Rectangle textureCoordinates)
        {
            tileX = x;
            tileY = y;
            tileWidth = 32;
            tileHeight = 32;
            r = new Rectangle(x, y, tileWidth, tileHeight);
            this.spriteSheet = spriteSheet;
            this.textureCoordinates = textureCoordinates;
        }

        public void DrawBlock(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.spriteSheet, r, this.textureCoordinates, Color.White);
        }
    }
}
