namespace LoggingBehaviors
{
	#region usings

	using System;
	using System.Linq;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Description;
	using System.Collections.ObjectModel;
	using EventSources.ApplicationEventSource;
	using EventSources;

	#endregion

	[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
	public class EtwLoggingServiceBehavior : Attribute, IServiceBehavior
	{
		private readonly Context _context;
		private readonly IEtwLogger _etwLogger;

		public EtwLoggingServiceBehavior(Context context)
		{
			_context = context;

			_etwLogger = EtwLoggingFactory.CreateLogger(context);
		}
		
		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (var endpoint in serviceDescription.Endpoints)
			{
				endpoint.Behaviors.Add(new EtwLoggingEnpointBehavior(_etwLogger));

				foreach (var operation in endpoint.Contract.Operations)
				{
					var bFound = operation.Behaviors.Any(opBehavior => opBehavior.GetType().Name == "EtwLoggingOperationBehavior");

					if (bFound) continue;

					var behavior = new EtwLoggingOperationBehavior(_context);
					operation.Behaviors.Add(behavior);
				}
			}
		}

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}

		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}
	}
}
