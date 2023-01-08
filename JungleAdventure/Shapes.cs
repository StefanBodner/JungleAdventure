using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;

namespace JungleAdventure
{
    public sealed class Shapes : IDisposable
    {
        bool isDisposed;
        private Game1 game;
        private BasicEffect effect;
        private VertexPositionTexture[] vertices;
        private int[] indices;

        private int shapeCount;
        private int vertexCount;
        private int indexCount;

        public Shapes(Game1 gameReal, Texture2D spriteSheet)
        {
            isDisposed = false;
            game = gameReal;

            effect = new BasicEffect(game.GraphicsDevice);
            effect.TextureEnabled = true;
            effect.Texture = spriteSheet;
            effect.FogEnabled = true;
            effect.LightingEnabled = true;
            effect.World = Matrix.Identity;
            effect.View = Matrix.Identity;
            effect.Projection = Matrix.Identity;

            const int MaxVertexCount = 1024;
            const int MaxIndexCount = MaxVertexCount * 3;

            vertices = new VertexPositionTexture[MaxVertexCount];
            indices = new int[MaxIndexCount];
            
            shapeCount = 0;
            vertexCount = 0;
            indexCount = 0;

        }

        public void Begin()
        {
            Viewport vp = game.GraphicsDevice.Viewport;
            effect.Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0, 1);
        }

        public void End()
        {

        }

        public void Flush()
        {
            if(shapeCount == 0)
            {
                return;
            }

            foreach(EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vertices, 0, vertexCount, indices, 0, indexCount / 3);
            }

            shapeCount = 0;
            vertexCount = 0;
            indexCount = 0;
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }
            effect?.Dispose();
            isDisposed = true;
        }

        public void EnsureSpace(int shapeVertexCount, int shapeIndexCount)
        {
            if(shapeVertexCount > vertices.Length)
            {
                throw new Exception ("Maximum shape vertex count is: " + vertices.Length);
            } 
            if(shapeIndexCount > indices.Length)
            {
                throw new Exception("Maximum shape index count is: " + indices.Length);
            }
            
            if(vertexCount + shapeVertexCount > vertices.Length || indexCount + shapeIndexCount > indices.Length)
            {
                Flush();
            }
        }

        public void DrawRectangle(float x, float y, float width, float height, Vector2 texture)
        {
            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;

            EnsureSpace(shapeVertexCount, shapeIndexCount);

            float left = x;
            float top = y + height;
            float right = x + width;
            float bottom = y;

            Vector2 a = new Vector2(left, top);
            Vector2 b = new Vector2(right, top);
            Vector2 c = new Vector2(right, bottom);
            Vector2 d = new Vector2(left, bottom);

            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 1 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 3 + vertexCount;

            vertices[vertexCount++] = new VertexPositionTexture(new Vector3(a, 0), texture);
            vertices[vertexCount++] = new VertexPositionTexture(new Vector3(b, 0), texture);
            vertices[vertexCount++] = new VertexPositionTexture(new Vector3(c, 0), texture);
            vertices[vertexCount++] = new VertexPositionTexture(new Vector3(d, 0), texture);

            shapeCount++;
        }
    }
}
