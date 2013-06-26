namespace LoggingBehaviors
{
	#region usings

	using System.ServiceModel.Channels;
	using System.ServiceModel.Description;
	using System.ServiceModel.Dispatcher;
	using EventSources;

	#endregion

	public class EtwLoggingEnpointBehavior : IEndpointBehavior
	{
		private readonly IEtwLogger _etwLogger;
		private readonly string _clientCookie;

		public EtwLoggingEnpointBehavior(IEtwLogger etwLogger)
		{
			_etwLogger = etwLogger;
			_clientCookie = string.Empty;
		}

		public EtwLoggingEnpointBehavior(IEtwLogger etwLogger, string clientCookie)
		{
			_etwLogger = etwLogger;
			_clientCookie = clientCookie;
		}
		
		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			var customMessage = new CustomClientMessageInspector(_clientCookie);
			clientRuntime.MessageInspectors.Add(customMessage);
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			var inspector = new EtwLoggingMessageInspector(_etwLogger);
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}
	}
}
