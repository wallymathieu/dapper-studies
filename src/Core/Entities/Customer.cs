using System.Collections.Generic;

namespace SomeBasicDapperApp.Core.Entities
{
	public class Customer 
    {
		public virtual int Id { get; set; }
		public virtual string Firstname { get; set; }

		public virtual string Lastname { get; set; }
		
		public virtual int Version { get; set; }
	}
}
