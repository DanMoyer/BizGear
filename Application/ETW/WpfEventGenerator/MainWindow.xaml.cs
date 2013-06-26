using System.Windows;
using EventSources.EventSources;

namespace WpfEventGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FirstEvent(object sender, RoutedEventArgs e)
        {
			SalesContextEventSource.Log.Presentation("ServiceClass1", "Method 1", "Some Data");
        }

        private void SecondEvent(object sender, RoutedEventArgs e)
        {
			SalesContextEventSource.Log.Business("ClientClass2", "Method 1", "Some Data");
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
			SalesContextEventSource.Log.Service("DataClass2", "Method 1", "Some Data");
        }
    }
}
