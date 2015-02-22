using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using SomeBasicDapperApp.Core;
using SomeBasicDapperApp.Tests.NH.Core.Entities;
using System;

namespace SomeBasicDapperApp.Tests.NH
{
	public class HibernateSession
	{
		private class CustomConf : DefaultAutomappingConfiguration
		{
			public override bool IsId(Member member)
			{
				return member.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase);
			}
			public override Type GetParentSideForManyToMany(Type left, Type right)
			{
				return base.GetParentSideForManyToMany(left, right);
			}
		}
		public class TableNameConvention : IClassConvention
		{
			public void Apply(FluentNHibernate.Conventions.Instances.IClassInstance instance)
			{
				string typeName = instance.EntityType.Name;

				instance.Table(typeName + "s");
			}
		}
		private readonly IMapPath _mapPath;

		public HibernateSession(IMapPath mapPath)
		{
			_mapPath = mapPath;
		}

		private FluentConfiguration ConfigureMaps(FluentConfiguration conf)
		{
			return conf.Mappings(m => m.AutoMappings.Add(new AutoPersistenceModel()
				.AddEntityAssembly(typeof(Customer).Assembly)
				.Conventions.Add(new TableNameConvention())
				.Where(t => t.Namespace.EndsWith("Core.Entities")))
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
