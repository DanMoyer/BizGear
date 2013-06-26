using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsTestCircularBuff
{
	using System.Diagnostics.Tracing;
	using System.Threading;

	class Program
	{
		static void Main(string[] args)
		{
			int numOfEvents;
			if (args != null && args.Length > 0)
			{
				numOfEvents = Convert.ToInt32(args[0]);
			}
			else
			{
				numOfEvents = 10;
			}

			var guid = TestCircularBuffer.Log.Guid.ToString();

			var data = GetTestData();
			for (var ctr = 0; ctr < numOfEvents; ctr++)
			{
				if ((ctr % 100) == 0)
					Console.WriteLine(ctr);

				Thread.Sleep(1);

				TestCircularBuffer.Log.Test1(ctr, "test1");
				TestCircularBuffer.Log.Test2(ctr, "test2");
			}

			Console.WriteLine("Finished");
			Console.ReadLine();
		}

		private static string GetTestData()
		{
			var tstMsg1 = "111111111_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_";
			var tstMsg2 = "123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_";
			var tstMsg3 = "123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_";
			var tstMsg4 = "123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_";
			var tstMsg5 = "123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_";
			var tstMsg6 = "123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_";
			var tstMsg = tstMsg1;// +tstMsg2 + tstMsg3 + tstMsg4;

			return tstMsg;
		}
	}

	//  61ca7641-39a3-5ff6-ec8b-18d71774b6d6
	[EventSource(Name = "Optum-Test-CircularBuffer")]
	public class TestCircularBuffer : EventSource
	{
		public static readonly TestCircularBuffer Log = new TestCircularBuffer();

		[Event(1, Level = EventLevel.Informational, Task=Tasks.Class1, Opcode = Opcodes.Method1)]
		//[Event(1, Level = EventLevel.Informational)]
		public void Test1(int counter, string data){if (IsEnabled()) WriteEvent(1, counter, data);}

		[Event(2, Level = EventLevel.Informational, Task = Tasks.Class2, Opcode = Opcodes.Method2)]
		public void Test2(int counter, string data) { if (IsEnabled()) WriteEvent(2, counter, data); }


		public class Keywords
		{
			public const EventKeywords Presentation = (EventKeywords)Layers.Presentation;  //0x0001;
			public const EventKeywords Business = (EventKeywords)Layers.Business;  //0x0002;
			public const EventKeywords DataAccess = (EventKeywords)Layers.DataAccess;   //0x0004;
			public const EventKeywords Service = (EventKeywords)Layers.Service; //0x0008;
		}

		public class Opcodes
		{
			public const EventOpcode Method1 = (EventOpcode)Methods.Method1; //0x10;
			public const EventOpcode Method2 = (EventOpcode)Methods.Method2;
		}

		public class Tasks
		{
			public const EventTask Class1 = (EventTask)Classes.Class1;
			public const EventTask Class2 = (EventTask)Classes.Class2;
		}

	}

	public enum Layers
	{
		Presentation = 0x0001,
		Business = 0x0002,
		DataAccess = 0x0004,
		Service = 0x0008
	}


	public enum Classes
	{
		Class1 = 1,
		Class2 = 2,
		Class3 = 3
	}

	public enum Methods
	{
		Method1 = 0x10,
		Method2 = 0x11,
		Method3 = 0x12
	}
}
