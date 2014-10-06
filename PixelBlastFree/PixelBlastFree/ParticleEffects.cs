using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelBlastFree
{
    //particle is not a full class of its own. Struct is more efficient in this case since we're just holding information
    /// <summary>
    /// A set of values with a texture that I am calling a particle.
    /// Intended to be used with the classes below.
    /// </summary>
    struct Particle
    {
        float spawnTime, maxAge;   //time that the particle spawned at and the maximum life of the particle in seconds
        float timeSinceSpawn, rotation, scale;
        Vector2 spawnPosition, acceleration, direction, currentPosition;    //where the particle spawned, the acceleration of it, it's direction and where it is now
        Color colour;
        static Texture2D defaultTexture;    //default texture of a particle
        Texture2D texture;  //actual texture of the particle

        #region PROPERTIES
        public static Texture2D DefaultTexture
        {
            get { return defaultTexture; }
            set { defaultTexture = value; }
        }
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        public float SpawnTime
        {
            get { return spawnTime; }
            set { spawnTime = value; }
        }
        public float MaxAge
        {
            get { return maxAge; }
            set { maxAge = value; }
        }
        public Vector2 SpawnPosition
        {
            get { return spawnPosition; }
            set { spawnPosition = value; }
        }
        public Vector2 CurrentPosition
        {
            get { return currentPosition; }
            set { currentPosition = value; }
        }
        public Vector2 Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }
        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        public Color Colour
        {
            get { return colour; }
            set { colour = value; }
        }
        public float TimeSinceSpawn
        {
            get { return timeSinceSpawn; }
            set { timeSinceSpawn = value; }
        }
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        #endregion
    }

    /// <summary>
    /// emits a group of particles outwards in a random direction from it's origin.
    /// the particles fade out when maxAge is exceeded.
    /// </summary>
    class Explosion
    {
        Particle[] particleArray;
        #region CONSTRUCTORS
        /// <param name="origin">the centre of the explosion</param>
        /// <param name="numberOfParticles">how many particles the explosion will have</param>
        /// <param name="radius">the radius of the explosion</param>
        /// <param name="maxAge">the maximum amount of time(in seconds) that a particle can live for</param>
        public Explosion(Vector2 origin, int numberOfParticles, float radius, float maxAge, TimeSpan time)
        {
            Random aGen = new Random();
            particleArray = new Particle[numberOfParticles];

            for (int i = 0; i < particleArray.Length; i++)
            {
                //create a new empty particle
                particleArray[i] = new Particle();

                //set all variables of the particle
                particleArray[i].SpawnPosition = origin;
                particleArray[i].CurrentPosition = origin;
                particleArray[i].MaxAge = maxAge;
                particleArray[i].SpawnTime = (float)time.TotalSeconds;
                particleArray[i].Colour = Color.White;

                //pick a random direction
                particleArray[i].Direction = PickRandomDirection(aGen, radius);
                particleArray[i].Acceleration = 3.0f * particleArray[i].Direction;
                particleArray[i].Direction.Normalize(); //make it a proper direction vector of length 1

                //set the texture
                particleArray[i].Texture = Particle.DefaultTexture;

                //set random rotation and scale
                particleArray[i].Rotation = aGen.Next();
                particleArray[i].Scale = (float)aGen.NextDouble() + 0.25f; //+0.25f makes sure that particle is never too small
            }
        }//end constructor

        /// <summary>
        /// same as other constructor except that we may specify a custom texture for the particles
        /// </summary>
        /// <param name="particleTexture">the texture to be given to each particle</param>
        public Explosion(Vector2 origin, int numberOfParticles, float radius, float maxAge, TimeSpan time, Texture2D particleTexture)
        {
            Random aGen = new Random();
            particleArray = new Particle[numberOfParticles];

            for (int i = 0; i < particleArray.Length; i++)
            {
                //create a new empty particle
                particleArray[i] = new Particle();

                //set all variables of the particle
                particleArray[i].SpawnPosition = origin;
                particleArray[i].CurrentPosition = origin;
                particleArray[i].MaxAge = maxAge;
                particleArray[i].SpawnTime = (float)time.TotalSeconds;
                particleArray[i].Colour = Color.White;

                //pick a random direction
                particleArray[i].Direction = PickRandomDirection(aGen, radius);
                particleArray[i].Acceleration = 3.0f * particleArray[i].Direction;
                particleArray[i].Direction.Normalize(); //make it a proper direction vector of length 1

                //set the texture
                particleArray[i].Texture = particleTexture;

                //set random rotation and scale
                particleArray[i].Rotation = aGen.Next();
                particleArray[i].Scale = (float)aGen.NextDouble() + 0.25f; //+0.25f makes sure that particle is never too small
            }
        }//end constructor 
        #endregion

        #region METHODS
        /// <summary>
        /// Generates a random direction vector
        /// </summary>
        /// <param name="aGen">the generator to be used</param>
        /// <param name="size">the size of the explosion</param>
        /// <returns></returns>
        private Vector2 PickRandomDirection(Random aGen, float size)
        {
            float distance, angle;
            Vector2 direction;

            distance = (float)(aGen.NextDouble() * size);   //how far the particle should have travelled by the end of its life
            direction = new Vector2(distance, 0);   //create a vector with our distance
            angle = MathHelper.ToRadians(aGen.Next(360));   //generate a random angle from 0 to 359

            direction = Vector2.Transform(direction, Matrix.CreateRotationZ(angle));    //rotate our direction vector by the angle

            return direction;
        }//end PickRandomDirection()

        public void Update(TimeSpan time)
        {
            float deltaTime = (float)time.TotalSeconds;

            for (int i = 0; i < particleArray.Length; i++)
            {
                particleArray[i].CurrentPosition += particleArray[i].Direction;
                particleArray[i].Rotation += deltaTime * 2;

                particleArray[i].TimeSinceSpawn += deltaTime;
                if (particleArray[i].TimeSinceSpawn > particleArray[i].MaxAge)
                {   //fade out if lifespan is over
                    particleArray[i].Colour *= 0.95f;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < particleArray.Length; i++)
            {
                Vector2 origin = new Vector2(particleArray[i].Texture.Bounds.Center.X, particleArray[i].Texture.Bounds.Center.Y);   //origin for rotation

                spriteBatch.Draw(particleArray[i].Texture, particleArray[i].CurrentPosition, null, particleArray[i].Colour, particleArray[i].Rotation, origin, 1, SpriteEffects.None, 1);
            }
        }

        /// <summary>
        /// static method to check if all particles are transparent. Threshold for alpha is 15.
        /// </summary>
        /// <param name="explosion">the explosion effect that contains the particles to check</param>
        /// <returns>true if all particles in the effect have an aplha value below the threshold; else false</returns>
        public static bool CheckAllParticlesTransparent(Explosion explosion)
        {
            bool allParticlesAreTransparent = true;
            const byte threshold = 15;   //what we will accept as transparent enough (we're probably using something like 1/time to set transparency over time)

            foreach (Particle particle in explosion.particleArray)
            {
                if (particle.Colour.A > threshold)
                    allParticlesAreTransparent = false;
            }

            return allParticlesAreTransparent;
        }
        #endregion
    }

    /// <summary>
    /// spawns particles at the top edge of the screen(going downwards) to give the feel of travel and depth
    /// </summary>
    class Starfield
    {
        Particle[] particleArray;   //array to hold all our particles
        Rectangle screenBounds; //so we can move stars when they dissappear off the screen
        bool rotateStars;   //do we want stars to rotate as they fall?

        /// <summary>
        /// creates a new instance of Starfield at the point (0,0) that fills the screen
        /// </summary>
        /// <param name="rotate">set to true if you want stars to rotate as they fall</param>
        public Starfield(int screenWidth, int screenHeight, int numberOfStars, Texture2D starTexture, bool rotate)
        {
            rotateStars = rotate;
            Random aGen = new Random();
            screenBounds = new Rectangle(0, 0, screenWidth, screenHeight);
            particleArray = new Particle[numberOfStars];

            for (int i = 0; i < particleArray.Length; i++)
            {
                //create a new empty particle
                particleArray[i] = new Particle();

                //set all variables of the particle
                particleArray[i].SpawnPosition = new Vector2(aGen.Next(0, screenBounds.Width), aGen.Next(0, screenBounds.Height));
                particleArray[i].CurrentPosition = particleArray[i].SpawnPosition;

                //vary the colour a little to make is more interesting
                if (aGen.NextDouble() > 0.90)
                    particleArray[i].Colour = Color.Gold;
                else if(aGen.NextDouble() < 0.10)
                    particleArray[i].Colour = Color.PaleTurquoise;
                else
                    particleArray[i].Colour = Color.White;

                //randomize the scale of the particle
                particleArray[i].Scale = (float)aGen.NextDouble();

                float distance = (screenHeight + aGen.Next(0, 151));   //how far the particle should have travelled by the end of its life (we vary this so the stars dont all travel at the same speed
                particleArray[i].Direction = new Vector2(0, distance / 5000.0f);
                particleArray[i].Direction *= (float)aGen.NextDouble() + 0.5f;  //randomize speed a little
                //particleArray[i].Direction.Normalize();
                particleArray[i].Texture = starTexture;

                particleArray[i].Rotation = aGen.Next(0,6);    //apply random rotation (when nor arguement is suppied, the resulting number (for whatever reason) is not modified in Update(), even when rotateStars - true
            }
        }

        public void Update()
        {
            Random aGen = new Random();

            for (int i = 0; i < particleArray.Length; i++)
            {
                particleArray[i].CurrentPosition += particleArray[i].Direction;

                if (rotateStars == true)
                {
                    float originalRotation = particleArray[i].Rotation;
                    particleArray[i].Rotation = originalRotation + 0.01f;
                }

                //if the particle is off the screen, return it to the top
                if (OffScreen.Check(screenBounds.Height, screenBounds.Width, particleArray[i].CurrentPosition, particleArray[i].Texture.Width, particleArray[i].Texture.Height))
                {
                    particleArray[i].CurrentPosition = new Vector2(aGen.Next(0, screenBounds.Width), 0);
                }//end if
            }//end for
        }//end Update()

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Particle particle in particleArray)
            {
                //find the center of each particle by finding the center of one full size particle and multiplying the X and Y by the scale
                Vector2 origin = new Vector2(particle.Texture.Bounds.Width * particle.Scale, particle.Texture.Bounds.Height * particle.Scale);

                //draw all the particles
                spriteBatch.Draw(particle.Texture, particle.CurrentPosition, null, particle.Colour, particle.Rotation, origin, particle.Scale, SpriteEffects.None, 0);
            }//end foreach
        }//end Draw()
    }
}
