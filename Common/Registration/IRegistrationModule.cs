namespace Common.Registration
{
	using Microsoft.Practices.Unity;

	public interface IRegistrationModule
	{
		void Register(IUnityContainer container);
	}
}
