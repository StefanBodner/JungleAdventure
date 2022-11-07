using JungleAdventure.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace JungleAdventure
{
    public class Game1 : Game
    {
        #region Variables
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        static List<Block> liBlocks = new List<Block>();
        static List<Slope> liSlopes = new List<Slope>();

        Texture2D spriteSheet;

        private Texture2D boxTexture;
        private int boxWidth = 32;
        private int boxHeight = 32;

        private Texture2D playerTexture;
        private int playerSpeed = 3;
        private int playerHeight = 48;
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

        static Rectangle colBottom;
        static Rectangle colTop;
        static Rectangle colLeft;
        static Rectangle colRight;
        static Rectangle playerBotCenter;

        static bool canMoveToTheLeft;
        static bool canMoveToTheRight;

        static int borderRight;
        static int borderLeft;
        static bool touchesBorderRight;
        static bool touchesBorderLeft;


        float timer; // A timer that stores milliseconds.
        int threshold; // An int that is the threshold for the timer.
        
        Rectangle[] sourceRectangles;// A Rectangle array that stores sourceRectangles for animations.
        int previousAnimationIndex; // These bytes tell the spriteBatch.Draw() what sourceRectangle to display. 
        int currentAnimationIndex;

        //Texture Coordinates SpriteSheet
        Rectangle dirtBlock = new Rectangle(0,0,32,32);
        #endregion 

        #region Worlds
        static int worldOffsetX;

        static int[,] world = new int[,] {
            { 1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,1,0,0,0,0,0,0,3,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0 },
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 }};

        #endregion

        #region Game Initialization
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //graphics.PreferredBackBufferWidth = 640;  // set this value to the desired width of your window
            //graphics.PreferredBackBufferHeight = 360;   // set this value to the desired height of your window
            borderLeft = graphics.PreferredBackBufferWidth / 100 * 40;
            borderRight = graphics.PreferredBackBufferWidth / 100 * 60;
            graphics.ApplyChanges();
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }
        #endregion

        #region Game Structure
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            boxTexture = Content.Load<Texture2D>("dirt");
            playerTexture = Content.Load<Texture2D>("SpriteSheet");
            spriteSheet = Content.Load<Texture2D>("dirt");

            AnimationLoadContent();
        }
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            SetCollision();
            SetPlayerMovementBounds();
            PlayerInput();
            CheckCollision();
            BasicMovement();
            AnimationUpdate(gameTime);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin();
            DrawPlayer();
            DrawWorld();
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

        #region Collision
        public void SetCollision()
        {
            colBottom = new Rectangle(playerX, playerY + playerHeight, playerWidth, playerSpeed); //Bottom Collision
            colTop = new Rectangle(playerX, playerY - playerSpeed, playerWidth, playerSpeed); //Top Collision
            colLeft = new Rectangle(playerX - playerSpeed, playerY, playerSpeed, playerHeight); // Left Top Collision
            colRight = new Rectangle(playerX + playerWidth, playerY, playerSpeed, playerHeight); // Left Top Collision
            playerBotCenter = new Rectangle(playerX + playerWidth, playerY + playerHeight - 1, 1, 1); //Bottom Center Collision
        }

        private void CheckCollision()
        {
            SetCollision();
            
            inAir = true;
            headroom = true;
            canMoveToTheLeft = true;
            canMoveToTheRight = true;

            foreach (Slope s in liSlopes)
            {
                if (playerBotCenter.Intersects(s.r))
                {
                    inAir = false;
                    playerY = s.CalcPlayerBottomCenterY(new Point(playerBotCenter.Left, playerBotCenter.Top)) - playerHeight; 
                    gravity = 0;
                    SetCollision();
                }
            }

            //Check all full Blocks
            foreach (Block b in liBlocks)
            {
                if (colBottom.Intersects(b.r))
                {
                    inAir = false;
                    playerY = b.r.Top - playerHeight;
                    gravity = 0;
                    SetCollision();
                }
                if (colTop.Intersects(b.r) && gravity < 0)
                {
                    headroom = false;
                    gravity = 0;
                    playerY = b.r.Bottom;
                    SetCollision();
                }
                if (colLeft.Intersects(b.r))
                {
                    canMoveToTheLeft = false;
                    playerX = b.r.Right;
                    SetCollision();
                }
                if (colRight.Intersects(b.r))
                {
                    canMoveToTheRight = false;
                    playerX = b.r.Left - playerWidth;
                    SetCollision();
                }
            }
        }
        #endregion

        #region Player + World Creation
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
            if (left && canMoveToTheLeft && !touchesBorderLeft)
            {
                playerX -= playerSpeed;
            }
            else if (right && canMoveToTheRight && !touchesBorderRight)
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
        public void SetPlayerMovementBounds()
        {
            if (playerX + playerWidth > borderRight)
            {
                touchesBorderRight = true;
            }
            else
            {
                touchesBorderRight = false;
            }

            if (playerX < borderLeft)
            {
                touchesBorderLeft = true;
            }
            else
            {
                touchesBorderLeft = false;
            }
        }
        public void SetWorldOffset()
        {
            if (touchesBorderLeft && left)
            {
                worldOffsetX += playerSpeed;
            }
            else if (touchesBorderRight && right)
            {
                worldOffsetX -= playerSpeed;
            }
        }
        public void DrawWorld()
        {
            SetWorldOffset();
            liBlocks.Clear();
            liSlopes.Clear();
            for (int y = 0; y < world.GetLength(0); y++)
            {
                for (int x = 0; x < world.GetLength(1); x++)
                {
                    if (world[y, x] == 1) // normal block
                    {
                        Block b = new Block(x * boxWidth + worldOffsetX, y * boxHeight, spriteSheet, dirtBlock);
                        b.DrawBlock(spriteBatch);
                        liBlocks.Add(b);
                    }
                    else if (world[y, x] == 2) // steep slope
                    {
                        Slope s = new Slope(x * boxWidth + worldOffsetX, y * boxHeight, 1f, 0, spriteSheet, dirtBlock);
                        s.DrawBlock(spriteBatch);
                        liSlopes.Add(s);
                    }
                    else if (world[y, x] == 3) // flat slope Bottom
                    {
                        Slope s = new Slope(x * boxWidth + worldOffsetX, y * boxHeight, 0.5f, 0, spriteSheet, dirtBlock);
                        s.DrawBlock(spriteBatch);
                        liSlopes.Add(s);
                    }
                    else if (world[y, x] == 4) // flat slope Top
                    {
                        Slope s = new Slope(x * boxWidth + worldOffsetX, y * boxHeight, 0.5f, boxHeight / 2, spriteSheet, dirtBlock);
                        s.DrawBlock(spriteBatch);
                        liSlopes.Add(s);
                    }
                }
            }
        }
        public void DrawPlayer()
        {
            Rectangle player = new Rectangle(playerX, playerY, playerWidth, playerHeight);
            spriteBatch.Draw(playerTexture, player, sourceRectangles[currentAnimationIndex], Color.White);
        }
        #endregion

        #region Animation
        public void AnimationLoadContent()
        {
            // Set a default timer value.
            timer = 0;
            // Set an initial threshold of 250ms, you can change this to alter the speed of the animation (lower number = faster animation).
            threshold = 250;
            
            sourceRectangles = new Rectangle[3];
            sourceRectangles[0] = new Rectangle(0, 0, 550, 750);
            sourceRectangles[1] = new Rectangle(603, 0, 550, 750);

            // This tells the animation to start on the left-side sprite.
            currentAnimationIndex = 1;
        }
        public void AnimationUpdate(GameTime gameTime)
        {
            // Check if the timer has exceeded the threshold.
            if (timer > threshold)
            {
                // If Alex is in the middle sprite of the animation.
                if (currentAnimationIndex == 1)
                {
                    currentAnimationIndex = 0;
                }
                else
                {
                    currentAnimationIndex = 1;
                }
                // Reset the timer.
                timer = 0;
            }
            // If the timer has not reached the threshold, then add the milliseconds that have past since the last Update() to the timer.
            else
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }
        #endregion
    }
}