using JungleAdventure.Blocks;
using JungleAdventure.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace JungleAdventure
{
    public class Game1 : Game
    {
        #region Variables
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        static List<Rectangle> liBlockID = new List<Rectangle>();

        static List<Block> liBlocks = new List<Block>();
        static List<Slope> liSlopes = new List<Slope>();
        static List<Spike> liSpikes = new List<Spike>();
        static List<Coin> liCoins = new List<Coin>();
        static List<Zombie> liZombie = new List<Zombie>();
        
        Texture2D spriteSheet;
        Texture2D background;
        BaseTile baseTile = new BaseTile() { };
        Zombie baseZombie = new Zombie() { };

        private Rectangle player;
        private int playerSpeed = 3;
        private int playerHeight = 55;
        private int playerWidth = 32;
        private int playerX = 490;
        private int playerY = 64;
        public int jumpTicks = 0;
        public int minJumpTicks = 5;
        public int force = -17;
        public int gravity = 0;

        bool isInvincible = false;
        float damageTimer = 0;
        float damageThreshold = 1200;
        
        public bool inAir = true;
        public bool headroom = true;

        public bool left;
        public bool right;
        public bool up;
        public bool shoot;

        float shootTimer;
        float shootThreshold = 250;

        int score = 0;
        int life = 3;

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
        int threshold; // An int that is the threshold for the timer.
        Rectangle[] sourcePlayer;// A Rectangle array that stores sourcePlayer for animations.
        Rectangle[] sourceCoins;
        Rectangle[] sourceZombie;
        int playerAnimationIndex;
        int zombieAnimationIndex;
        int coinAnimationIndex;
        bool lastInputRight = true; // false = lastInputLeft
        SpriteEffects spriteEffects = SpriteEffects.None;

        //Texture Coordinates SpriteSheet
        Rectangle dirtBlock = new Rectangle(0,0,32,32);
        #endregion 

        #region Worlds
        static int worldOffsetX;

        static int[,] world = new int[,] {
            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0 ,0 ,0 , 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,9,0,0,0,0,0,0,0,0,0,0,0,0,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,8,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,9,0,0,0,0,0,0,3,4,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0 },
            { 0,1,8,0,0,0,0,11,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,4,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,1,8,0,0,2,1,0,0,0,0,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,4,1,1,1,1,1,1,1,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0 },
            { 0,0,0,1,8,2,1,1,0,0,0,2,1,1,0,0,0,10,0,0,0,0,0,0,0,0,0,0,0,0,3,4,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0 },
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
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
            background = Content.Load<Texture2D>("background");
            Rectangle r = new Rectangle(baseTile.tileWidth, baseTile.tileHeight, baseTile.tileWidth, baseTile.tileHeight);
            liBlockID.Add(r);
            font = Content.Load<SpriteFont>("defaultFont");

            for (int y = 0; y <= 2; y++)
            {
                for (int x = 0; x <= 11; x++)
                {
                    r = new Rectangle(baseTile.tileWidth * x, baseTile.tileHeight * y, baseTile.tileWidth, baseTile.tileHeight);
                    liBlockID.Add(r);
                }
            }

            //Load all Animations
            AnimationLoadContent();

            //Load All Zombies in List
            for (int y = 0; y < world.GetLength(0); y++)
            {
                for (int x = 0; x < world.GetLength(1); x++)
                {
                    switch (world[y, x])
                    {
                        case 10:
                            liZombie.Add(new Zombie(x * baseTile.tileWidth, y * baseTile.tileHeight - 10, spriteSheet, sourceZombie[zombieAnimationIndex]));
                            break;
                    }
                }
            }
            
        }

        protected override void Update(GameTime gameTime)
        {
            SetCollision();
            SetPlayerMovementBounds();
            PlayerInput();
            CheckCollisionPlayer(gameTime);
            CheckCollisionEnemy();
            BasicMovement();

            AnimationUpdate(gameTime);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            //Draw Background
            spriteBatch.Draw(background, new Rectangle(worldOffsetX / 3 - background.Width, 0, background.Width, background.Height), new Rectangle(0, 0, background.Width, background.Height), Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(background, new Rectangle(worldOffsetX / 3, 0, background.Width, background.Height), Color.White);
            spriteBatch.Draw(background, new Rectangle(worldOffsetX / 3 + background.Width, 0, background.Width, background.Height), new Rectangle(0, 0, background.Width, background.Height),Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(background, new Rectangle(worldOffsetX / 3 + background.Width * 2, 0, background.Width, background.Height), Color.White);

            DrawPlayer();
            DrawWorld();
            DrawEnemies();

            DrawScoreAndLifes();
            Whip(gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }
        private void DrawEnemies()
        {
            foreach (Zombie z in liZombie)
            {
                z.textureCoordinates = sourceZombie[zombieAnimationIndex];
                z.DrawZombie(spriteBatch, worldOffsetX);
            }
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
        private void CheckCollisionPlayer(GameTime gameTime)
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
                    RemoveLife();
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

            //Check Coins
            foreach (Coin c in liCoins)
            {
                if (player.Intersects(c.r))
                {
                    score++;
                    if(score == 10)
                    {
                        life++;
                        score = 0;
                    }

                    int coinBlockY = Convert.ToInt32(Math.Floor(((float)c.tileX - (float)worldOffsetX) / (float)baseTile.tileWidth));
                    int coinBlockX = Convert.ToInt32(Math.Floor((float)c.tileY / (float)baseTile.tileWidth));

                    world[coinBlockX, coinBlockY] = 0;
                }
            }

            //Check Zombie
            foreach (Zombie z in liZombie)
            {
                if (player.Intersects(z.r))
                {
                    RemoveLife();
                }
            }
            
            if (isInvincible)
            {
                if (damageTimer < damageThreshold)
                {
                    damageTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
                else
                {
                    isInvincible = false;
                    damageTimer = 0;
                }
            }

            if(playerY > 16 * 32)
            {
                RemoveLife();
            }
        }
        private void CheckCollisionEnemy()
        {
            foreach (Zombie z in liZombie)
            {
                //Create Ground Check for Zombies
                Rectangle groundCheck = new Rectangle(z.r.X + z.zombieWidth / 2, z.r.Y + z.zombieHeight, 1, 16);

                bool inAir = true;
                bool touchesWall = false;

                foreach (Block b in liBlocks)
                {
                    //Check if Zombie touches Ground
                    if (groundCheck.Intersects(b.r))
                    {
                        inAir = false;
                    }
                    if (z.r.Intersects(b.r))
                    {
                        touchesWall = true;
                    }
                }

                if (touchesWall || inAir)
                {
                    //Change walking direction
                    z.zombieSpeed = -z.zombieSpeed;
                }

                //Move Zombie
                z.awayFromBaseXCoordinate += z.zombieSpeed;
            }
        }
        private void RemoveLife()
        {
            if (!isInvincible)
            {
                life--;
                isInvincible = true;
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
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            else { left = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                right = true;
                spriteEffects = SpriteEffects.None;
            }
            else { right = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                up = true;
            }
            else { up = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.Space) && !left && !right)
            {
                shoot = true;
            }
            else { shoot = false; }
            if(Keyboard.GetState().IsKeyDown(Keys.R))
            {
                playerX = 300;
                playerY = 64;
                gravity = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                //TODO: OPEN NEW WINDOW
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
            if(shootTimer > shootThreshold)
            {
                Block b = new Block(playerX + playerWidth, playerY + playerHeight / 2, spriteSheet, dirtBlock);
                b.DrawBlock(spriteBatch);
            }

            if (shoot)
            {
                shootTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                shootTimer = 0;
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
            liCoins.Clear();
            liSpikes.Clear();

            Block b;
            Slope s;
            Coin c;
            Spike spike;

            for (int y = 0; y < world.GetLength(0); y++)
            {
                for (int x = 0; x < world.GetLength(1); x++)
                {
                    switch (world[y, x])
                    {
                        case 1: //dirt block
                            b = new Block(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, spriteSheet, liBlockID[1]);
                            b.DrawBlock(spriteBatch);
                            liBlocks.Add(b);
                            break;
                        case 2: //grass block
                            b = new Block(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, spriteSheet, liBlockID[2]);
                            b.DrawBlock(spriteBatch);
                            liBlocks.Add(b);
                            break;
                        case 3: //grass flat bottom slope | bottom right
                            s = new Slope(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, 0.5f, 0, spriteSheet, liBlockID[3]);
                            s.DrawBlock(spriteBatch);
                            liSlopes.Add(s);
                            break;
                        case 4: //grass flat top slope | bottom right
                            s = new Slope(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, 0.5f, baseTile.tileHeight / 2, spriteSheet, liBlockID[4]);
                            s.DrawBlock(spriteBatch);
                            liSlopes.Add(s);
                            break;
                        case 5: //grass steep slope | bottom right
                            s = new Slope(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, 1f, 0, spriteSheet, liBlockID[5]);
                            s.DrawBlock(spriteBatch);
                            liSlopes.Add(s);
                            break;
                        case 6: //grass flat bottom slope | bottom left
                            s = new Slope(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, 0.5f, 0, spriteSheet, liBlockID[3]);
                            s.DrawBlockRotate(spriteBatch);
                            liSlopes.Add(s);
                            break;
                        case 7: //grass flat top slope | bottom left
                            s = new Slope(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, 0.5f, baseTile.tileHeight / 2, spriteSheet, liBlockID[4]);
                            s.DrawBlockRotate(spriteBatch);
                            liSlopes.Add(s);
                            break;
                        case 8: //grass steep slope | bottom left
                            s = new Slope(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, -1f, baseTile.tileHeight, spriteSheet, liBlockID[5]);
                            s.DrawBlockRotate(spriteBatch);
                            liSlopes.Add(s);
                            break;
                        case 9: //Coin
                            c = new Coin(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, spriteSheet, sourceCoins[coinAnimationIndex]);
                            c.DrawBlock(spriteBatch);
                            liCoins.Add(c);
                            break;
                        case 10:
                            //Already handled somewhere else
                            break;
                        case 11:
                            spike = new Spike(x * baseTile.tileWidth + worldOffsetX, y * baseTile.tileHeight, spriteSheet, liBlockID[6]);
                            spike.DrawBlock(spriteBatch);
                            liSpikes.Add(spike);
                            break;

                    }
                }
            }
        }
        public void DrawPlayer()
        {
            player = new Rectangle(playerX, playerY, playerWidth, playerHeight);
            spriteBatch.Draw(spriteSheet, player, sourcePlayer[playerAnimationIndex], Color.White, 0, new Vector2(0, 0), spriteEffects, 0);
        }

        #endregion

        public void DrawScoreAndLifes()
        {
            spriteBatch.Draw(this.spriteSheet, new Rectangle(0,0,4*32,3*32), new Rectangle(8*32, 6*32, 4*32, 3*32), Color.White);

            spriteBatch.DrawString(font, "Lifes: " + life, new Vector2(30, 20), Color.Black);
            spriteBatch.DrawString(font, "Score: " + score, new Vector2(30, 45), Color.Black);
        }

        #region Animation
        public void AnimationLoadContent()
        {
            // Set a default timer value.
            timer = 0;
            //speed of the animation (lower number = faster animation).
            threshold = 100;

            sourcePlayer = new Rectangle[7];
            sourcePlayer[0] = new Rectangle(0, 105, 32, playerHeight);
            sourcePlayer[1] = new Rectangle(32, 105, 32, playerHeight);
            sourcePlayer[2] = new Rectangle(64, 105, 32, playerHeight);
            sourcePlayer[3] = new Rectangle(96, 105, 32, playerHeight);
            sourcePlayer[4] = new Rectangle(128, 105, 32, playerHeight);
            sourcePlayer[5] = new Rectangle(160, 105, 32, playerHeight);
            sourcePlayer[6] = new Rectangle(192, 105, 32, playerHeight);

            sourceCoins = new Rectangle[6];
            sourceCoins[0] = new Rectangle(0, 64, baseTile.tileWidth, baseTile.tileHeight);
            sourceCoins[1] = new Rectangle(32, 64, baseTile.tileWidth, baseTile.tileHeight);
            sourceCoins[2] = new Rectangle(64, 64, baseTile.tileWidth, baseTile.tileHeight);
            sourceCoins[3] = new Rectangle(96, 64, baseTile.tileWidth, baseTile.tileHeight);
            sourceCoins[4] = new Rectangle(128, 64, baseTile.tileWidth, baseTile.tileHeight);
            sourceCoins[5] = new Rectangle(160, 64, baseTile.tileWidth, baseTile.tileHeight);

            sourceZombie = new Rectangle[4];
            sourceZombie[0] = new Rectangle(0, 224 + 22, baseZombie.zombieWidth, baseZombie.zombieHeight);
            sourceZombie[1] = new Rectangle(32, 224 + 22, baseZombie.zombieWidth, baseZombie.zombieHeight);
            sourceZombie[2] = new Rectangle(64, 224 + 22, baseZombie.zombieWidth, baseZombie.zombieHeight);
            sourceZombie[3] = new Rectangle(96, 224 + 22, baseZombie.zombieWidth, baseZombie.zombieHeight);

            // This tells the animation to start on the left-side sprite.
            playerAnimationIndex = 1;
        }
        public void AnimationUpdate(GameTime gameTime)
        { 
            // Check if the timer has exceeded the threshold.
            if (timer > threshold) 
            {
                //Player Animation
                if(!right && !left)
                {
                    if (lastInputRight)
                    {
                        playerAnimationIndex = 0;
                    }
                    else
                    {
                        playerAnimationIndex = 0;
                    }
                }
                if (right || left)
                {
                    playerAnimationIndex++;
                    if (playerAnimationIndex == sourcePlayer.Length)
                    {
                        playerAnimationIndex = 1;
                    } 
                }

                //Coins Animation
                coinAnimationIndex++;
                if (coinAnimationIndex == 6)
                {
                    coinAnimationIndex = 0;
                }

                //Zombie Animation
                zombieAnimationIndex++;
                if(zombieAnimationIndex == 3)
                {
                    zombieAnimationIndex = 0;
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