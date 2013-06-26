namespace BizGear.WPF
{
	#region usings

	using System;
	using Common.Registration;
	using BizGear.ViewModel;
	using ViewModel;
	using Microsoft.Practices.Unity;

	#endregion

	public class RegistrationModule : IRegistrationModule
	{
		public void Register(IUnityContainer container)
		{
			Func<LifetimeManager> makeRepositoryLifetimeManager = () => new ContainerControlledLifetimeManager();

			container.RegisterType<MainViewModel>();
			container.RegisterType<MerchandiseReturnViewModel>();
			container.RegisterType<SalesViewModel>();
			container.RegisterType<AdministrationViewModel>();
		}
	}
}