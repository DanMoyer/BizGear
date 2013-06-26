using System;
using System.ComponentModel;
using System.Windows;
using System.Threading.Tasks;
using Diagnostics.Tracing;
using Diagnostics.Tracing.Parsers;
using EventSources;

namespace WpfEventMonitor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		#region declarations
		
		private static Guid _providerGuid;
		private string _providerName;
		private string _sessionName;

		private TraceEventSession _session;
		private static ETWTraceEventSource _source;
		private static DynamicTraceEventParser _parser;

		#endregion

		#region properties

		private int LastMyEventId { get; set; }

		private double LastMyEventMSec { get; set; }

		#endregion

		public MainWindow()
		{
			InitializeComponent();

			InitializeEtwMonitor();
		}

		private void InitializeEtwMonitor()
		{
			_providerName = EventSourceNames.SalesContext; // "Acme-BizGear-SalesContext";

			// Given just the name of the eventSource you can get the GUID for the evenSource by calling this API.  
			// From a ETW perspective, the GUID is the 'true name' of the EventSource.  
			_providerGuid = TraceEventSession.GetEventSourceGuidFromName(_providerName);

			var myguid = _providerGuid.ToString();

			// Today you have to be Admin to turn on ETW events (anyone can write ETW events).   
			if (!(TraceEventSession.IsElevated() ?? false))
			{
				MessageBox.Show("To turn on ETW events you need to be Administrator, please run from an Admin process.");
				return;
			}

			// As mentioned below, sessions can outlive the process that created them.  Thus you need a way of 
			// naming the session so that you can 'reconnect' to it from another process.   This is what the name
			// is for.  It can be anything, but it should be descriptive and unique.   If you expect mulitple versions
			// of your program to run simultaneously, you need to generate unique names (e.g. add a process ID suffix) 
			_sessionName = "My Session";

			//emit status message

			// To demonstrate non-trivial event manipuation, we calculate the time delta between 'MyFirstEvent and 'MySecondEvent'
			// These variables are used in this calculation 
			LastMyEventId = int.MinValue; // an illegal value to start with.  
			LastMyEventMSec = 0;

			Stop.IsEnabled = false;
		}
		
		private void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			if (_session != null)
			{
				_session.Dispose();
			}

			if (_source != null)
			{
				_source.Dispose();
			}
		}

		private void OnStart(object sender, RoutedEventArgs e)
		{
			_session = new TraceEventSession(_sessionName, null) {StopOnDispose = true};

			// Note that sessions create a OS object (a session) that lives beyond the lifetime of the process
			// that created it (like Filles), thus you have to be more careful about always cleaning them up. 
			// An importanty way you can do this is to set the 'StopOnDispose' property which will cause the session to 
			// stop (and thus the OS object will die) when the TraceEventSession dies.   Because we used a 'using'
			// statement, this means that any exception in the code below will clean up the OS object.   

			_source = new ETWTraceEventSource(_sessionName, TraceEventSourceType.Session);

			// Hook up the parser that knows about EventSources
			_parser      = new DynamicTraceEventParser(_source);
			_parser.All += OnMySessionEvent;

			// Enable my provider, you can call many of these on the same session to get other events.  
			_session.EnableProvider(_providerGuid);

			//Starting Listing for events
			// go into a loop processing events can calling the callbacks.  Because this is live data (not from a file)
			// processing never completes by itself, but only because someone called 'source.Close()'.  

			Task.Factory.StartNew(() =>
									  {
										  _source.Process();
									  });

			Start.IsEnabled = false;
			Stop.IsEnabled  = true;
		}
		
		private void OnMySessionEvent(TraceEvent data)
		{
			// To demonstrate non-trivial event manipuation, we calculate the time delta between 'MyFirstEvent and 'MySecondEvent'
			// These variables are used in this calculation 
			var lastMyEventID = int.MinValue;       // an illegal value to start with.  
			double lastMyEventMSec = 0;

			//Console.WriteLine("GOT EVENT: " + data);

			if (data.ProviderGuid == _providerGuid)  // We don't actually need this since we only turned one one provider. 
			{
				// Note that this is the inefficient way of parsing events (there are string comparisions on the 
				// event Name and every payload value), however it is fine for events that occur less than 100 times
				// a second.   For more volumous events, you should consider making a parser for you eventSource 
				// (covered in another demo).  This makes your code fare less 'reflection-like' where you have lots
				// of strings (e.g. "MyFirstEvent", "MyId" ...) which is better even ignoring the performance benefit.  
				
				var evd = data.ToString();

				var payloadData0 = data.PayloadString(0);
				var payloadData1 = data.PayloadString(1);
				var payloadData2 = data.PayloadString(2);

				var eventName = data.EventName;
				var payloadName0 = data.PayloadNames[0];
				var payloadName1 = data.PayloadNames[1];
				var payloadName2 = data.PayloadNames[2];

				var timestamp1 = data.TimeStamp100ns;
				var timestamp2 = data.TimeStamp;
				var timestamp3 = data.TimeStampRelativeMSec;
				var processId = data.ProcessID;
				var threadId = data.ThreadID;

				var msg = string.Format("EventName: {0} {1}: {2}  {3}: {4}  {5}: {6}  Time: {7} TimeMsec {8}",
										eventName, 
										payloadName0, payloadData0, 
										payloadName1, payloadData1,
										payloadName2, payloadData2, 
										timestamp2, timestamp3);


				EventsList.Dispatcher.BeginInvoke(new Action(() => EventsList.Items.Add(msg)));
				
				
				//if (data.EventName == "MyFirstEvent")
				//{
				//	EventsList.Dispatcher.BeginInvoke(new Action(() => EventsList.Items.Add("MyFirstEvent")));

				//	// On First Events, simply remember the ID and time of the event
				//	lastMyEventID = (int)data.PayloadByName("myId");
				//	lastMyEventMSec = data.TimeStampRelativeMSec;
				//}
				//else if (data.EventName == "MySecondEvent")
				//{
				//	EventsList.Dispatcher.BeginInvoke(new Action(() => EventsList.Items.Add("MySecondEvent")));


				//	// On Second Events, if the ID matches, compute the delta and display it. 
				//	//if (lastMyEventID == (int)data.PayloadByName("myId"))
				//		//Console.WriteLine("   > Time Delta from first Event = {0:f3} MSec", data.TimeStampRelativeMSec - lastMyEventMSec);
				//}
				//else if (data.EventName == "Stop")
				//{
				//    //// Stop processing after we we see the 'Stop' event
				//    //_source.Close();
				//    Start.IsEnabled = true; 
				//    Stop.IsEnabled = false;
				//}
			}            
		}

		private void OnStop(object sender, RoutedEventArgs e)
		{
			if (_source != null)
			{
				_source.Close();
				//_parser.All -= OnMySessionEvent;  will throw Not Supported
				_parser = null;
			}

			Start.IsEnabled = true;
			Stop.IsEnabled = false;
		}

		private void OnClosing(object sender, CancelEventArgs e)
		{
			if (_source != null)
			{
				_source.Close();
			}
		}

		private void OnClear(object sender, RoutedEventArgs e)
		{
			EventsList.Items.Clear();
		}
	}
}
