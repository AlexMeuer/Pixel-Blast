using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace PixelBlastFree
{
    class TutorialManager
    {
        /// <summary>
        /// Whether the tutorial had already been run
        /// </summary>
        bool tutorialFinished;

        //TODO: plan rest of variables
        //      write method implementation

        public TutorialManager();

        /// <summary>
        /// Draws tutorial elements to the screen
        /// </summary>
        public void draw(SpriteBatch spriteBatch);

        /// <summary>
        /// Updates the tutorial
        /// </summary>
        /// <param name="elapsedTime">the time passed since last update</param>
        public void update(TimeSpan elapsedTime);

        /// <summary>
        /// Loads tutorialFinished bool from file
        /// </summary>
        /// <param name="path">the path to the file</param>
        /// <returns>True if tutorial has already been completed</returns>
        private bool loadFromConfigFile(string path);

        /// <summary>
        /// saves tutorialFinished bool to file
        /// </summary>
        /// <param name="path">the path to the file</param>
        private void saveToConfigFile(string path);
    }
}
