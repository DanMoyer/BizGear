namespace SimpleDemoConsole
{
	#region usings

	using System;
	using System.Diagnostics.Tracing;
	using EventSources.EventSources;
	using System.Threading;

	#endregion

	class Program
	{
		static void Main()
		{
			DisplayGuids();

			DisplayEnabled();

			DisplayEventLevels();

			for (var ctr = 0; ctr < 5000; ctr++ )
			{
				if ((ctr % 100) == 0)
					Console.WriteLine(ctr);

				Thread.Sleep(1);
				TestSalesLogging();

				TestBusinessLogging();

				TestMerchandiseLogging();
			}

			Console.WriteLine("Finished");
			Console.ReadLine();
		}

		private static void DisplayGuids()
		{
			var salesGuid             = SalesContextEventSource.Log.Guid.ToString();
			var businessGuid          = InventoryContextEventSource.Log.Guid.ToString();
			var merchandiseReturnGuid = MerchandiseReturnContextEventSource.Log.Guid.ToString();

			Console.WriteLine("Sales Guid\t\t{0}", salesGuid);
			Console.WriteLine("Business Guid\t\t{0}", merchandiseReturnGuid);
			Console.WriteLine("Merchandise Return Guid\t{0}", merchandiseReturnGuid);
		}

		private static  void DisplayEnabled()
		{
			var isSalesEnabled       = SalesContextEventSource.Log.IsEnabled();
			var isBusinessEnabled    = InventoryContextEventSource.Log.IsEnabled();
			var isMerchandiseEnabled = MerchandiseReturnContextEventSource.Log.IsEnabled();

			Console.WriteLine("Sales enabled \t\t{0} ", isSalesEnabled);
			Console.WriteLine("Business enabled \t{0} ",  isBusinessEnabled);
			Console.WriteLine("Merchandise enabled \t{0}", isMerchandiseEnabled);
		}

		private static void DisplayEventLevels()
		{
			var always        = Convert.ToInt32(EventLevel.LogAlways);
			var critical      = Convert.ToInt32(EventLevel.Critical);
			var error         = Convert.ToInt32(EventLevel.Error);
			var warning       = Convert.ToInt32(EventLevel.Warning);
			var informational = Convert.ToInt32(EventLevel.Informational);
			var verbose       = Convert.ToInt32(EventLevel.Verbose);

			Console.WriteLine("always\t\t\t{0}", always);
			Console.WriteLine("critical\t\t{0}", critical);
			Console.WriteLine("error\t\t\t{0}", error);
			Console.WriteLine("warning\t\t\t{0}", warning);
			Console.WriteLine("informationional\t{0}", informational);
			Console.WriteLine("verbose\t\t\t{0}", verbose);
		}

		private static void TestSalesLogging()
		{
			var tstMsg1 = "111111111_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_";
			var tstMsg2 = "123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_";
			var tstMsg3 = "123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_";
			var tstMsg4 = "123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_";
			var tstMsg5 = "123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_";
			var tstMsg6 = "123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_";
			var tstMsg = tstMsg1 + tstMsg2 + tstMsg3 + tstMsg5;

			SalesContextEventSource.Log.Presentation("Sales_Presentation_className", "Sales_presentation_MethodName", tstMsg);
			SalesContextEventSource.Log.Presentation("Sales_Presentation_className", "Sales_presentation_MethodName", "Sales presentation data");
			SalesContextEventSource.Log.Business("Sales_Business_ClassName", "Sales_business_MethodName", "sales business Data");
			SalesContextEventSource.Log.Data("Sales_Data_ClassName", "Sales_data access_MethodName", "sales data access data");
		}

		private static void TestBusinessLogging()
		{
			InventoryContextEventSource.Log.Presentation("Business_Presentation_className", "Business_presentation_MethodName", "Business presentation data");
			InventoryContextEventSource.Log.Business("Business_Business_ClassName", "Business_business_MethodName", "Business business Data");
			InventoryContextEventSource.Log.Data("Business_Data_ClassName", "Business_data access_MethodName", "Business data access data");
		}

		private static void TestMerchandiseLogging()
		{
			MerchandiseReturnContextEventSource.Log.Presentation("MerchandiseReturn_Presentation_className", "MerchandiseReturn_presentation_MethodName", "MerchandiseReturn presentation data");
			MerchandiseReturnContextEventSource.Log.Business("MerchandiseReturn_MerchandiseReturn_ClassName", "MerchandiseReturn_MerchandiseReturn_MethodName", "MerchandiseReturn business Data");
			MerchandiseReturnContextEventSource.Log.Data("MerchandiseReturn_Data_ClassName", "MerchandiseReturn_data access_MethodName", "MerchandiseReturn data access data");
		}
	}
}
