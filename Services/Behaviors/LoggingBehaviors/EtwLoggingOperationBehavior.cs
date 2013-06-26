namespace LoggingBehaviors
{
	#region usings

	using System;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Description;
	using System.ServiceModel.Dispatcher;
	using EventSources;
	using EventSources.Adapters;
	using EventSources.ApplicationEventSource;
	
	#endregion

	[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
	public class EtwLoggingOperationBehavior : Attribute, IOperationBehavior
	{
		private readonly IEtwLogger _etwLogger;

		public EtwLoggingOperationBehavior(Context context)
		{
			_etwLogger = EtwLoggingFactory.CreateLogger(context);
		}
		
		public void ApplyDispatchBehavior(OperationDescription operationDescription, 
										  DispatchOperation dispatchOperation)
		{
			var serviceName = dispatchOperation.Parent.Type.Name;
			var inspector = new EtwLoggingParamaterInspector(serviceName, _etwLogger);

			dispatchOperation.ParameterInspectors.Add(inspector);
		}

		public void Validate(OperationDescription operationDescription)
		{
		}

		public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
		{
		}

		public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
		{
		}

	}
}
