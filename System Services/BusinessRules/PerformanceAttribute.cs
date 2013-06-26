using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.Practices.Unity;

namespace BusinessRules
{
	public class PerformanceAttribute : HandlerAttribute
	{
		public override ICallHandler CreateHandler(IUnityContainer container)
		{
			return new CanReturnMerchandiseRuleHandler();
		}
	}
}
