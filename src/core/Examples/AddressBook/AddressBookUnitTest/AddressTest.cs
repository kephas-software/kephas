using NUnit.Framework;
using System;
using AddressBook;

namespace AddressBookUnitTest
{
	[TestFixture]
	public class AddressTest
	{
		[Test]
		public void AddressToStringReturnsName()
		{
			var address = new Address ("John Doe", "john@example.com");
			Assert.AreEqual ("John Doe", address.ToString ());
		}
	}
}

