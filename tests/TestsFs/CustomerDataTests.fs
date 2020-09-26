namespace CoreFsTests

open Xunit
open System.IO
open System
open System.Collections.Generic

open SomeBasicFileStoreApp
open GetCommands
open FSharp.Control.Tasks 
open Npgsql

type CustomerDataTests()=
    let connString= ""
    let postgres = new NpgsqlConnection(connString)
    let repository = Repository(postgres) :> IRepository

    do
        
        postgres.Open()
        //_container.Boot();
        let commands = getCommands()
        let run = Command.run repository
        let t = task{
            for c in commands do
                do! run c.Command
        }
        t.GetAwaiter().GetResult()

    [<Fact>]
    member this.CanGetCustomerById()=task{
        let! c = repository.GetCustomer(1)
        Assert.NotNull(c)}

    [<Fact>]
    member this.CanGetProductById()=task{
        let! p= repository.GetProduct(1)
        Assert.NotNull(p)}

    [<Fact>]
    member this.OrderContainsProduct()=task{
        let! order = repository.GetOrder(1)
        Assert.True(order.Value.Products |> List.tryFind( fun p -> p.Id = 1) |> Option.isSome) }

    //[<Test>]
    //member this.OrderHasACustomer()=
    //    Assert.IsNotNullOrEmpty(_repository.GetTheCustomerOrder(1).Firstname)