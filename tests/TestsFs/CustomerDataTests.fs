namespace CoreFsTests

open Xunit
open System.IO
open System
open System.Collections.Generic

open SomeBasicFileStoreApp
open GetCommands
open FSharp.Control.Tasks 
open Npgsql
open FsMigrations

type CustomerDataTests()=
    let connString= "Username=postgres;Database=dapperstudies;Password=b31592ca295b45c495fb61e1a88334f5;Host=localhost;Port=5432"
    let repository ()=
        let postgres = new NpgsqlConnection(connString)
        postgres.Open()
        Repository(postgres) :> IRepository

    do
        let migrator = MigrationRunner.create connString "PostgreSQL"
        
        migrator.MigrateUp()
        Dapper.FSharp.OptionTypes.register()

        //_container.Boot();
        let commands = getCommands()
        let repository = repository()
        let run = Command.run repository
        let t = task{
            for c in commands do
                do! run c.Command
        }
        t.ConfigureAwait(true).GetAwaiter().GetResult()

    [<Fact>]
    member this.CanGetCustomerById()=task{
        let repository = repository()
        let! c = repository.GetCustomer(1)
        Assert.NotNull(c)}

    [<Fact>]
    member this.CanGetProductById()=task{
        let repository = repository()
        let! p= repository.GetProduct(1)
        Assert.NotNull(p)}

    [<Fact>]
    member this.OrderContainsProduct()=task{
        let repository = repository()
        let! products = repository.GetOrderProducts(1)
        
        Assert.True(products |> Seq.tryFind( fun p -> p.Id = 1) |> Option.isSome) }

    //[<Test>]
    //member this.OrderHasACustomer()=
    //    Assert.IsNotNullOrEmpty(_repository.GetTheCustomerOrder(1).Firstname)