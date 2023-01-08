using JungleAdventure.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace JungleAdventure
{
    public class Game1 : Game
    {
        #region Variables
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        static List<Rectangle> liBlockID = new List<Rectangle>();

        static List<Block> liBlocks = new List<Block>();
        static List<Slope> liSlopes = new List<Slope>();
        static List<Spike> liSpikes = new List<Spike>();
        static List<Coin> liCoins = new List<Coin>();

        Texture2D spriteSheet;
        BaseTile baseTile = new BaseTile() { };

        private Rectangle player;
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
        public bool whip;

        float whipTimer;
        float whipThreshold = 250;

        // Collision Detectors
        static Rectangle colBottom;
        static Rectangle colTop;
        static Rectangle colLeft;
        static Rectangle colRight;
        static Rectangle colBottomCenter;
        static Rectangle colBelowBottomCenter;
        static Rectangle colRightTop;
        static Rectangle colLeftTop;

        // Result of Collision Detection
        static bool canMoveToTheLeft;
        static bool canMoveToTheRight;

        // Set Bounds for "Camera" Movement
        static int borderRight;
        static int borderLeft;
        static bool touchesBorderRight;
        static bool touchesBorderLeft;

        // Graphics/Animation Timer
        float timer; // A timer that stores milliseconds.
        int threshold; // An int that is the threshold for the timer.s
        
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
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,0,0,0,0,0,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,5,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0 },
            { 1,1,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,4,0,0,0,0,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,1,5,0,0,2,1,0,0,0,0,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,4,0,0,0,0,2,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0 },
            { 1,0,0,1,5,2,1,1,0,0,0,2,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,4,0,0,0,0,2,1,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0 },
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
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
            base.Initialize();
        }
        #endregion 

        #region Game Structure
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteSheet = Content.Load<Texture2D>("SpriteSheet");
            Rectangle r;

            for (int y = 0; y <= 2; y++)
            {
                for (int x = 0; x <= 11; x++)
                {
                    r = new Rectangle(baseTile.tileWidth * x, baseTile.tileHeight * y, baseTile.tileWidth, baseTile.tileHeight);
                    liBlockID.Add(r);
                }
            }

            AnimationLoadContent();
        }
        protected override void Update(GameTime gameTime)
        {
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
            Whip(gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

        #region Collision
        public void SetCollision()
        {
            colBottom = new Rectangle(playerX, playerY + playerHeight, playerWidth, playerSpeed); //Bottom Collision
            colTop = new Rectangle(playerX, playerY + gravity, playerWidth, playerSpeed); //Top Collision
            colLeft = new Rectangle(playerX - playerSpeed, playerY, playerSpeed, playerHeight); // Left Collision
            colLeftTop = new Rectangle(playerX - playerSpeed, playerY, playerSpeed, baseTile.tileHeight / 2); // Left Bottom Collision
            colRight = new Rectangle(playerX + playerWidth, playerY, playerSpeed, playerHeight); // Right Collision
            colRightTop = new Rectangle(playerX + playerWidth, playerY, playerSpeed, baseTile.tileHeight / 2); // Right Bottom Collision
            colBottomCenter = new Rectangle(playerX + playerWidth / 2, playerY + playerHeight - 2, 1, 1); //Bottom Center Collision
            colBelowBottomCenter = new Rectangle(playerX + playerWidth / 2, playerY + playerHeight + baseTile.tileHeight / 2, 1, 1); //Bottom Center Collision
        }

        private void CheckCollision()
        {
            SetCollision();
            
            inAir = true;
            headroom = true;
            canMoveToTheLeft = true;
            canMoveToTheRight = true;
            bool onSlope = false;

            //Check Spikes
            foreach (Spike s in liSpikes)
            {
                if (player.Intersects(s.r))
                {
                    //ToDo: die
                }
            }

            //Check Slopes
            foreach (Slope s in liSlopes)
            {
                //Check if Player stands on Slope
                if (colBelowBottomCenter.Intersects(s.r) && gravity >= 0)
                {
                    inAir = false;
                    playerY = s.CalcPlayerBottomCenterY(new Point(colBelowBottomCenter.Left - worldOffsetX, colBelowBottomCenter.Top)) - playerHeight;
                    gravity = 0;
                    SetCollision();
                    onSlope = true;
                }
                //Check if Player touches Slope
                else if (colBottomCenter.Intersects(s.r) && gravity >= 0)
                {
                    inAir = false;
                    playerY = s.CalcPlayerBottomCenterY(new Point(colBottomCenter.Left - worldOffsetX, colBottomCenter.Top)) - playerHeight;
                    gravity = 0;
                    SetCollision();
                    onSlope = true;
                }
            }

            //Check Blocks
            foreach (Block b in liBlocks)
            {
                if (colBottom.Intersects(b.r) && !onSlope)
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
                if ((colLeft.Intersects(b.r) && !onSlope) || (colLeftTop.Intersects(b.r) && onSlope))
                {
                    canMoveToTheLeft = false;
                    playerX = b.r.Right;
                    SetCollision();
                }
                if ((colRight.Intersects(b.r) && !onSlope) || (colRightTop.Intersects(b.r) && onSlope))
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
            if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                left = true;
            }
            else { left = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                right = true;
            }
            else { right = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                up = true;
            }
            else { up = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.Space) && !left && !right)
            {
                whip = true;
            }
            else { whip = false; }
            if(Keyboard.GetState().IsKeyDown(Keys.R))
            {
                playerX = 300;
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
            else if (gravity >= 0) //if player starts falling whip again - reset jumpTick tracker
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
        public void Whip(GameTime gameTime)
        {
            if(whipTimer > whipThreshold)
            {
                Block b = new Block(playerX + playerWidth, playerY + playerHeight / 2, spriteSheet, dirtBlock);
                b.DrawBlock(spriteBatch);
            }

            if (whip)
            {
                whipTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                whipTimer = 0;
                return;
            }
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
                        Block b = new Block(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, spriteSheet, liBlockID[0]);
                        b.DrawBlock(spriteBatch);
                        liBlocks.Add(b);
                    }
                    else if (world[y, x] == 2) // steep slope right
                    {
                        Slope s = new Slope(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, 1f, 0, spriteSheet, liBlockID[4]);
                        s.DrawBlock(spriteBatch);
                        liSlopes.Add(s);
                    }
                    else if (world[y, x] == 3) // flat slope Bottom
                    {
                        Slope s = new Slope(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, 0.5f, 0, spriteSheet, liBlockID[2]);
                        s.DrawBlock(spriteBatch);
                        liSlopes.Add(s);
                    }
                    else if (world[y, x] == 4) // flat slope Top
                    {
                        Slope s = new Slope(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, 0.5f, baseTile.tileHeight / 2, spriteSheet, liBlockID[3]);
                        s.DrawBlock(spriteBatch);
                        liSlopes.Add(s);
                    }
                    else if (world[y, x] == 5) // steep slope left
                    {
                        Slope s = new Slope(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, -1f, baseTile.tileHeight, spriteSheet, liBlockID[4]);
                        s.DrawBlockRotate(spriteBatch);
                        liSlopes.Add(s);
                    }
                    else if (world[y, x] == 6) // Spike
                    {
                        Spike s = new Spike(x* baseTile.tileWidth + worldOffsetX, y* baseTile.tileHeight, spriteSheet, liBlockID[5]);
                        s.DrawBlock(spriteBatch);
                        liSpikes.Add(s);
                    }
                    else if (world[y, x] == 7) // Coin
                    {
                        Coin c = new Coin(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, spriteSheet, liBlockID[24]);
                        c.DrawBlock(spriteBatch);
                        liCoins.Add(c);
                    }
                }
            }
        }
        public void DrawPlayer()
        {
            player = new Rectangle(playerX, playerY, playerWidth, playerHeight);
            spriteBatch.Draw(spriteSheet, player, sourceRectangles[currentAnimationIndex], Color.White);
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
            sourceRectangles[0] = new Rectangle(0, 96, 25, 34);
            sourceRectangles[1] = new Rectangle(25, 96, 25, 34);

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