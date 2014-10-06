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
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        // Simple button Click event handler to take us to the second page
        private void start_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }

        private void report_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void difficulty_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}