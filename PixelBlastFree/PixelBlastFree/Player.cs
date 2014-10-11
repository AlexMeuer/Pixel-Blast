using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace PixelBlastFree
{
    class Player
    {
        #region Variables
        Texture2D spriteSheet, collisionMask, solidColor, shieldTex;  //texture for player, solid color for stat display, shield texture

        int enemiesKilled;
        int score, health, shield, MaxHealth, MaxShield;  //player stats
        string name;    //player's name

        const byte MaxSpeed = 10;
        byte lives, gunCooldown;

        Rectangle healthRec, shieldRec; //for health and shield display
        Vector2 newLocation, location;               //where the player is on the screen

        bool shieldActive = false, isFiring = true;      //whether the player is currently using their shield and whether the player wants to fire their weapons
        //Viewport viewport;  //is there a better way to do this?
        Vector2 viewport;

        //used for animation
        TimeSpan timeSinceLastFrameChange;
        byte frameLength, currentFrame, frameWidth;
        #endregion

        #region CONSTRUCTOR(S)
        public Player(float difficultyFactor, Vector2 startingPosition)
        {
            //name = playerName;
            //viewport = graphics.GraphicsDevice.Viewport;

            //set no of lives. we subtract from 8 so that easy mode had 5 lives and extreme has one
            lives = (byte)(8 - difficultyFactor);

            difficultyFactor = (difficultyFactor / 4.0f) + 1;  //+1 is added to make things scale upwards as difficulty decreases
            //set shield and health - will get smaller with harder difficulty levels
            shield = (int)(255 * 1/difficultyFactor);
            health = (int)(255 * 1/difficultyFactor);
            MaxShield = (int)(255 * 1/difficultyFactor);
            MaxHealth = (int)(255 * 1/difficultyFactor);

            //create rectangles to display health and shield levels
            healthRec = new Rectangle(10, (int)viewport.Y - 50, 10, health / 5);
            healthRec.Y -= (health / 2) + 20;
            shieldRec = new Rectangle(20, (int)viewport.Y - 50, 10, shield /5);
            shieldRec.Y -= (shield / 2) + 20;

            location = startingPosition;
            newLocation = location;

            gunCooldown = 0;
            timeSinceLastFrameChange = TimeSpan.Zero;
            frameLength = 85;   //milliseconds
        }
        #endregion

        #region METHODS
        public void LoadContent(ContentManager content, byte numberOfFrames, Vector2 screenBounds)
        {
            //load texture for the player and solid color texture(1x1 pixels) for health and shield display
            spriteSheet = content.Load<Texture2D>("player/player");
            solidColor = content.Load <Texture2D>("solidColor");
            shieldTex = content.Load<Texture2D>("player/playerShield");

            viewport = screenBounds;

            //set up collision mask, whos width and height is the size of each frame
            frameWidth = (byte)(spriteSheet.Width / numberOfFrames);    //spritesheets must be 1 frame tall
            collisionMask = ModifyTexture2D.Crop(spriteSheet, new Rectangle(0, 0, frameWidth, spriteSheet.Height));
        }

        public void Update(Vector2 touchLocation)
        {
            //float shieldVibration = 0.0f;   //used to determine what value to use when setting gampepad vibration
            if (health <= 0)
            {
                lives -= 1;
                RestoreHealth();
                RestoreShield();
            }

            UpdateHealthAndShieldDisplay(); //handles the display rectangles for health and shield levels

            ////process gamePad input only if connected
            //if (currentGamePadState.IsConnected)
            //#region GamePadInput
            //{
            //    #region Movement
            //    float xMove = currentGamePadState.ThumbSticks.Left.X;
            //    float yMove = currentGamePadState.ThumbSticks.Left.Y;

            //    //do x movement
            //    float distance = xMove * MaxSpeed;
            //    if (xMove > 0.2f || xMove < 0.2f)  //account for dead zones
            //    {
            //        location.X += distance;
            //    }

            //    //do y movement
            //    distance = yMove * MaxSpeed;
            //    if (yMove > 0.2f || yMove < 0.2f)
            //    {
            //        location.Y -= distance;
            //    }
            //    #endregion

            //    #region Shielding & firing
            //    if (currentGamePadState.Triggers.Left > 0.1f && shield >= 1)   //does the player want to shield
            //    {
            //        shieldActive = true;
            //        shieldVibration = shield / (float)MaxShield;  //slow the vibration as the shield wears off
            //        shield--;   //reduce shield level
            //    }
            //    else
            //    {
            //        shieldActive = false;
            //    }

            //    if (currentGamePadState.Triggers.Right > 0.2f) //does the player want to fire
            //    {
            //        isFiring = true;
            //    }
            //    else
            //    {
            //        isFiring = false;
            //    }
            //    #endregion

            //    GamePad.SetVibration(PlayerIndex.One,
            //                            shieldVibration, /*slow moter vibrates in accordance with shield level*/
            //                            (float)Math.Min(gunCooldown / 10, 1));  //fast motor vibrates in accordance with gun fire

            //}//end if(currentState.IsConnected) 
            //#endregion
            //else //take keyboard input
            //#region KeyboardInput
            //{
            //    //unforunately we can only use max speed when using keyboard
            //    #region Movement
            //    //do y movement
            //    if (currentKeyboardState.IsKeyDown(Keys.W) || currentKeyboardState.IsKeyDown(Keys.Up))
            //    {
            //        location.Y -= MaxSpeed;
            //    }
            //    else if (currentKeyboardState.IsKeyDown(Keys.S) || currentKeyboardState.IsKeyDown(Keys.Down))
            //    {
            //        location.Y += MaxSpeed;
            //    }

            //    //do x movement
            //    if (currentKeyboardState.IsKeyDown(Keys.D) || currentKeyboardState.IsKeyDown(Keys.Right))
            //    {
            //        location.X += MaxSpeed;
            //    }
            //    else if (currentKeyboardState.IsKeyDown(Keys.A) || currentKeyboardState.IsKeyDown(Keys.Left))
            //    {
            //        location.X -= MaxSpeed;
            //    }
            //    #endregion
            //    #region Shielding/Firing
            //    //accept multiple keys for firing/shielding to allow for hand position that suits player
            //    if ((currentKeyboardState.IsKeyDown(Keys.LeftControl) || currentKeyboardState.IsKeyDown(Keys.RightControl) || currentKeyboardState.IsKeyDown(Keys.LeftAlt)) && shield >= 1)
            //    {
            //        shieldActive = true;
            //        shield--;   //reduce shield level
            //    }
            //    else
            //    {
            //        shieldActive = false;
            //    }

            //    if (currentKeyboardState.IsKeyDown(Keys.Space) || currentKeyboardState.IsKeyDown(Keys.LeftShift))
            //    {
            //        isFiring = true;
            //    }
            //    else
            //    {
            //        isFiring = false;
            //    }
            //    #endregion
            //}
            //#endregion

            if ( float.IsNaN(touchLocation.X) )
            {
                if (shield >= 1)
                {
                    shieldActive = true;
                    shield--;   //reduce shield level
                }
                else
                {
                    shieldActive = false;
                }
            }
            else
            {
                shieldActive = false;
                //newLocation = touchLocation;
                newLocation = Vector2.Clamp(touchLocation, new Vector2(0, 500), new Vector2(500, 800));
            }

            location = Vector2.SmoothStep(location, newLocation, 0.2f);

            //restrict the player to within the screen
            if (location.X < 0)
            {
                location.X = 0;
            }
            else if (location.X > viewport.X - frameWidth)
            {
                location.X = viewport.X - frameWidth;
            }
            if (location.Y < 0)
            {
                location.Y = 0;
            }
            else if (location.Y > viewport.Y - spriteSheet.Height)
            {
                location.Y = viewport.Y - spriteSheet.Height;
            }

            if (gunCooldown > 0)    //reduce cooldown by one if gun is hot
            {
                gunCooldown--;
            }
        }//end Update()

        public void Draw(SpriteBatch spriteBatch, SpriteFont sFont, TimeSpan timeElapsed)
        {
            //get the time since we last changed frame
            timeSinceLastFrameChange += timeElapsed;

            //draw the player
            Animate(spriteBatch, timeElapsed);  //draws the correct frame of the sprite

            //draw the lives
            spriteBatch.DrawString(sFont, "" + lives + " lives", new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(sFont, "\nScore: " + score, new Vector2(10, 30), Color.Gold);

            //display health and shield levels as visible rectangles
            spriteBatch.Draw(solidColor, shieldRec, Color.Blue * 0.85f);
            spriteBatch.Draw(solidColor, healthRec, Color.Red * 0.85f);
        }

        private void UpdateHealthAndShieldDisplay()
        {
            healthRec.Height = health;
            healthRec.Y = 430 - (health);
            shieldRec.Height = shield;
            shieldRec.Y = 430 - (shield);
        }

        private void Animate(SpriteBatch spriteBatch, TimeSpan totalTimeElapsed)
        {
            Texture2D textureToDraw = spriteSheet;  //we hold the texture we want to draw in a variable to avoid duplicate code (see below)

            //change frame after frameLength amount of milliseconds
            if (timeSinceLastFrameChange >= TimeSpan.FromMilliseconds(frameLength))
            {
                if (currentFrame == 0)
                { currentFrame = 1; }
                else //(currentFrame == 1)
                { currentFrame = 0; }

                timeSinceLastFrameChange = TimeSpan.Zero;   //reset the frame timing
            }

            if (shieldActive)
            {   //if player has shield activated, set the texture we want to draw to be the shield texture(else is would be the default texture, see above)
                textureToDraw = shieldTex;
            }

            //destRec is where we want to draw the player texture
            Rectangle destRec = new Rectangle((int)location.X, (int)location.Y, frameWidth, spriteSheet.Height);
            //source rec is what part of the texture we want to draw. "(CurrentFrame - 1)" makes sure the correct position for each frame is chosen. This can now be used for any texture, any amount of frames, etc...
            Rectangle sourceRec = new Rectangle(frameWidth * currentFrame, 0, frameWidth, spriteSheet.Height);
            spriteBatch.Draw(textureToDraw, destRec, sourceRec, Color.White);

        }//end Animate()
        #endregion

        #region Restore/reset stat methods
        private void RestoreShield()
        {
            shield = MaxShield;
        }
        public void RestoreShield(byte amount)
        {
            shield += amount;
            if (shield > MaxShield)
            {
                shield = MaxShield;
            }
        }
        private void RestoreHealth()
        {
            health = MaxHealth;
        }
        public void RestoreHealth(byte amount)
        {
            health += amount;
            if (health > MaxHealth)
            {
                health = MaxHealth;
            }
        }
        public void ResetGunCooldown()
        {
            gunCooldown = 15;
        }
        #endregion
        #region PROPERTIES
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public Vector2 Centre
        {
            get { return new Vector2(BoundingBox.Center.X, BoundingBox.Center.Y); }
        }
        public int EnemiesKilled
        {
            get { return enemiesKilled; }
            set { enemiesKilled = value; }
        }
        public byte Width
        {
            get { return frameWidth; }
            //no set
        }
        public Texture2D CollisionMask
        {
            get { return collisionMask; }
            //no set - only meant for per pixel collision
        }
        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)location.X, (int)location.Y, frameWidth, spriteSheet.Height); }
            //no set
        }
        public byte Shield
        {
            get {
                    return (byte)shield;
                }
            set {
                    shield = value;
                    if(shield > MaxShield)
                    {
                        shield = MaxShield;
                    }
                }
        }
        public int Health
        {
            get
            {
                return health;
            }
            set
            {
                health = value;
                if (health > MaxHealth)
                {
                    health = MaxHealth;
                }
            }
        }
        public byte Lives
        {
            get { return lives; }
            set { lives = value; }
        }
        public bool IsFiring
        {
            get { return isFiring; }
            //no set
        }
        public bool IsShielding
        {
            get { return shieldActive; }
            //no set
        }
        public byte GunCooldown
        {
            get { return gunCooldown; }
            set { gunCooldown = value; }
        }
        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }
        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        #endregion
    }
}