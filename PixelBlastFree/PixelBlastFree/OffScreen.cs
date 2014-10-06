using Microsoft.Xna.Framework;

namespace PixelBlastFree
{
    public abstract class OffScreen
    {
        /// <summary>
        /// Check if something is off the edge of the screen.
        /// Returns true if object is off the screen.
        /// </summary>
        /// <param name="screenHeight">Height of the screen to be used for checking</param>
        /// <param name="screenWidth">Width of the screen to be used for checking</param>
        /// <param name="objectXpos">X position of the object</param>
        /// <param name="objectYpos">Y position of the object</param>
        /// <param name="objectWidth">Width of the object</param>
        /// <param name="objectHeight">Height of the object</param>
        /// <returns>boolean: true is the object is completely off the screen; otherwise returns false</returns>
        public static bool Check(int screenHeight, int screenWidth, int objectXpos, int objectYpos, int objectWidth, int objectHeight)
        {
            bool isOffScreen = false;

            if (objectXpos > screenWidth || objectYpos > screenHeight)
            {
                isOffScreen = true;
            }
            else if ((objectXpos + objectWidth) < 0 || (objectYpos + objectHeight) < 0)
            {
                isOffScreen = true;
            }
            
            return isOffScreen;
        }

        /// <summary>
        /// Check if something is off the edge of the screen.
        /// Returns true if object is off the screen.
        /// </summary>
        /// <param name="screenHeight">Height of the screen to be used for checking</param>
        /// <param name="screenWidth">Width of the screen to be used for checking</param>
        /// <param name="objectPosition">X and Y position of the object to check</param>
        /// <param name="objectWidth">Width of the object</param>
        /// <param name="objectHeight">Height of the object</param>
        /// <returns>boolean: true is the object is completely off the screen; otherwise returns false</returns>
        public static bool Check(int screenHeight, int screenWidth, Vector2 objectPosition, int objectWidth, int objectHeight)
        {
            bool isOffScreen = false;

            if (objectPosition.X > screenWidth || objectPosition.Y > screenHeight)
            {
                isOffScreen = true;
            }
            else if ((objectPosition.X + objectWidth) < 0 || (objectPosition.Y + objectHeight) < 0)
            {
                isOffScreen = true;
            }
            return isOffScreen;
        }

        /// <summary>
        /// Check if something is off the edge of the screen.
        /// Returns true if object is off the screen.
        /// </summary>
        /// <param name="screenHeight">Height of the screen to be used for checking</param>
        /// <param name="screenWidth">Width of the screen to be used for checking</param>
        /// <param name="rec">The rectangle that represents the object to check</param>
        /// <returns>boolean: true is the object is completely off the screen; otherwise returns false</returns>
        public static bool Check(int screenHeight, int screenWidth, Rectangle rec)
        {
            bool isOffScreen = false;

            if (rec.Left > screenWidth || rec.Bottom > screenHeight)
            {
                isOffScreen = true;
            }
            else if (rec.Right < 0 || rec.Top < 0)
            {
                isOffScreen = true;
            }
            return isOffScreen;
        }
    }
}
