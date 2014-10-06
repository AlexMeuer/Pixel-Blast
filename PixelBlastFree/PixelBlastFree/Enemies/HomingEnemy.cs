using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelBlastFree
{
    class HomingEnemy : Enemy
    {
        Vector2 direction;

        /// <summary>
        /// Creates a new instance of HomingEnemy, which will go toward the vector supplied (and not stop at it but continue on).
        /// </summary>
        /// <param name="destination">The vector position that the enemy is to move towards(and past)</param>
        public HomingEnemy(int xStart, int yStart, int numberOfFrames, string textureToLoad, float difficultyFactor, Vector2 destination)
            :base(xStart, yStart, numberOfFrames, textureToLoad, difficultyFactor)
        {

            numberOfParticles *= 2; //we want a bigger explosion
            health *= 2;  //this is a stronger enemy

            //get the direction by subtracting our location from target location
            direction = destination - location;
            direction.Normalize();  //it is a direction vector so we want to normalize it (give it length of 1)

            //turn to face the target
            rotation = (float)(Math.Atan2(direction.Y, direction.X) + MathHelper.PiOver2);
        }

        public override void Update()
        {
            location += direction * MaxSpeed;
        }
    }
}
