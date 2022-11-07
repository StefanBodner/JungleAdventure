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
        float playerX;
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
            
            Debug.WriteLine("(" + p.X + " | " + p.Y + ")");
            Debug.WriteLine("tileNumberX: " + tileNumberX);
            Debug.WriteLine("fractionNumber: " + fractionNumber);
            Debug.WriteLine("posInSlopeY: " + posInSlopeY);
            Debug.WriteLine("tileNumberY: " + tileNumberY);
            Debug.WriteLine("fullTileNumberY: " + fullTileNumberY);
            Debug.WriteLine("setPositionY: " + setPositionY);
            Debug.WriteLine("");
            return setPositionY;
        }
        public int CalcPlayerX(Point p)
        {
            // f2(x) = mx + c

            float m = 1 / k * - 1; //shortes distance between point and funktion
            float b = p.Y - k * p.X;
            playerX = (b - d) / (k - m);
            int playerXtransformed = (int)Math.Round(playerX);
            return playerXtransformed;
        }
    }
}
