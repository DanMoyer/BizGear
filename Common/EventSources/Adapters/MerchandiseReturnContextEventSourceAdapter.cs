﻿using EventSources.EventSources;

namespace EventSources.Adapters
{
	public class MerchandiseReturnContextEventSourceAdapter : IEtwLogger
	{
		private readonly MerchandiseReturnContextEventSource _eventSource;

		public MerchandiseReturnContextEventSourceAdapter()
		{
			_eventSource = MerchandiseReturnContextEventSource.Log;
		}

		public bool IsEnabled()
		{
			return _eventSource.IsEnabled();
		}

		public void Presentation(string className, string methodName, string data)
		{
			_eventSource.Presentation(className, methodName, data);
		}

		public void Business(string className, string methodName, string data)
		{
			_eventSource.Business(className, methodName, data);
		}

		public void Service(string className, string methodName, string data)
		{
			_eventSource.Service(className, methodName, data);
		}

		public void Data(string className, string methodName, string data)
		{
			_eventSource.Data(className, methodName, data);
		}
	}
}
