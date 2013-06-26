
namespace Common.Registration
{
	#region usings

	using EventSources;
	using EventSources.Adapters;
	using EventSources.ApplicationEventSource;
	using Microsoft.Practices.Unity;
	using System;

	#endregion

	public class RegistrationModule : IRegistrationModule
	{
		public void Register(IUnityContainer container)
		{
			Func<LifetimeManager> makeRepositoryLifetimeManager = () => new ContainerControlledLifetimeManager();

			container.RegisterType<IEtwLogger, BusinessContextEventSourceAdapter>("Business");
			container.RegisterType<IEtwLogger, SalesContextEventSourceAdapter>("Sales");
			container.RegisterType<IEtwLogger, MerchandiseReturnContextEventSourceAdapter>("MerchandiseReturn");
			container.RegisterType<ILogStrategy, ApplicationLog>();
			container.RegisterType<ILogStrategy, SalesLogStrategy>("SalesEnvelope");
		}
	}
}