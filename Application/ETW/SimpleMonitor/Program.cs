using Diagnostics.Tracing;
using Diagnostics.Tracing.Parsers;
using System;
using System.Diagnostics;
using EventSources;
using EventSources.EventSources;

// This program is a simple demo, it has both a EventSource and a EventGenerator which generate events for 10 seconds
// as well as a 'listener; that listens for these events (potentially in a differnet process, but we don't do that here)
// and does some rudimentary processing (calculates the difference between two particular events.    
namespace SimpleMonitor
{
	/// <summary>
	/// The main program is the 'listener' that listens and processes the events that come from EventGenerator
	/// </summary>
	class Program
	{
		private static ETWTraceEventSource _source;
		private static Guid _providerGuid;

		/// <summary>
		/// This is a demo of using TraceEvent to activate a 'real time' provider that is listening to 
		/// the MyEventSource above.   Normally this event source would be in a differnet process,  but 
		/// it also works if this process generate the evnets and I do that here for simplicity.  
		/// </summary>
		static int Main(string[] args)
		{
			// This is the name of the event source.   
			const string providerName = EventSourceNames.SalesContext;  // "Acme-BizGear-SalesContext";

			//Debug.Assert(providerName == MyEventSource.Log.Name);
			// Given just the name of the eventSource you can get the GUID for the evenSource by calling this API.  
			// From a ETW perspective, the GUID is the 'true name' of the EventSource.  
			_providerGuid = TraceEventSession.GetEventSourceGuidFromName(providerName);
			Debug.Assert(_providerGuid == SalesContextEventSource.Log.Guid);

			// Today you have to be Admin to turn on ETW events (anyone can write ETW events).   
			if (!(TraceEventSession.IsElevated() ?? false))
			{
				Console.WriteLine("To turn on ETW events you need to be Administrator, please run from an Admin process.");
				return -1;
			}

			// As mentioned below, sessions can outlive the process that created them.  Thus you need a way of 
			// naming the session so that you can 'reconnect' to it from another process.   This is what the name
			// is for.  It can be anything, but it should be descriptive and unique.   If you expect mulitple versions
			// of your program to run simultaneously, you need to generate unique names (e.g. add a process ID suffix) 
			Console.WriteLine("Creating a 'My Session' session");
			const string sessionName = "My Session";    

			using (var session = new TraceEventSession(sessionName, null))  // the null second parameter means 'real time session'
			{
				// Note that sessions create a OS object (a session) that lives beyond the lifetime of the process
				// that created it (like Filles), thus you have to be more careful about always cleaning them up. 
				// An importanty way you can do this is to set the 'StopOnDispose' property which will cause the session to 
				// stop (and thus the OS object will die) when the TraceEventSession dies.   Because we used a 'using'
				// statement, this means that any exception in the code below will clean up the OS object.   
				session.StopOnDispose = true;

				// By default, if you hit Ctrl-C your .NET objects may not be disposed, so force it to.  It is OK if dispose is called twice.
				Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e) { session.Dispose(); };

				// prepare to read from the session, connect the ETWTraceEventSource to the session
				using (var source = new ETWTraceEventSource(sessionName, TraceEventSourceType.Session))
				{
					_source = source;
					// To demonstrate non-trivial event manipuation, we calculate the time delta between 'MyFirstEvent and 'MySecondEvent'
					// These variables are used in this calculation 
					int lastMyEventID = int.MinValue;       // an illegal value to start with.  
					double lastMyEventMSec = 0;

					// Hook up the parser that knows about EventSources
					var parser = new DynamicTraceEventParser(source);
					//parser.All += delegate(TraceEvent data)

					parser.All += OnMySessionEvent;

					//parser.All +=  (data) =>
					//{
					//	Console.WriteLine("GOT EVENT: " + data);

					//	if (data.ProviderGuid == providerGuid)  // We don't actually need this since we only turned one one provider. 
					//	{
					//		// Note that this is the inefficient way of parsing events (there are string comparisions on the 
					//		// event Name and every payload value), however it is fine for events that occur less than 100 times
					//		// a second.   For more volumous events, you should consider making a parser for you eventSource 
					//		// (covered in another demo).  This makes your code fare less 'reflection-like' where you have lots
					//		// of strings (e.g. "MyFirstEvent", "MyId" ...) which is better even ignoring the performance benefit.  
					//		if (data.EventName == "MyFirstEvent")
					//		{
					//			// On First Events, simply remember the ID and time of the event
					//			lastMyEventID = (int)data.PayloadByName("myId");
					//			lastMyEventMSec = data.TimeStampRelativeMSec;
					//		}
					//		else if (data.EventName == "MySecondEvent")
					//		{
					//			// On Second Events, if the ID matches, compute the delta and display it. 
					//			if (lastMyEventID == (int)data.PayloadByName("myId"))
					//				Console.WriteLine("   > Time Delta from first Event = {0:f3} MSec", data.TimeStampRelativeMSec - lastMyEventMSec);
					//		}
					//		else if (data.EventName == "Stop")
					//		{
					//			// Stop processing after we we see the 'Stop' event
					//			source.Close();
					//		}
					//	}
					//};

					// Enable my provider, you can call many of these on the same session to get other events.  
					session.EnableProvider(_providerGuid);

					// Start another thread that Causes MyEventSource to create some events
					// Normally this code as well as the EventSource itself would be in a different process.  
					//EventGenerator.CreateEvents();

					Console.WriteLine("Staring Listing for events");
					// go into a loop processing events can calling the callbacks.  Because this is live data (not from a file)
					// processing never completes by itself, but only because someone called 'source.Close()'.  
					source.Process();
					Console.WriteLine();


					Console.WriteLine("Stopping the collection of events.");
				}
			}

			return 0;
		}

		public static void OnMySessionEvent(TraceEvent data)
		{
			// To demonstrate non-trivial event manipuation, we calculate the time delta between 'MyFirstEvent and 'MySecondEvent'
			// These variables are used in this calculation 
			int lastMyEventID = int.MinValue;       // an illegal value to start with.  
			double lastMyEventMSec = 0;

			Console.WriteLine("GOT EVENT: " + data);

			if (data.ProviderGuid == _providerGuid)  // We don't actually need this since we only turned one one provider. 
			{
				// Note that this is the inefficient way of parsing events (there are string comparisions on the 
				// event Name and every payload value), however it is fine for events that occur less than 100 times
				// a second.   For more volumous events, you should consider making a parser for you eventSource 
				// (covered in another demo).  This makes your code fare less 'reflection-like' where you have lots
				// of strings (e.g. "MyFirstEvent", "MyId" ...) which is better even ignoring the performance benefit.  
				if (data.EventName == "MyFirstEvent")
				{
					// On First Events, simply remember the ID and time of the event
					lastMyEventID = (int)data.PayloadByName("myId");
					lastMyEventMSec = data.TimeStampRelativeMSec;
				}
				else if (data.EventName == "MySecondEvent")
				{
					// On Second Events, if the ID matches, compute the delta and display it. 
					if (lastMyEventID == (int)data.PayloadByName("myId"))
						Console.WriteLine("   > Time Delta from first Event = {0:f3} MSec", data.TimeStampRelativeMSec - lastMyEventMSec);
				}
				else if (data.EventName == "Stop")
				{
					// Stop processing after we we see the 'Stop' event
					_source.Close();
				}
			}			
		}
	}
}
