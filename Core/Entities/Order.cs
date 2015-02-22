using System;
using System.Collections.Generic;

namespace SomeBasicDapperApp.Core.Entities
{
	public class Order 
    {
		public virtual int Id { get; set; }

		public virtual int Customer { get; set; }

		public virtual DateTime OrderDate { get; set; }

		public virtual int Version { get; set; }
		
	}
}
