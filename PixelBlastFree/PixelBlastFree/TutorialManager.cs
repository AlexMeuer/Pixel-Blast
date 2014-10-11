using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PixelBlastFree
{
    class TutorialManager
    {
        /// <summary>
        /// Whether the tutorial had already been run
        /// </summary>
        bool tutorialFinished;

        readonly TimeSpan maxTime;

        Texture2D tapSprite;
        const string moveString = "Tap to move!";

        Color tutColor;

        /// <summary>
        /// Initialises the tutorial manager
        /// </summary>
        public TutorialManager(ContentManager contentManager)
        {
            //tapSprite = contentManager.Load<Texture2D>("tapHere.png");

            tutorialFinished = loadFromConfigFile("config.xml");

            maxTime = TimeSpan.FromSeconds(5);
            tutColor = Color.White;
        }

        /// <summary>
        /// Draws tutorial elements to the screen
        /// </summary>
        public void draw(SpriteBatch spriteBatch, Vector2 screenBounds, SpriteFont font)
        {
            if(!tutorialFinished)
            {
                //TODO: positions need fine tuning (take into account texture dimensions)
                spriteBatch.Draw(tapSprite, new Vector2(screenBounds.X * 0.2f, screenBounds.Y * 0.85f), Color.White);
                spriteBatch.Draw(tapSprite, new Vector2(screenBounds.X * 0.8f, screenBounds.Y * 0.85f), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0);
                spriteBatch.DrawString(font, moveString, new Vector2(screenBounds.X * 0.5f, screenBounds.Y * 0.75f), Color.White);
            }
        }

        /// <summary>
        /// Updates the tutorial
        /// </summary>
        /// <param name="totalElapsedTime">the time passed since game loop started</param>
        public void update(TimeSpan totalElapsedTime)
        {
            Color targetColor;

            if (totalElapsedTime > maxTime)
            {
                tutorialFinished = true;
            }

            //fade everything in or out depending on whether tutorial is finished or not
            if (tutorialFinished)
            {
                targetColor = Color.Transparent;
            }
            else
            {
                targetColor = Color.White;
            }

            tutColor = Color.Lerp(tutColor, Color.Transparent, 0.1f);
        }

        /// <summary>
        /// Loads tutorialFinished bool from file
        /// </summary>
        /// <param name="path">the path to the file</param>
        /// <returns>True if tutorial has already been completed</returns>
        private bool loadFromConfigFile(string path)
        {
            XElement config = XElement.Load(path);

            XAttribute attribute = config.Element("tutorial").Attribute("bFinished");

            return Convert.ToBoolean(attribute.Value);
        }

        /// <summary>
        /// saves tutorialFinished bool to file
        /// </summary>
        /// <param name="path">the path to the file</param>
        private void saveToConfigFile(string path)
        {
            XElement config = XElement.Load(path);

            XAttribute attribute = config.Element("tutorial").Attribute("bFinished");

            attribute.SetValue(Convert.ToString(tutorialFinished));
        }
    }
}
