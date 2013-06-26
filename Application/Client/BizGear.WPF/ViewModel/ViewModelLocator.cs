/*
  In App.xaml:
  <Application.Resources>
	  <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:BizGear.ViewModel"
								   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/


namespace BizGear.WPF.ViewModel
{
	using BizGear.ViewModel;
	using GalaSoft.MvvmLight;
	using BizGear.Model;
	using Microsoft.Practices.Unity;

	/// <summary>
	/// This class contains static references to all the view models in the
	/// application and provides an entry point for the bindings.
	/// <para>
	/// See http://www.galasoft.ch/mvvm
	/// </para>
	/// </summary>
	public class ViewModelLocator
	{
		public static readonly UnityContainer Container = ContainerFactory.CreateContainer();

		static ViewModelLocator()
		{
			if (ViewModelBase.IsInDesignModeStatic)
			{
				Container.RegisterType<IDataService, Design.DesignDataService>();
			}
			else
			{
				Container.RegisterType<IDataService, DataService>();
			}
		}

		/// <summary>
		/// Gets the Main property.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic",
			Justification = "This non-static member is needed for data binding purposes.")]
		public static MainViewModel Main
		{
			get
			{
				return Container.Resolve<MainViewModel>();
			}
		}

		public static SalesViewModel Sales
		{
			get
			{
				return Container.Resolve<SalesViewModel>();
			}
		}

		public static MerchandiseReturnViewModel MerchandiseReturns
		{
			get
			{
				return Container.Resolve<MerchandiseReturnViewModel>();
			}
		}

		public static AdministrationViewModel Administration
		{
			get
			{
				return Container.Resolve<AdministrationViewModel>();
			}
		}


		/// <summary>
		/// Cleans up all the resources.
		/// </summary>
		public static void Cleanup()
		{
		}
	}
}