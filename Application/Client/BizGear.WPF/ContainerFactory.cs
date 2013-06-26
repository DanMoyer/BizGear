namespace BizGear.WPF
{
	#region usings
	
	using System.Collections.Generic;
	using Microsoft.Practices.Unity;

	using CommonRegistration = Common.Registration.RegistrationModule;
	using ClientRegistration = RegistrationModule;

	using IRegistrationModule = Common.Registration.IRegistrationModule;

	#endregion
	
	public class ContainerFactory
	{
		public static UnityContainer CreateContainer()
		{
			var container = new UnityContainer();

			ClientBootstrapper(container);

			return container;
		}


		private static void ClientBootstrapper(IUnityContainer container)
		{
			IList<object> layersToRegister = new List<object>
				                                 {
					                                 new CommonRegistration(),
													 new ClientRegistration()
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
