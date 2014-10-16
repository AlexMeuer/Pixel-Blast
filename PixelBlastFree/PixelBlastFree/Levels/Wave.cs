using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PixelBlastFree
{
    //------------------------------------------//
    //  TODO:                                   //
    //      - remove offscreen enemies          //
    //      - check collision                   //
    //      - implement more enemy types        //
    //------------------------------------------//

    class Wave
    {
        List<Enemy> enemyList;  //the enemies to be spawned
        int index;  //the index of the next enemy to spawn
        TimeSpan delay; //the delay between each eney spawn
        TimeSpan timeSinceLastSpawn;    //the amount of time since an enemy was spawned
        bool finished; //whether all enemies are gone and wave is done

        /// <summary>
        /// Creates a new Wave instance.
        /// NOTE: Enemies will not be spawned until Start() is called.
        /// </summary>
        /// <param name="xmlPath">the path to the xml document to load the wave from</param>
        /// <param name="difficultyFactor">the difficulty to scale the enemies by</param>
        /// <param name="delay">how long to wait between spawning enemies</param>
        public Wave(string xmlPath, float difficultyFactor, TimeSpan delay)
        {
            enemyList = LoadFromXML(xmlPath, difficultyFactor);
            this.delay = delay;
            index = 0;
            timeSinceLastSpawn = TimeSpan.Zero;
        }

        /// <summary>
        /// Starts the wave
        /// </summary>
        public void Start()
        {
            timeSinceLastSpawn = TimeSpan.MaxValue;
        }

        /// <summary>
        /// Loads enemies from an xml file
        /// </summary>
        /// <param name="xmlPath">the path to the xml file</param>
        /// <param name="difficultyFactor">the difficulty to scale the enemies by</param>
        /// <returns>a list of enemies</returns>
        private List<Enemy> LoadFromXML(string xmlPath, float difficultyFactor)
        {
            XDocument doc = XDocument.Load(xmlPath);

            var data = from query in doc.Descendants("enemy")
                       select new Enemy
                       (
                            (int)query.Element("x"),
                            (int)query.Element("y"),
                            (int)query.Element("frames"),
                            (string)query.Element("texture"),
                            difficultyFactor
                       );

            return data.ToList<Enemy>();
        }

        /// <summary>
        /// Updates all the enemies in the wave
        /// </summary>
        /// <param name="elapsedTime">time since last update</param>
        public void Update(TimeSpan elapsedTime)
        {
            //only concern ourselves with spawning new enemies if there are eny left to spawn
            if (index < enemyList.Count)
            {
                if (timeSinceLastSpawn >= delay)
                {
                    index++;
                    timeSinceLastSpawn = TimeSpan.Zero;
                }
                else
                {
                    timeSinceLastSpawn += elapsedTime;
                }
            }

            if (enemyList.Count > 0)
            {
                for (int i = 0; i <= index; i++)
                {
                    enemyList[i].Update();
                }
            }
            else
            {
                finished = true;
            }
        }
    }
}
