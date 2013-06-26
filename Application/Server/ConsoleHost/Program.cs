namespace ConsoleHost
{
	#region usings

	using Microsoft.Practices.Unity;
	using System;
	using System.Linq;
	using System.ServiceModel;
	using System.ServiceModel.Description;
	using System.Text.RegularExpressions;
	using Unity.Wcf;


	#endregion

	//  http://www.devtrends.co.uk/blog/introducing-unity.wcf-providing-easy-ioc-integration-for-your-wcf-services
	//  Describes how to use Unity.Wcf
	//

	class Program
	{
		private static ServiceHost _salesServiceHost;
		private static ServiceHost _merchandiseServiceHost;

		static void Main(string[] args)
		{
			var container = ContainerFactory.CreateContainer();

			//IServiceBehavior loggingBehavior = new EtwLoggingServiceBehavior(Context.Sales);
			
			//OpenAllServiceHosts(container, loggingBehavior);
			OpenAllServiceHosts(container, null);

			Console.ReadLine();

			CloseAllServiceHosts();
		}


		private static void OpenAllServiceHosts(UnityContainer container, IServiceBehavior behavior)
		{
			_salesServiceHost = OpenServiceHost(typeof(SalesService.SalesService), container, behavior);

			_merchandiseServiceHost = OpenServiceHost(typeof (MerchandiseReturnService.MerchandiseReturnService), container, null);
		}

		private static ServiceHost OpenServiceHost(Type serviceHostType, UnityContainer container, IServiceBehavior behavior)
		{
			try
			{
				//var serviceHost = new ServiceHost(serviceHostType);
				var serviceHost = new UnityServiceHost(container, serviceHostType);

				if (behavior != null)
				{
					serviceHost.Description.Behaviors.Add(behavior);
				}

				serviceHost.Open();

				//var add = serviceHost.BaseAddresses[0];

				Console.WriteLine("{0} is running ",ToWords(serviceHostType.ToString()));
				return serviceHost;
			}

			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return null;
		}


		private static void CloseAllServiceHosts()
		{
			_salesServiceHost.Close();
			_merchandiseServiceHost.Close();
		}

		private static string ToWords(string stringToSplit)
		{
			return
				Regex.Matches(stringToSplit, "[A-Z][a-z]+").OfType<Match>().Select(match => match.Value).Aggregate(
					(acc, b) => acc + " " + b).TrimStart(' ');
		}
	}
}
