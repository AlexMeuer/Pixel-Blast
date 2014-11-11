using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace PixelBlastFree
{
    public partial class MainPage : PhoneApplicationPage
    {
        string difficulty = "Medium";
        string playerName = "Player";

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        // Simple button Click event handler to take us to the second page
        private void start_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml?difficulty="+difficulty+"&playerName="+playerName, UriKind.Relative));
        }

        private void report_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void difficulty_Click(object sender, RoutedEventArgs e)
        {
            switch (difficulty)
            {
                case "Easy":
                    difficulty = "Medium";
                    break;
                case "Medium":
                    difficulty = "Hard";
                    break;
                case "Hard":
                    difficulty = "Extreme";
                    break;
                default:
                    difficulty = "Easy";
                    break;
            }

            UpdateDifficultyButton();
        }

        private void UpdateDifficultyButton()
        {
            difficultyButton.Content = "Difficulty: " + difficulty;
        }

        private void name_change(object sender, TextChangedEventArgs e)
        {
            playerName = name_box.Text;
        }

        /// <summary>
        /// Closes the application instead of returning to highscore page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            //remove the ability to go back so that app simply closes
            if (NavigationService.CanGoBack)
            {
                while (NavigationService.RemoveBackEntry() != null)
                {
                    NavigationService.RemoveBackEntry();
                }
            }

            base.OnBackKeyPress(e);
        }
    }
}