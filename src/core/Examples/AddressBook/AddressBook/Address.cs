using System;

namespace AddressBook
{
	public class Address : IAddress 
	{
		public string Name { get; set; }
		public string Email { get; set; }

		public Address(string name, string email)
		{
			this.Name = name;
			this.Email = email;
		}

		public override string ToString ()
		{
			return this.Name;
		}
	}
}

