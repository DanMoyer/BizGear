namespace ConsoleHost
{
	#region usings 

	using Microsoft.Practices.Unity;
	using System.Collections.Generic;

	using CommonRegistration     = Common.Registration.RegistrationModule;
	using DataAccessRegistration = DataAccess.Registration.RegistrationModule;
	using SystemRegistration     = SystemServices.Registration.RegistrationModule;
	using SystemServices         = Services.Registration.RegistrationModule;

	using IRegistrationModule    = Common.Registration.IRegistrationModule;

	#endregion
	
	public class ContainerFactory
	{
		public static UnityContainer CreateContainer()
		{
			var container = new UnityContainer();

			ServiceBootstrapper(container);

			return container;
		}


		private static void ServiceBootstrapper(IUnityContainer container)
		{
			IList<object> layersToRegister = new List<object>
				                                 {
					                                 new CommonRegistration(),
													 new DataAccessRegistration(),
													 new SystemRegistration(),
													 new SystemServices()
				                                 };

			foreach (var layer in layersToRegister)
			{
				var registrationModule = layer as IRegistrationModule;
				if (registrationModule != null)
				{
					registrationModule.Register(container);
				}
			}
		}
	}
}
