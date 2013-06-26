namespace LoggingBehaviors
{
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Dispatcher;

	public class CustomClientMessageInspector : IClientMessageInspector
	{
		private readonly string _clientCookie;

		public CustomClientMessageInspector(string clientCookie)
		{
			_clientCookie = "AAA_" + clientCookie + "_AAA";
		}

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			var dataToSend = new CustomEtwLoggingHeader {EtwCookie = _clientCookie};

			var header = MessageHeader.CreateHeader(CustomEtwLoggingHeader.Name,
													CustomEtwLoggingHeader.NameSpace, 
													dataToSend);
			request.Headers.Add(header);

			return null;
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
		}
	}
}
