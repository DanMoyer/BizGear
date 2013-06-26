namespace Backend
{
	using System;

	using NServiceBus;
	using ServiceBus.Contracts;

	public class EtwMessageHandler : IHandleMessages<EtwMessage>
	{
		public void Handle(EtwMessage message)
		{
			Console.WriteLine("Source: {0} Classname: {1}  MethodName: {2}  Data: {3}",
				message.Source, message.ClassName, message.MethodName, message.Data);
		}
	}
}
