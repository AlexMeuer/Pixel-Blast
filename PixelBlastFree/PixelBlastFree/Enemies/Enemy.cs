using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PixelBlastFree
{
    class Enemy
    {
        #region Class Level Variables
        protected Texture2D spriteSheet, collisionMask;
        protected float rotation;   //angle (radians) to rotate before drawing
        protected Vector2 origin;   //centre of rotation for the enemy
        protected SpriteEffects spriteEffects;  //default NONE. can be used to flip sprite horiontally or verically

        protected float health;
        protected Vector2 location;
        protected const byte MaxSpeed = 5;
        protected int offscreenAllowance;   //time allowed (for spawning purposes) that we are allowed offscreen without getting removed (time counts down from moment of spawn)

        //used for animation
        protected TimeSpan timeSinceLastFrameChange;
        protected byte frameLength, currentFrame, frameWidth;  //currentFrame means the frame of animation
        //framewidth is no longer used since i have included my game library and a collisionmask

        protected static ContentManager content;    //content manager so we can load assets 

        protected int numberOfParticles;    //the number of particles emitted when we are killed
        protected bool isBoss = false;
        #endregion

        #region CONSTRUCTOR(S)

        /// <summary>
        /// Creates a new enemy that falls vertically down the screen
        /// </summary>
        /// <param name="xStart">starting position of the new enemy</param>
        /// <param name="yStart">starting position of the new enemy</param>
        /// <param name="numberOfFrames">number of frames in the spritesheet</param>
        /// <param name="textureToLoad">asset name of the enemy's texture</param>
        /// <param name="difficultyFactor"></param>
        public Enemy(int xStart, int yStart, int numberOfFrames, string textureToLoad, float difficultyFactor)
        {
            location.X = xStart;
            location.Y = yStart;
            spriteSheet = content.Load<Texture2D>(textureToLoad);  //object may not exist when LoadContent is called from game class

            //set up collision mask, whos width and height is the size of each frame
            frameWidth = (byte)(spriteSheet.Width / numberOfFrames);    //spritesheets must be 1 frame tall
            collisionMask = ModifyTexture2D.Crop(spriteSheet, new Rectangle(0, 0, frameWidth, spriteSheet.Height));
            
            //set stats to default values
            health = (10 * difficultyFactor);    //modify health by difficulty
            offscreenAllowance = 2000;
            frameLength = 85;

            //set rotation to default (zero) and origin to centre of enemy. spriteEffects assigned to default (none).
            rotation = 0f;
            origin = new Vector2(collisionMask.Width / 2.0f, collisionMask.Height / 2.0f);
            spriteEffects = SpriteEffects.None;

            numberOfParticles = 5;
        }
        #endregion

        #region METHODS
        public virtual void Update()
        {
            //fall down the screen
            location.Y += MaxSpeed;
            //decrement offscreen allowance untill zero
            if (offscreenAllowance > 0)
            {
                offscreenAllowance--;
            }
        }

        public void Draw(SpriteBatch spriteBatch, TimeSpan totalTimeElapsed)
        {
            Animate(spriteBatch, totalTimeElapsed);
        }

        protected void Animate(SpriteBatch spriteBatch, TimeSpan totalTimeElapsed)
        {
            timeSinceLastFrameChange += totalTimeElapsed;

            //change frame after frameLength amount of milliseconds
            if (timeSinceLastFrameChange >= TimeSpan.FromMilliseconds(frameLength))
            {
                if (currentFrame == 0)
                { currentFrame = 1; }
                else //(currentFrame == 1)
                { currentFrame = 0; }

                timeSinceLastFrameChange = TimeSpan.Zero;   //reset the frame timing
            }

            //destRec is where we want to draw the player texture
            Rectangle destRec = new Rectangle((int)location.X, (int)location.Y, collisionMask.Width, collisionMask.Height);
            //source rec is what part of the texture we want to draw. This can now be used for any texture, any amount of frames, etc...
            Rectangle sourceRec = new Rectangle(collisionMask.Width * currentFrame, 0, collisionMask.Width, collisionMask.Height);

            spriteBatch.Draw(spriteSheet, destRec, sourceRec, Color.White, rotation, origin, spriteEffects, 0f);
        }
        #endregion

        #region PROPERTIES
        public static ContentManager Content
        {
            get { return content; }
            set { content = value; }
        }
        public float Health
        {
            get { return health; }
            set { health = value; }
        }
        public Texture2D CollisionMask
        {
            get { return collisionMask; }
            //no set - only meant for per pixel collision
        }
        public int OffScreenAllowance
        {
            get { return offscreenAllowance; }
            //no set
        }
        public bool IsBoss
        {
            get { return isBoss; }
            set { isBoss = value; }
        }
        public int ExplosionSize
        {
            get { return numberOfParticles; }
            set { numberOfParticles = value; }
        }
        /// <remarks>
        /// Originally threw an exception that was caught by the collision method
        /// (which then did rotated collision because bounding box wont work with rotation).
        /// When bounding box was used elsewhere, other han collision, i had to change it back.
        /// Now an if statement check if rotation is zero and picks the appropriate collision method.
        /// </remarks>
        public Rectangle BoundingBox
        {
            get
            {
                //if (rotation == 0)
                //{
                return new Rectangle((int)location.X, (int)location.Y, collisionMask.Width, collisionMask.Height);
                //}
                //else throw new InvalidOperationException("BoundingBox will not work with rotation.");
            }
            //no set
        }
        public float FloatRotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }
        /// <summary>
        /// returns the rotation of the enemy as a matrix so that it can be used for collision purposes
        /// </summary>
        public Matrix Rotation
        {                       //translate so that we will rotate around correct point    ->     apply rotation      ->       translate back
            get { return Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(location, 0.0f)); }
            //no set
        }
        #endregion

        //this is only to be used with enemies that fire bullets e.g. boss
        public virtual void Update(ref List<Bullet> enemyBullets, Vector2 vector2)
        {
            throw new NotImplementedException();
        }
    }
}
