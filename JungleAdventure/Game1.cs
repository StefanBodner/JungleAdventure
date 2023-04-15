﻿using JungleAdventure.Blocks;
using JungleAdventure.Enemies;
using JungleAdventure.PlayerFolder;
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
        //Monogame visualization
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        //List to store blocks
        static List<Rectangle> liBlockID = new List<Rectangle>();

        //Lists of Elements
        static List<Block> liBlocks = new List<Block>();
        static List<Slope> liSlopes = new List<Slope>();
        static List<Spike> liSpikes = new List<Spike>();
        static List<Coin> liCoins = new List<Coin>();
        static List<Zombie> liZombie = new List<Zombie>();
        static List<Bullet> liBullets = new List<Bullet>();
        
        //Create BaseTypes of Classes to access variables
        Texture2D spriteSheet;
        Texture2D background;
        BaseTile baseTile = new BaseTile() { };
        Zombie baseZombie = new Zombie() { };
        Bullet bullet = new Bullet() { };
        Player p = new Player() { };

        //Player TODO: remove
        private Rectangle player;
        private int playerX = 490;
        private int playerY = 64;

        //Timer for Damage Animation
        float damageTimer = 0;
        float damageThreshold = 1500;
        int damageAnimationInvisFrame;

        //User input 
        public bool left;
        public bool right;
        public bool up;
        public bool shoot;

        //Timer for Shooting ability
        int shootingAnimationIndex;
        float shootTimer;
        float shootThreshold = 500;


        bool readyToFire = true;
        int bulletAmount = 6;
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
        Rectangle[] sourcePlayer;
        Rectangle[] shootingPlayer;
        Rectangle[] sourceCoins;
        Rectangle[] sourceZombie;
        int playerAnimationIndex;
        int zombieAnimationIndex;
        int coinAnimationIndex;
        bool lastInputRight = true; // false = lastInputLeft
        SpriteEffects spriteEffects = SpriteEffects.None;
        #endregion 

        #region Worlds
        static int worldOffsetX;

        static int[,] world = new int[,] {
            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0 ,0 ,0 , 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,9,0,0,0,0,0,0,0,0,0,0,0,1,10,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
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
            CheckCollisionBullet();
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
            DrawBullets();
            DrawWorld();
            DrawEnemies();

            DrawScoreAndLifes();
            Shoot(gameTime);

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
        private void DrawBullets()
        {
            foreach(Bullet b in liBullets)
            {
                b.DrawBullet(spriteBatch, worldOffsetX);         
            }

            for (int i = liBullets.Count - 1; i >= 0; i--)
            {
                if(liBullets[i].x < 0 - worldOffsetX || liBullets[i].x > 1000 - worldOffsetX)
                {
                    liBullets.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Collision
        public void SetCollision()
        {
            colBottom = new Rectangle(playerX, playerY + p.height, p.width, p.speed); //Bottom Collision
            colTop = new Rectangle(playerX, playerY + p.gravity, p.width, p.speed); //Top Collision
            colLeft = new Rectangle(playerX - p.speed, playerY, p.speed, p.height); // Left Collision
            colLeftTop = new Rectangle(playerX - p.speed, playerY, p.speed, baseTile.tileHeight / 2); // Left Bottom Collision
            colRight = new Rectangle(playerX + p.width, playerY, p.speed, p.height); // Right Collision
            colRightTop = new Rectangle(playerX + p.width, playerY, p.speed, baseTile.tileHeight / 2); // Right Bottom Collision
            colBottomCenter = new Rectangle(playerX + p.width / 2, playerY + p.height - 2, 1, 1); //Bottom Center Collision
            colBelowBottomCenter = new Rectangle(playerX + p.width / 2, playerY + p.height + baseTile.tileHeight / 2, 1, 1); //Bottom Center Collision
        }
        private void CheckCollisionPlayer(GameTime gameTime)
        {
            SetCollision();
            
            p.inAir = true;
            p.headroom = true;
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
                if (colBelowBottomCenter.Intersects(s.r) && p.gravity >= 0)
                {
                    p.inAir = false;
                    playerY = s.CalcPlayerBottomCenterY(new Point(colBelowBottomCenter.Left - worldOffsetX, colBelowBottomCenter.Top)) - p.height;
                    p.gravity = 0;
                    SetCollision();
                    onSlope = true;
                }
                //Check if Player touches Slope
                else if (colBottomCenter.Intersects(s.r) && p.gravity >= 0)
                {
                    p.inAir = false;
                    playerY = s.CalcPlayerBottomCenterY(new Point(colBottomCenter.Left - worldOffsetX, colBottomCenter.Top)) - p.height;
                    p.gravity = 0;
                    SetCollision();
                    onSlope = true;
                }
            }

            //Check Blocks
            foreach (Block b in liBlocks)
            {
                if (colBottom.Intersects(b.r) && !onSlope)
                {
                    p.inAir = false;
                    playerY = b.r.Top - p.height;
                    p.gravity = 0;
                    SetCollision();
                }
                if (colTop.Intersects(b.r) && p.gravity < 0)
                {
                    p.headroom = false;
                    p.gravity = 0;
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
                    playerX = b.r.Left - p.width;
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
            
            if (p.isInvincible)
            {
                if (damageTimer < damageThreshold)
                {
                    //Animatie Damage
                    damageTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
                else
                {
                    p.isInvincible = false;
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
        private void CheckCollisionBullet()
        {
            if(liBullets.Count <= 0)
            {
                return;
            }

            foreach(Bullet b in liBullets)
            {
                //Check Zombie Collision
                foreach(Zombie z in liZombie)
                {
                    if (b.r.Intersects(z.r))
                    {
                        liBullets.Remove(b);
                        liZombie.Remove(z);
                        score += 2; //Get Scorepoints for killing Zombie
                        return;
                    }
                }

                //Check Block Collision
                foreach(Block bl in liBlocks)
                {
                    if (b.r.Intersects(bl.r))
                    {
                        liBullets.Remove(b);
                        return;
                    }
                }
            }
        }
        private void RemoveLife()
        {
            if (!p.isInvincible)
            {
                life--;
                p.isInvincible = true;
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
                lastInputRight = false;
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            else { left = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                right = true;
                lastInputRight = true;
                spriteEffects = SpriteEffects.None;
            }
            else { right = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                up = true;
            }
            else { up = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.Space) && !left && !right && !p.inAir)
            {
                shoot = true;
            }
            else 
            { 
                shoot = false;
                readyToFire = true;
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
                playerX -= p.speed;
            }
            else if (right && canMoveToTheRight && !touchesBorderRight)
            {
                playerX += p.speed;
            }

            //allow player to jump
            if (up && !p.inAir)
            {
                p.inAir = true;
                p.gravity = p.force;
            }

            //check if player touches the floor
            if (p.inAir)
            {
                p.gravity += 1;
            }
            else if (!p.inAir)
            {
                p.gravity = 0;
            }

            //track jumping duration
            if (p.gravity < 0)
            {
                p.jumpTicks++;
            }
            else if (p.gravity >= 0) //if player starts falling - reset jumpTick tracker
            {
                p.jumpTicks = 0;
            }

            //Prevent player from going higher
            if (!up && p.gravity < 0 && p.jumpTicks >= p.minJumpTicks)
            {
                //-3 because of smoother jumping curve - not an aprupt stop in velocity
                p.gravity = -3;
                p.jumpTicks = 0;
            }

            //move player according to p.gravity's value
            playerY += p.gravity;
        }
        public void Shoot(GameTime gameTime)
        {
            if (!shoot || !readyToFire) //Do nothing if player doesn't shoot or isnt ready to fire
            {
                shootTimer = 0;
                return; 
            } 

            if (bulletAmount <= 0) //Do nothing if player has no bullets left
            {
                return;
            }

            if (shootTimer > shootThreshold)
            {
                //Shoot bullet after loading the gun
                liBullets.Add(new Bullet(playerX + p.width / 2 - worldOffsetX - bullet.bulletWidth / 2, playerY + p.height / 2 - 6, spriteSheet, new Rectangle(352, 224, bullet.bulletWidth, bullet.bulletHeight), lastInputRight));
                bulletAmount--;
                readyToFire = false;
                return;
            }
            shootTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

        }
        public void SetPlayerMovementBounds()
        {
            if (playerX + p.width > borderRight)
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
                worldOffsetX += p.speed;
            }
            else if (touchesBorderRight && right)
            {
                worldOffsetX -= p.speed;
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
                        case 11: //Spike
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
            player = new Rectangle(playerX, playerY, p.width, p.height);

            if (p.isInvincible) //Player took damage
            {
                if (damageAnimationInvisFrame <= 4) //Let user know that player was hit -> blinking
                {
                    damageAnimationInvisFrame ++;
                    spriteBatch.Draw(spriteSheet, player, sourcePlayer[playerAnimationIndex], Color.White, 0, new Vector2(0, 0), spriteEffects, 0);
                }
                else
                {
                    damageAnimationInvisFrame = 0;
                }
            }
            else if(shoot && readyToFire && bulletAmount != 0)
            {
                //Draw Player
                if(shootingAnimationIndex >= 21)
                {
                    spriteBatch.Draw(spriteSheet, player, shootingPlayer[3], Color.White, 0, new Vector2(0, 0), spriteEffects, 0);
                }
                else if (shootingAnimationIndex >= 14)
                {
                    spriteBatch.Draw(spriteSheet, player, shootingPlayer[2], Color.White, 0, new Vector2(0, 0), spriteEffects, 0);
                }
                else if (shootingAnimationIndex >= 7)
                {
                    spriteBatch.Draw(spriteSheet, player, shootingPlayer[1], Color.White, 0, new Vector2(0, 0), spriteEffects, 0);
                }
                else if (shootingAnimationIndex >= 0)
                {
                    spriteBatch.Draw(spriteSheet, player, shootingPlayer[0], Color.White, 0, new Vector2(0, 0), spriteEffects, 0);
                }

                //Next Animation Frame
                shootingAnimationIndex++;
            }
            else
            {
                //Draw Player - normal state
                spriteBatch.Draw(spriteSheet, player, sourcePlayer[playerAnimationIndex], Color.White, 0, new Vector2(0, 0), spriteEffects, 0);
                shootingAnimationIndex = 0;
            }

            
        }
        public void DrawScoreAndLifes()
        {
            spriteBatch.Draw(this.spriteSheet, new Rectangle(0, 0, 4 * 32, 3 * 32), new Rectangle(8 * 32, 6 * 32, 4 * 32, 3 * 32), Color.White);

            spriteBatch.DrawString(font, "Lifes: " + life, new Vector2(25, 20), Color.Black);
            spriteBatch.DrawString(font, "Score: " + score, new Vector2(25, 40), Color.Black);
            spriteBatch.DrawString(font, "Bullets: " + bulletAmount, new Vector2(25, 60), Color.Black);
        }
        #endregion

        #region Animation
        public void AnimationLoadContent()
        {
            // Set a default timer value.
            timer = 0;
            //speed of the animation (lower number = faster animation).
            threshold = 100;

            sourcePlayer = new Rectangle[7];
            sourcePlayer[0] = new Rectangle(0, 105, 32, p.height);
            sourcePlayer[1] = new Rectangle(32, 105, 32, p.height);
            sourcePlayer[2] = new Rectangle(64, 105, 32, p.height);
            sourcePlayer[3] = new Rectangle(96, 105, 32, p.height);
            sourcePlayer[4] = new Rectangle(128, 105, 32, p.height);
            sourcePlayer[5] = new Rectangle(160, 105, 32, p.height);
            sourcePlayer[6] = new Rectangle(192, 105, 32, p.height);

            shootingPlayer = new Rectangle[4];
            shootingPlayer[0] = new Rectangle(0, 169, 32, p.height);
            shootingPlayer[1] = new Rectangle(32, 169, 32, p.height);
            shootingPlayer[2] = new Rectangle(64, 169, 32, p.height);
            shootingPlayer[3] = new Rectangle(96, 169, 32, p.height);

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
                if (!right && !left)
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