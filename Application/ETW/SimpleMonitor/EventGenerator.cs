using System;
using System.Threading;
using System.Threading.Tasks;
using EventSources;
using EventSources.EventSources;

namespace SimpleMonitor
{
	// This code belongs in the process generating the events.   It is in this process for simplicity.  
	class EventGenerator
	{
		public static void CreateEvents()
		{
			// This just spanws a thread that generates events every second for 10 seconds than issues a Stop event.  
			Task.Factory.StartNew(delegate
				{
					for (int i = 0; i < 10; i++)
					{
						Console.WriteLine("** Generateing the MyFirst and MySecond events.");
						SalesContextEventSource.Log.Presentation("ClassName1", "MethodName1", "Data1");
						Thread.Sleep(10);
						SalesContextEventSource.Log.Presentation("ClassName2", "MethodName2", "Data2");
						Thread.Sleep(1000);
					}
					Console.WriteLine("** Generating the Stop Event.");
					//SalesContextEventSource.Log.Stop();
				});
		}
	}
}