using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JungleAdventure.Blocks
{
    internal class Spike : BaseTile
    {
        public Spike(int x, int y, Texture2D spriteSheet, Rectangle textureCoordinates) : base(x, y, spriteSheet, textureCoordinates)
        {
            this.textureCoordinates = textureCoordinates;
        }
    }
}
