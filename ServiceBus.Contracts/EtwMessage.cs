namespace ServiceBus.Contracts
{
	using System;
	using NServiceBus;

	public class EtwMessage : IMessage
	{
		public string Source { get; set; }
		public string ClassName { get; set; }
		public string MethodName { get; set; }
		public string Data { get; set; }
	}
}
