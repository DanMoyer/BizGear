namespace BizGear2
{
	using System.Windows;
	using GalaSoft.MvvmLight.Threading;
	using NServiceBus;

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static IBus Bus;

		static App()
		{
			DispatcherHelper.Initialize();
		}


		private void App_OnStartup(object sender, StartupEventArgs e)
		{
			Bus = Configure.With()
							.Log4Net()
							.DefaultBuilder()
							.XmlSerializer()
							.MsmqTransport()
							.UnicastBus()
							.SendOnly();
		}
	}
}
