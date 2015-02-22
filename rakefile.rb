require_relative './.nuget/nuget'

require 'albacore'
require 'win32ole'
require 'fileutils'
require 'rexml/document'

include FileUtils

task :default => [:all]

desc "Rebuild solution"
build :build do |msb, args|
  msb.prop :configuration, :Debug
  msb.prop :VisualStudioVersion, '12.0' # build fails sometimes because of wrong vs version in env
  msb.target = [:Rebuild]
  msb.sln = "dapper-studies.sln"
end

desc "Install missing NuGet packages."
task :install_packages do
    NuGet::exec("restore dapper-studies.sln -source http://www.nuget.org/api/v2/")
end

desc "test using console"
test_runner :test => [:build] do |runner|
  runner.exe =NuGet::nunit_86_path
  #{}"  /framework=net-4.5 "  
  d = File.dirname(__FILE__)
  files = [File.join(d,"Tests","bin","Debug","Tests.dll")]
  runner.files = files 
end

desc "Run everything!"
task :all => [:build, "migrations:run"]

# Below: migrations, not part of the regular build
namespace :migrations do
  pwd = File.dirname(__FILE__)
  def sqlite
     "#{NuGet::migrate_path} /connection \"Data Source=#{File.join(pwd,".db.sqlite")};Version=3;\" /db sqlite /target DbMigrations.dll"
  end
  
  desc "Run migrations"
  task :run, [:version] do |t,args|
    #to migrate back, you can use "rake migrations:run[1]", where 1 is the desired version
    args.with_defaults(:version => nil)
    runcmd = sqlite
    version = args[:version]
    if version
      runcmd += " --version "+version
    end
    cd File.join("DbMigrations","bin","debug") do
    sh runcmd do |ok, res|
      if ! ok
        raise "failed to run migrations (status = #{res.exitstatus})"
      end
    end
    end
  end

  desc "Dry run migrations"
  task :dryrun do |t,args|
    cd File.join("DbMigrations","bin","debug") do
    sh "#{sqlite} --preview" do |ok, res|
      if ! ok
        raise "failed to run migrations (status = #{res.exitstatus})"
      end
    end
    end
  end
end
