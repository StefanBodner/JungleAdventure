using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JungleAdventure.Blocks
{
    internal class Slope : BaseTile
    {
        // f(x) = kx + d

        float k;
        float d;
        int x;
        int y;

        public Slope(int x, int y, float k, float d, Texture2D spriteSheet, Rectangle textureCoordinates) : base(x, y, spriteSheet, textureCoordinates)
        {
            this.x = x;
            this.y = y;
            this.k = k;
            this.d = d;
            this.textureCoordinates = textureCoordinates;
        }


        public int CalcPlayerBottomCenterY(Point p)
        {
            float tileNumberX = (float)p.X / (float)tileWidth;
            float fractionNumber = (float)tileNumberX - (float)Math.Floor(tileNumberX);
            float temp2 = fractionNumber * (float)tileWidth;
            float posInSlopeY = (float)k * (float)temp2 + (float)d;
            float tileNumberY = (float)p.Y / (float)tileHeight;
            float fullTileNumberY = (int)Math.Ceiling(tileNumberY);
            float temp = fullTileNumberY * (float)tileHeight - posInSlopeY;
            int setPositionY = (int)temp;
           
            return setPositionY;
        }
    }
}
