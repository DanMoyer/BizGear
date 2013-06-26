namespace DataAccess.Registration
{
	#region usings

	using Common.Registration;
	using MerchandiseReturnDataAccess;
	using Microsoft.Practices.Unity;
	using SalesDataAccess;

	#endregion
	
	public class RegistrationModule : IRegistrationModule
	{
		public void Register(IUnityContainer container)
		{
			container.RegisterType<IMerchandiseReturnRepository, MerchandiseReturnRepository>();
			container.RegisterType<ISalesRepository, SalesRepository>();
		}
	}
}
