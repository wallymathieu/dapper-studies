using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using SomeBasicDapperApp.Tests.NH.Entities;
using System;

namespace SomeBasicDapperApp.Tests.NH
{
	public class HibernateSession
	{
		public class TableNameConvention : IClassConvention
		{
			public void Apply(FluentNHibernate.Conventions.Instances.IClassInstance instance)
			{
				string typeName = instance.EntityType.Name;

				instance.Table(typeName + "s");
			}
		}

		private FluentConfiguration ConfigureMaps(FluentConfiguration conf)
		{
			return conf.Mappings(m => m.AutoMappings.Add(
				new AutoPersistenceModel()
				.AddEntityAssembly(typeof(Customer).Assembly)
				.Conventions.Add(new TableNameConvention())
				.Where(t => t.Namespace?.EndsWith("SomeBasicDapperApp.Tests.NH.Entities")??false))
				);
		}
		public ISessionFactory CreateTestSessionFactory(string file, bool newDb = false)
		{
			return ConfigureMaps(Fluently.Configure()
			  .Database(
				SQLiteConfiguration.Standard.UsingFile(file))//NOTE:why not use in memory? some queries wont work for nhibernate
			  ).ExposeConfiguration(cfg =>
			  {
				  if (newDb)
				  {
					  new SchemaExport(cfg).Execute(true, true, false);
				  }
			  })
			  .BuildSessionFactory();
		}
	}

}
