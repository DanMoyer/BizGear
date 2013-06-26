
namespace Services.Registration
{
	#region usings

	using Common.Registration;
	using Interfaces;
	using Microsoft.Practices.Unity;

	#endregion

	public class RegistrationModule : IRegistrationModule
	{
		public void Register(IUnityContainer container)
		{
			container.RegisterType<IMerchandiseReturnService, MerchandiseReturnService.MerchandiseReturnService>();
			container.RegisterType<ISalesService, SalesService.SalesService>();
		}
	}
}
