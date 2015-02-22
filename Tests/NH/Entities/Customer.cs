using System.Collections.Generic;

namespace SomeBasicDapperApp.Tests.NH.Entities
{
    public class Customer 
    {

        public virtual int Id { get; set; }

        public virtual string Firstname { get; set; }

        public virtual string Lastname { get; set; }

        public virtual IList<Order> Orders { get; set; }

        public virtual int Version { get; set; }

    }
}
