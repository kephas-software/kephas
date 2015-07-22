using System;
using AddressBook;

namespace AddressBookCLI
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var address = new Address("John Doe", "jon@example.com");
			Console.WriteLine("Hello " + address);
		}
	}
}
