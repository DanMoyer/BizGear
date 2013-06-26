namespace LoggingBehaviors
{
	#region usings

	using System.Security.Principal;
	using System.ServiceModel;
	using System.ServiceModel.Dispatcher;
	using System.Threading;
	using EventSources;


	#endregion

	public class EtwLoggingParamaterInspector : IParameterInspector
	{
		private readonly string _service;
		private readonly IEtwLogger _etwLogger;

		public EtwLoggingParamaterInspector(string service, IEtwLogger etwLogger)
		{
			_service = service;
			_etwLogger = etwLogger;
		}

		void IParameterInspector.AfterCall(string operationName, object[] outputs, 
											object returnValue, object correlationState)
		{
			//_etwLogger.Service(_service, operationName, "after call");
		}

		object IParameterInspector.BeforeCall(string operationName, object[] inputs)
		{
			var currentPrincipal = new WindowsPrincipal(ServiceSecurityContext.Current.WindowsIdentity);

			var name = currentPrincipal.Identity.Name;

			var msg = string.Format("BeforeCall Name {0}", name);

			_etwLogger.Service(_service, operationName, msg);

			return null;
		}
	}
}
