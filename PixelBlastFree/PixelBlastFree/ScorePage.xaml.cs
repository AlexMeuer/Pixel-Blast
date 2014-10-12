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
    /// <summary>
    /// Stores a name and a score.
    /// </summary>
    struct highScore
    {
        public string name;
        public int score;

        public highScore(string _name, int _score)
        {
            name = _name;
            score = _score;
        }

        public override string ToString()
        {
            //return base.ToString();
            return String.Format("{0}:\t{1}", name, score);
        }
    }

    /// <summary>
    /// Displays the game's highscores & saves/loads them to/from XML.
    /// </summary>
    public partial class ScorePage : PhoneApplicationPage
    {
        const int maxNumScores = 10;
        List<highScore> scoreList;
        bool scoresLoaded = false;

        public ScorePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the highscores from XML (if it hasn't done so already) and adds the player's score.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!scoresLoaded)  //without this, player's score adds itself every time we get here
            {
                LoadScoresFromXML("scores.xml");    //do this before querying the current player's name and score (this method overwrites scoreList)

                string name, score;

                //query player's name and score
                NavigationContext.QueryString.TryGetValue("name", out name);
                NavigationContext.QueryString.TryGetValue("score", out score);

                scoreList.Add(new highScore(name, Convert.ToInt32(score)));

                if (scoreList.Count > maxNumScores)
                {
                    scoreList.RemoveRange(maxNumScores, scoreList.Count - maxNumScores);    //needs testing
                }

                scoresLoaded = true;
            }

            scoreBox.ItemsSource = scoreList;
        }

        /// <summary>
        /// Saves the highscores to XML.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SaveScoresToXML("scores.xml");
            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Navigates back to main menu instead of back to game page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            base.OnBackKeyPress(e);
        }

        #region Saving & Loading
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

        private void SaveScoresToXML(string name)
        {
            var ele = new XElement("scores",
                    from highscore in scoreList
                    select new XElement("highscore",
                            new XElement("name", highscore.name),
                            new XElement("score", highscore.score)
                            ));

            SaveToIsolatedStorage(ele, name);
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

        public static void SaveToIsolatedStorage(XElement doc, string name)
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
        #endregion
    }
}