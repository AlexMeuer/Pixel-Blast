using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PixelBlastFree
{
    struct highScore
    {
        public string name;
        public int score;

        public highScore(string _name, int _score)
        {
            name = _name;
            score = _score;
        }
    }

    public partial class ScorePage : PhoneApplicationPage
    {
        const int maxNumScores = 10;
        List<highScore> scoreList;

        public ScorePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadScoresFromXML("scores.xml");    //do this before querying the current player's name and score (this method overwrites scoreList)

            string name, score;

            //query player's name and score
            NavigationContext.QueryString.TryGetValue("name", out name);
            NavigationContext.QueryString.TryGetValue("score", out score);

            scoreList.Add(new highScore(name, Convert.ToInt32(score)));

        }

        private void LoadScoresFromXML(string name)
        {
            //try loading from storage...
            XDocument doc = LoadFromIsolatedStorage(name);

            //...if not in storage, load from resource
            if (doc == null)
            {
                doc = XDocument.Load(name);
            }

            var data = from query in doc.Descendants("highscore")
                       select new highScore
                       {
                           name = (string)query.Element("name"),
                           score = (int)query.Element("score"),
                       };
            scoreList = data.ToList<highScore>();
        }

        public static XDocument LoadFromIsolatedStorage(string name)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(name))
                {
                    using (var stream = store.OpenFile(name, FileMode.Open))
                        return XDocument.Load(stream);
                }
                else
                    return null;
            }
        }

        public static void SaveToIsolatedStorage(XDocument doc, string name)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var dir = System.IO.Path.GetDirectoryName(name);

                if (!store.DirectoryExists(dir))
                    store.CreateDirectory(dir);

                using (var file = store.OpenFile(name, FileMode.OpenOrCreate))
                    doc.Save(file);
            }
        }
    }
}