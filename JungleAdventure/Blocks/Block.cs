using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JungleAdventure.Blocks
{
    internal class Block : BaseTile
    {
        public Block(int x, int y, Texture2D spriteSheet, Rectangle textureCoordinates) : base(x, y, spriteSheet, textureCoordinates)
        {
            this.textureCoordinates = textureCoordinates;
        }
    }
}
