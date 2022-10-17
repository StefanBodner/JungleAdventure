﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace JungleAdventure
{
    public class Game1 : Game
    {
        #region Variables
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        static List<Rectangle> liBlocks = new List<Rectangle>();
        private Texture2D boxTexture;
        private int boxWidth = 32;
        private int boxHeight = 32;

        private Texture2D playerTexture;
        private int playerSpeed = 4;
        private int playerHeight = 64;
        private int playerWidth = 32;
        private int playerX = 512;
        private int playerY = 64;
        public int jumpTicks = 0;
        public int minJumpTicks = 5;
        public int force = -17;
        public int gravity = 0;
        public bool inAir = true;
        public bool headroom = true;

        public bool left;
        public bool right;
        public bool up;
        public bool down;

        static Rectangle colBot;
        static Rectangle colTop;
        static Rectangle colLeftTop;
        static Rectangle colRightTop;
        static Rectangle colLeftBot;
        static Rectangle colRightBot;

        static Point midBotPlayer;

        static bool canMoveToTheLeft;
        static bool canMoveToTheRight;
        static bool borderRight;
        #endregion 

        #region Worlds
        static bool worldCreated = false;

        static int[,] world = new int[,] {
            { 1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0 },
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 }};

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            boxTexture = Content.Load<Texture2D>("dirt");
            playerTexture = Content.Load<Texture2D>("leonZwerg");

            LoadWorld();
            
            // TODO: use this.Content to load your game content here
        }
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            PlayerInput();
            BasicMovement();
            Collision();
            base.Update(gameTime);
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            
            Rectangle player = new Rectangle(playerX, playerY, playerWidth, playerHeight);
            spriteBatch.Draw(playerTexture, player, Color.AliceBlue);
            
            DrawWorld();
           
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public void SetCollision()
        {
            colBot = new Rectangle(playerX, playerY + playerHeight, playerWidth, playerSpeed); //Bottom Collision
            colTop = new Rectangle(playerX, playerY - playerSpeed, playerWidth, playerSpeed); //Top Collision
            colLeftTop = new Rectangle(playerX - playerSpeed, playerY, playerSpeed, playerHeight); // Left Top Collision
            colRightTop = new Rectangle(playerX + playerWidth, playerY, playerSpeed, playerHeight); // Left Top Collision
        }

        private void Collision()
        {
            SetCollision();

            //Check if player intersects with any Blocks/Enemies/...
            inAir = true;
            headroom = true;
            canMoveToTheLeft = true;
            canMoveToTheRight = true;
            foreach (Rectangle b in liBlocks)
            {
                if (colBot.Intersects(b))
                {
                    inAir = false;
                    playerY = b.Top - playerHeight;
                    gravity = 0;
                    SetCollision();
                }
                if (colTop.Intersects(b) && gravity < 0)
                {
                    headroom = false;
                    gravity = 0;
                    playerY = b.Bottom;
                    SetCollision();
                }
                if (colLeftTop.Intersects(b))
                {
                    canMoveToTheLeft = false;
                    playerX = b.Right;
                    SetCollision();
                }
                if (colRightTop.Intersects(b))
                {
                    canMoveToTheRight = false;
                    playerX = b.Left - playerWidth;
                    SetCollision();
                }
            }
        }
        private void PlayerInput()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                left = true;
            }
            else { left = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                right = true;
            }
            else { right = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                up = true;
            }
            else { up = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                down = true;
            }
            else { down = false; }
            if(Keyboard.GetState().IsKeyDown(Keys.R))
            {
                playerX = 512;
                playerY = 64;
                gravity = 0;
            }
        }

        public void BasicMovement()
        {
            //basic left & right movement
            if (left && canMoveToTheLeft)
            {
                playerX -= playerSpeed;
            }
            else if (right && canMoveToTheRight)
            {
                playerX += playerSpeed;
            }

            //allow player to jump
            if (up && !inAir)
            {
                inAir = true;
                gravity = force;
            }

            //check if player touches the floor
            if (inAir)
            {
                gravity += 1;
            }
            else if (!inAir)
            {
                gravity = 0;
            }

            //track jumping duration
            if (gravity < 0)
            {
                jumpTicks++;
            }
            else if (gravity >= 0) //if player starts falling down again - reset jumpTick tracker
            {
                jumpTicks = 0;
            }

            //Prevent player from going higher
            if (!up && gravity < 0 && jumpTicks >= minJumpTicks)
            {
                //-3 because of smoother jumping curve - not an aprupt stop in velocity
                gravity = -3;
                jumpTicks = 0;
            }

            //move player according to gravity's value
            playerY += gravity;
        }
        public void LoadWorld()
        {
            for (int y = 0; y < world.GetLength(0); y++)
            {
                for (int x = 0; x < world.GetLength(1); x++)
                {
                    if (world[y, x] == 1) // normal block
                    {
                        Rectangle block = new Rectangle(x * boxWidth, y * boxHeight, boxWidth, boxHeight);
                        liBlocks.Add(block);
                    }
                }
            }
        }
        public void DrawWorld()
        {
            foreach (Rectangle r in liBlocks)
            {
                spriteBatch.Draw(boxTexture, r, Color.AliceBlue);
            }
        }
    }
}