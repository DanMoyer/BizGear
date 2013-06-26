namespace EventSources.EventSources
{
	using System.Diagnostics.Tracing;

	//guid for this provider:  {9a9cf874-7496-5df5-6e80-1c5804eccd57}
	[EventSource(Name = EventSourceNames.BusinessContext)]
	public class BusinessContextEventSource : EventSource
	{
		public static BusinessContextEventSource Log = new BusinessContextEventSource();

		[Event(1, Level = EventLevel.Informational, Keywords = Keywords.Presentation)]
		public void Presentation(string className, string methodName, string data) { if (IsEnabled()) WriteEvent(1, className, methodName, data); }

		[Event(2, Level = EventLevel.Informational, Keywords = Keywords.Business)]
		public void Business(string className, string methodName, string data) { if (IsEnabled()) WriteEvent(2, className, methodName, data); }

		[Event(3, Level = EventLevel.Informational, Keywords = Keywords.DataAccess)]
		public void Data(string className, string methodName, string data) { if (IsEnabled()) WriteEvent(3, className, methodName, data); }

		[Event(4, Level = EventLevel.Informational, Keywords = Keywords.Service)]
		public void Service(string className, string methodName, string data) { if (IsEnabled()) WriteEvent(4, className, methodName, data); }

		public class Keywords
		{
			public const EventKeywords Presentation = (EventKeywords)0x0001;
			public const EventKeywords Business = (EventKeywords)0x0002;
			public const EventKeywords DataAccess = (EventKeywords)0x0004;
			public const EventKeywords Service = (EventKeywords)0x0008;
		}
	}
}
