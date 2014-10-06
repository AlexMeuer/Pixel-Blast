using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PixelBlastFree
{
    class HorizontalEnemy : Enemy
    {
        sbyte direction;    //-1 = left, 1= right, 0 = no movement(shouldnt happen), greater numbers will multiply move speed

        //ALSO: implement a rotation matrix so that sprites fast correct direction of travel

        #region CONSTRUCTOR(S)
        /// <summary>
        /// Creates a new enemy that travels horzontally across the screen
        /// </summary>
        /// <param name="xStart">starting position of the new enemy</param>
        /// <param name="yStart">starting position of the new enemy</param>
        /// <param name="numberOfFrames">number of frames in the spritesheet</param>
        /// <param name="textureToLoad">asset name of the enemy's texture</param>
        /// <param name="difficultyFactor"></param>
        public HorizontalEnemy(int xStart, int yStart, int numberOfFrames, string textureToLoad, float difficultyFactor)
            :base(xStart, yStart, numberOfFrames, textureToLoad, difficultyFactor)
        {
            /* this stuff is done in the base constructor and so is not required here
            ------------------------
            //location.X = xStart;
            //location.Y = yStart;
            //health = (sbyte)(10 * difficultyFactor);    //modify health by difficulty
            //spriteSheet = content.Load<Texture2D>(textureToLoad); //object may not exist when LoadContent is called from game class
            //offscreenAllowance = 500;
             ------------------------*/

            if (xStart < 0) //set the x direction of travel
            {
                direction = 1;  //go right if we spawn of the left edge of the screen
                //we dont need to assign to spriteEffects here because texture is already facing correct direction
            }
            else
            {
                direction = -1; //go left if we spawn of the right side
                spriteEffects = SpriteEffects.FlipHorizontally; //flip the texture horizontally
            }
        }
        #endregion
        
        #region METHODS
        public override void Update()
        {
            location.X += direction * MaxSpeed;
            //decrement offscreen allowance untill zero
            if (offscreenAllowance > 0)
            {
                offscreenAllowance--;
            }
        }
        #endregion
    }
}
