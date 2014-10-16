using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PixelBlastFree
{
    class Level
    {
        List<Wave> waveList;
        int id;

        public Level(string xmlPath, float difficultyFactor)
        {
            waveList = LoadFromXML(xmlPath, difficultyFactor);
        }

        /// <summary>
        /// Loads waves from an xml file
        /// </summary>
        /// <param name="xmlPath">the path to the xml file</param>
        /// <param name="difficultyFactor">the difficulty to scale the enemies by</param>
        /// <returns>a list of waves</returns>
        private List<Wave> LoadFromXML(string xmlPath, float difficultyFactor)
        {
            XDocument doc = XDocument.Load(xmlPath);

            var data = from query in doc.Descendants("wave")
                       select new Wave
                       (
                            (string)query.Element("xmlPath"),
                            difficultyFactor,
                            TimeSpan.FromSeconds((double)query.Element("delay"))
                       );

            return data.ToList<Wave>();
        }
    }
}
