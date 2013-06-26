
using Microsoft.Practices.Unity.InterceptionExtension;

namespace SystemServices.Registration
{
	#region usings

	using BusinessRules;
	using Common.Registration;
	using Microsoft.Practices.Unity;

	#endregion

	public class RegistrationModule : IRegistrationModule
	{
		public void Register(IUnityContainer container)
		{
			container.AddNewExtension<Interception>();

			container.RegisterType<ICanReturnMerchandiseRule, CanReturnMerchandiseRule>()
			.Configure<Interception>()
			.SetInterceptorFor<ICanReturnMerchandiseRule>(new InterfaceInterceptor());
		}
	}
}
