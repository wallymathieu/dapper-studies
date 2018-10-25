using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SomeBasicDapperApp.Tests
{
	public class Migrator
	{
		private static void RunWithServices(string processorId, string connectionString)
		{
			// Initialize the services
			var serviceProvider = new ServiceCollection()
				.AddLogging(lb => lb.AddDebug().AddFluentMigratorConsole())
				.AddFluentMigratorCore()
				.ConfigureRunner(
					builder => builder
						.AddSQLite()
						.WithGlobalConnectionString(connectionString)
						.ScanIn(typeof(DbMigrations.AddTables).Assembly).For.Migrations())
				.Configure<SelectingProcessorAccessorOptions>(
					opt => opt.ProcessorId = processorId)
				.BuildServiceProvider();

			// Instantiate the runner
			var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

			// Run the migrations
			runner.MigrateUp();
		}
    

		private readonly string _db;
		public Migrator(string db)
		{
			_db = db;
		}
		public void Migrate()
		{
			RunWithServices(connectionString: "Data Source=" + _db + ";Version=3;", processorId: "sqlite");
		}
	}
}
