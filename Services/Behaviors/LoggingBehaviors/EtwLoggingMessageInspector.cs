namespace LoggingBehaviors
{
	#region usings

	using System;
	using System.Globalization;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Dispatcher;
	using System.Xml;
	using EventSources;

	#endregion

	public class EtwLoggingMessageInspector : IDispatchMessageInspector
	{
		private readonly IEtwLogger _etwLogger;

		public EtwLoggingMessageInspector(IEtwLogger etwLogger)
		{
			_etwLogger = etwLogger;
		}

		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			if (!_etwLogger.IsEnabled()) return null;

			var etwCookie = string.Empty;

			var headerIndex = request.Headers.FindHeader(CustomEtwLoggingHeader.Name,
			                                             CustomEtwLoggingHeader.NameSpace, "");
			if (headerIndex > -1)
			{
				var content = request.Headers.GetHeader<XmlNode[]>(headerIndex);
				etwCookie = content[0].InnerText;
			}
			var msg = request.ToString();

			if (msg == null) return null;

			var totalMemory = GC.GetTotalMemory(false);

			var etwMsg = string.Format("EtwCookie: {0}  MsgLen: {1} TotalMemory {2}", etwCookie, msg.Length, totalMemory);

			_etwLogger.Service("EtwLoggingMessageInspector", "AfterReceiveRequest", etwMsg);

			return null;
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			if (!_etwLogger.IsEnabled()) return;

			var msg = reply.ToString();

			if (msg == null) return;

			var msgLen = msg.Length;

			_etwLogger.Service("EtwLoggingMessageInspector", "BeforeSendReply", msgLen.ToString(CultureInfo.InvariantCulture));
		}
	}
}
