using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace PixelBlastFree
{
    public partial class GamePage : PhoneApplicationPage
    {
        ContentManager contentManager;
        GameTimer timer;
        //SpriteBatch spriteBatch;

        GraphicsDevice graphics;
        SpriteBatch spriteBatch;
        SpriteFont mainFont;//, errorFont, largerFont;

        Vector2 screen; //dimensions

        VibrateController vibrate;

        //resolution independence
        //Vector2 virtualScreen = new Vector2(800, 480);
        //static Vector3 scalingFactor;
        //Matrix scaleM;

        //enum for game states
        //public enum GameState { MainMenu, Playing, GameOver }
        //static GameState currentGameState;

        //using an enum for game difficulty. numbers associated with each entry is what is used to calculate and scale things
        public enum Difficulty { Easy = 3, Medium = 4, Hard = 5, Extreme = 7 };
        static Difficulty selectedDifficulty;

        //we cannot call exit from a screen so the screen sets a bool and the Game1 update method picks it up
        //static bool toExit = false;

        public static bool useGameMusic;
        public static bool isPaused;

        #region Class Variables
        Player playerOne;
        //SoundEffectInstance backgroundMusic;
        SoundEffect laserFire;
        Texture2D defaultBulletTex, playerBulletTex;

        List<Bullet> playerBullets; //list to contain all player's bullets
        //Bullet[] enemyBullets;  //array to contain all enemy bullets
        List<Bullet> enemyBullets;
        List<Enemy> enemies;    //list of all enemies existing in game
        List<Explosion> particleEffectsList;    //list for particle effects

        Starfield myStars;

        public GamePage()
        {
            InitializeComponent();

            // Get the content manager from the application
            contentManager = (Application.Current as App).Content;

            // Create a timer for this page
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;            

            //currentGameState = GameState.MainMenu;
            selectedDifficulty = Difficulty.Medium;

            vibrate = VibrateController.Default;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string diffString;
            NavigationContext.QueryString.TryGetValue("difficulty", out diffString);
            
            switch (diffString)
            {
                case "Easy":
                    selectedDifficulty = Difficulty.Easy;
                    break;
                case "Extreme":
                    selectedDifficulty = Difficulty.Extreme;
                    break;
                case "Hard":
                    selectedDifficulty = Difficulty.Hard;
                    break;
                default:
                    selectedDifficulty = Difficulty.Medium;
                    break;
            }

            //create the player object
            playerOne = new Player((float)selectedDifficulty, new Vector2(0));

            //set the player's name
            string playerName;
            NavigationContext.QueryString.TryGetValue("playerName", out playerName);
            playerOne.Name = playerName;

            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            graphics = SharedGraphicsDeviceManager.Current.GraphicsDevice;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(graphics);

            // TODO: use this.content to load your game content here
            Particle.DefaultTexture = contentManager.Load<Texture2D>("defaultParticle");

            //backgroundImage = content.Load<Texture2D>("space");
            defaultBulletTex = contentManager.Load<Texture2D>("defaultBullet");
            playerBulletTex = contentManager.Load<Texture2D>("player/playerBullet");
            //defaultEnemyTex = content.Load<Texture2D>("enemy");   //loaded by enemy classes
            //enemy2Tex = content.Load<Texture2D>("enemy2");
            laserFire = contentManager.Load<SoundEffect>("player/playerLaser");

            screen = new Vector2(graphics.PresentationParameters.BackBufferWidth, graphics.PresentationParameters.BackBufferHeight);
            playerOne.LoadContent(contentManager, 2, screen);

            mainFont = contentManager.Load<SpriteFont>("mainFont");

            //initalize lists
            playerBullets = new List<Bullet>();
            //enemyBullets = new Bullet[10];
            enemyBullets = new List<Bullet>((int)(2 + (float)selectedDifficulty)); //allow more enemy bullets for higher difficulties
            enemies = new List<Enemy>();
            Enemy.Content = contentManager;    //pass the content manager to the enemy class (static variable) so that is can load assets when created
            particleEffectsList = new List<Explosion>();

            //set up the starfield
            Texture2D starTexture = contentManager.Load<Texture2D>("star");
            myStars = new Starfield(
                graphics.PresentationParameters.BackBufferWidth,
                graphics.PresentationParameters.BackBufferHeight,
                60, starTexture, false);

            //ask the user if they wish to stop their music and use the games.
            if (MediaPlayer.State == MediaState.Playing || MediaPlayer.State == MediaState.Paused)
                switch (MessageBox.Show("Mediaplayer is currently running; would you like to stop it and use the game's music?", "MediaPlayer is running!", MessageBoxButton.OKCancel))
                {
                    case MessageBoxResult.OK:
                        useGameMusic = true;
                        break;
                    case MessageBoxResult.Cancel:
                    //fall through
                    default:
                        useGameMusic = false;
                        break;
                }

            playerOne.Location = new Vector2((screen.X / 2) - 60, screen.Y - 60);

            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            if ( useGameMusic )
                MediaPlayer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            // TODO: Add your update logic here

            TouchCollection touches = TouchPanel.GetState();
            float totalX = 0, totalY = 0;
            int i;
            for (i = 0; i < touches.Count; i++)
            {
                totalX += touches[i].Position.X;    //get the total x and y position of all touches
                totalY += touches[i].Position.Y;
            }

            //get the average of our touch locations and scale them so they work like they should
            //Vector2 averageTouchLocation = new Vector2((totalX / i) / scalingFactor.X, (totalY / i) / scalingFactor.Y);
            Vector2 averageTouchLocation = new Vector2(totalX / i, totalY / i);

            

            playerOne.Update( averageTouchLocation );
            //PlayerFiring();  //checks if playerOne.IsFiring == true and handles bullets accordingly

            HandleBullets();
            HandleEnemies(timer.UpdateInterval);
            HandleAllCollision(timer.UpdateInterval);

            #region Concerning Particles
            //update particle effects and remove as neccessary
            for (int j = 0; j < particleEffectsList.Count; j++)
            {
                particleEffectsList[j].Update(timer.UpdateInterval);

                if (Explosion.CheckAllParticlesTransparent(particleEffectsList[j]))
                {   //if all particles are transparent then we can remove the effect
                    particleEffectsList.RemoveAt(j);
                    j--; //dont want to skip anything in the list!
                }
            }

            //update the starfield
            myStars.Update();
            #endregion

            /*levelController.Update(gameTime);
                if (levelController.SpawnNewWave)
                {
                    Random aGen = new Random();
                    //pick random location for wave
                    Vector2 origin = GameLevelController.WaveSpawnLocation[aGen.Next(1,GameLevelController.NumWavesInFolder+1)];
                    levelController.ReleaseWaves(ref enemies, (float)selectedDifficulty, origin, playerOne.Centre);
                }*/

            if (playerOne.Lives == 0)
            {   //if the player has run out of lives then it's game over!
                NavigationService.Navigate(new Uri("/ScorePage.xaml?score="+playerOne.Score+"?name="+playerOne.Name, UriKind.Relative));
            }
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            myStars.Draw(spriteBatch);

            foreach (Bullet bullet in enemyBullets)
            {
                //if (bullet != null) --> no longer using an array(see top of class)
                //{
                bullet.Draw(spriteBatch);
                //}
            }

            foreach (Bullet bullet in playerBullets)
            {
                bullet.Draw(spriteBatch);
            }//end foreach

            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch, timer.UpdateInterval);
            }//end foreach

            foreach (Explosion explosion in particleEffectsList)
            {
                explosion.Draw(spriteBatch);
            }

            //we pass the time elapsed since last update for animation purposes
            playerOne.Draw(spriteBatch, mainFont, timer.UpdateInterval);

            spriteBatch.End();
        }

        /// <summary>
        /// checks if player wishes to fire and creates bullets according o player position and gun cooldown
        /// this is not done in the player class as bullet's are their own object and this is easier for collision purposes etc...
        /// </summary>
        private void PlayerFiring()
        {
            if (playerOne.GunCooldown == 0 && playerOne.IsFiring == true)
            {
                //play a sound when the player fires their weapon
                SoundEffectInstance playerLaser = laserFire.CreateInstance();
                playerLaser.Play();

                //position to spawn bullet is centre of the player (the player location, adjust for player's centre, adjust for bullet width)
                Vector2 playerBulletOffset = playerOne.Location + new Vector2((playerOne.Width / 2.0f) - (defaultBulletTex.Width / 2.0f) - 1, 0); //-1 at the end here makes sure bullet is exactly in position (this calculation always seems to put it 1 pixel off)

                //if player's gun is cool, add a bullet to our list
                playerBullets.Add(new Bullet(playerBulletOffset, playerBulletTex, -1));
                //reset gun cooldown
                playerOne.ResetGunCooldown();
            }//end if
        }//end method

        #region Entity & Collision Handling (enemies, bullets, etc)
        /// <summary>
        /// simple method that calls Update() on each bullet in the bullet lists
        /// </summary>
        private void HandleBullets()
        {
            for (int i = 0; i < playerBullets.Count; i++)
            {
                playerBullets[i].Update();
                if (OffScreen.Check((int)screen.Y, (int)screen.X, playerBullets[i].BoundingBox) || playerBullets[i].FlaggedForRemoval == true)
                {
                    playerBullets.RemoveAt(i);
                    i--;//dont want to skip anyhting in the list
                }
            }
            for (int i = 0; i < enemyBullets.Count; i++)
            {
                //if (enemyBullets[i] != null) --> no longer using array (see top of class)
                //{
                enemyBullets[i].Update();
                if (OffScreen.Check((int)screen.Y, (int)screen.X, enemyBullets[i].BoundingBox) || enemyBullets[i].FlaggedForRemoval == true)
                {
                    enemyBullets.RemoveAt(i);
                    i--;//dont want to skip anything in the list!
                    //   }
                }//end null check
            }
            //TODO: handle other bullet types
            //TODO: include offscreen check
        }

        /// <summary>
        /// Calls or handles all the updating and spawning for all enemies in the game
        /// </summary>
        private void HandleEnemies(TimeSpan time)
        {
            //////////////////////////////////////
            //TODO: add game state conditionals//
            /////////////////////////////////////
            Random aGen = new Random();
            if (aGen.Next(0, 101) % 35 == 0)
            {
                enemies.Add(new HomingEnemy(aGen.Next(30, (int)screen.X), -50, 3, "homingEnemy", (float)selectedDifficulty, playerOne.Centre));
                enemies.Add(new Enemy(aGen.Next(30, (int)screen.X), -50, 2, "enemy", (float)selectedDifficulty));
                if (aGen.Next() % 2 == 0)
                {
                    enemies.Add(new DiagonalEnemy(-50, aGen.Next(30, 401), 2, "enemy2", (float)selectedDifficulty));
                    enemies.Add(new HorizontalEnemy((int)screen.X, aGen.Next(0, (int)screen.Y - 50), 2, "enemyh", (float)selectedDifficulty));

                }
                else
                {
                    enemies.Add(new DiagonalEnemy((int)screen.X, aGen.Next(30, 401), 2, "enemy2", (float)selectedDifficulty));
                    enemies.Add(new HorizontalEnemy(-50, aGen.Next(0, (int)screen.Y - 50), 2, "enemyh", (float)selectedDifficulty));
                }

                if (playerOne.Score % 250 == 5)
                {
                    enemies.Add(new Boss(aGen.Next(10, 331), -100, 4, "boss", (float)selectedDifficulty));
                }
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].IsBoss)       //try
                {                           //only works for boss, else will throw NotImplementedException, which will be caught and enemy will update normally
                    enemies[i].Update(ref enemyBullets, playerOne.Centre);  //tell enemy to update
                }
                else //enemy is normal      //catch //(NotImplementedException ex) 
                {
                    enemies[i].Update();
                }
                //remove the enemy if is is dead or if it is off the screen and it's allowance is up
                if (enemies[i].Health <= 0 || (OffScreen.Check((int)screen.Y, (int)screen.X, enemies[i].BoundingBox) == true && enemies[i].OffScreenAllowance <= 0))
                {
                    if (enemies[i].Health <= 0)
                    {   //if player killed the enemy, then add particle effects
                        Vector2 origin = new Vector2(enemies[i].Location.X + enemies[i].CollisionMask.Bounds.Center.X, enemies[i].Location.Y + enemies[i].CollisionMask.Bounds.Center.Y);
                        Explosion newExplosion = new Explosion(origin, enemies[i].ExplosionSize, 5, 3, time);
                        particleEffectsList.Add(newExplosion);
                        playerOne.EnemiesKilled++;
                    }
                    enemies.RemoveAt(i);
                    i--;//we dont want to skip anything in the list
                }
            }//end for
        }//end HandleEnemies()

        private void HandleAllCollision(TimeSpan time)
        {
            for (int enemyIndex = 0; enemyIndex < enemies.Count; enemyIndex++)  //for every single enemy that exists
            {
                for (int bulletIndex = 0; bulletIndex < playerBullets.Count; bulletIndex++)  //for every single bullet that exists
                {
                    #region Standard Collision
                    if (enemies[enemyIndex].FloatRotation == 0) //changed from: try
                    {
                        if (Collision.BoundingBox(playerBullets[bulletIndex].BoundingBox, enemies[enemyIndex].BoundingBox))  //check is bounding boxes intersect
                        {
                            if (Collision.PerPixel(playerBullets[bulletIndex].BoundingBox, playerBullets[bulletIndex].Texture, enemies[enemyIndex].BoundingBox, enemies[enemyIndex].CollisionMask))  //per pixel collision to check if they Actually collided
                            {
                                //subtract bullet power from enemy health
                                //then remove the bullet and increase player score and shield
                                enemies[enemyIndex].Health -= playerBullets[bulletIndex].Damage;
                                playerBullets[bulletIndex].FlaggedForRemoval = true;
                                playerOne.Score += 5;
                                playerOne.RestoreShield(5);
                            }
                        }//end if
                    }
                    #endregion
                    #region Rotated Collision
                    else //changed from: catch //(InvalidOperationException ex)    //this exception will be thrown if we tried to get the bounding box of an enemy that had a rotation
                    {                                       //(normal bounding box will not work with rotation)
                        if (Distance(playerBullets[bulletIndex].Location, enemies[enemyIndex].Location) < 100)
                        {
                            Matrix bulletTransform = Matrix.CreateTranslation(new Vector3(playerBullets[bulletIndex].Location, 0.0f));
                            Matrix enemyTransform = enemies[enemyIndex].Rotation;

                            //try the other overload for PerPixel collision, seeing as how the last one threw this exception
                            if (Collision.PerPixel(playerBullets[bulletIndex].Texture, bulletTransform, enemies[enemyIndex].CollisionMask, enemyTransform))
                            {
                                //subtract bullet power from enemy health
                                //then remove the bullet and increase player score and shield
                                enemies[enemyIndex].Health -= playerBullets[bulletIndex].Damage;
                                playerBullets[bulletIndex].FlaggedForRemoval = true;
                                playerOne.Score += 5;
                                playerOne.RestoreShield(5);
                            }
                        }
                    #endregion
                    }
                }//end bullet-enemy collision check

                #region Standard Collision
                if (enemies[enemyIndex].FloatRotation == 0)
                {
                    if (Collision.BoundingBox(playerOne.BoundingBox, enemies[enemyIndex].BoundingBox))  //check if bounding box of enemy inersects player's bounding box
                    {   //if the bounding boxes intersect, do perPixel collision
                        if (Collision.PerPixel(playerOne.BoundingBox, playerOne.CollisionMask, enemies[enemyIndex].BoundingBox, enemies[enemyIndex].CollisionMask)) //width and texture passed sepaerately because multiple frames in one texture
                        {
                            if (!playerOne.IsShielding)
                            {
                                //hurt player if not shielding
                                playerOne.Health -= 5;
                            }
                            //hurt enemy
                            enemies[enemyIndex].Health -= 10;
                            vibrate.Start(TimeSpan.FromMilliseconds(100));
                        }
                    }//end enemy-player collision check
                }
                #endregion
                #region Rotated Collision
                else //changed from: catch //(InvalidOperationException ex)    //this exception will be thrown if we tried to get the bounding box of an enemy that had a rotation
                {                                       //(normal bounding box will not work with rotation)
                    if (Distance(playerOne.Centre, enemies[enemyIndex].Location) < 100)
                    {
                        Matrix playerTransform = Matrix.CreateTranslation(new Vector3(playerOne.Location, 0.0f));
                        Matrix enemyTransform = enemies[enemyIndex].Rotation;

                        //try the other overload for PerPixel collision, seeing as how the last one threw this exception
                        if (Collision.PerPixel(playerOne.CollisionMask, playerTransform, enemies[enemyIndex].CollisionMask, enemyTransform))
                        {
                            if (!playerOne.IsShielding)
                            {
                                //hurt player if not shielding
                                playerOne.Health -= 5;
                            }
                            //hurt enemy
                            enemies[enemyIndex].Health -= 10;
                            vibrate.Start(TimeSpan.FromMilliseconds(100));
                        }
                    }
                }
                #endregion
            }//finished enemy collision checking

            for (int i = 0; i < enemyBullets.Count; i++)
            {
                //    if (enemyBullets[i] != null) --> no longer using array (see top of class)
                //    {
                if (Collision.BoundingBox(playerOne.BoundingBox, enemyBullets[i].BoundingBox))  //check if bounding boxes intersect
                {
                    if (Collision.PerPixel(enemyBullets[i].BoundingBox, enemyBullets[i].Texture, playerOne.BoundingBox, playerOne.CollisionMask))  //per pixel collision to check if they Actually collided
                    {
                        if (!playerOne.IsShielding)
                        {
                            //subtract bullet power from health
                            //then remove the bullet
                            playerOne.Health -= enemyBullets[i].Damage;
                            enemyBullets[i].FlaggedForRemoval = true;
                            playerOne.RestoreShield(5); //intentional
                        }

                        //create a particle effect to show that the the bullet has it the player
                        particleEffectsList.Add(new Explosion(playerOne.Centre, 5, 2, 2, time));
                    }//end if
                }//end if
                //   }//end null check
            }//finished enemyBullet - player collision
        }//end HandleAllCollision()
        #endregion

        private double Distance(Vector2 locationA, Vector2 locationB)
        {
            double distance;

            double deltaX, deltaY;
            deltaX = locationB.X - locationA.X;
            deltaY = locationB.Y - locationA.Y;

            distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            return distance;
        }

        #region PROPERTIES
        public int PlayerScore
        {
            get { return playerOne.Score; }
            //no set
        }
        public int EnemiesKilled
        {
            get { return playerOne.EnemiesKilled; }
            set { playerOne.EnemiesKilled = value; }
        }
        //public bool IsPaused
        //{
        //    get { return paused; }
        //    //no set - should be done with a method call
        //}
        //public Song Music
        //{
        //    get { return bgMusic; }
        //    set { bgMusic = value; }
        //}
        /*public string PlayerName
        {
            get { return playerOne.Name; }
            set { playerOne.Name = value; }
        } */
        #endregion
        #endregion
    }
}