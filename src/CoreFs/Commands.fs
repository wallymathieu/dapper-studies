namespace SomeBasicFileStoreApp
open System
open System.Threading.Tasks
open FSharpPlus
open FSharp.Control.Tasks 

type Command = 
    | Empty
    | AddCustomerCommand of id:int * version:int * firstName:string * lastName:string
    | AddOrderCommand of id:int * version:int * customer:int * orderDate:DateTime
    | AddProductCommand of id:int * version:int * name:string * cost:decimal
    | AddProductToOrder of orderId:int * productId:int

module Command=
    let run (repository:IRepository) command=
        let taskTrue = task {return true}
        match command with
            | AddCustomerCommand(id=id ;version=version; firstName=firstName; lastName=lastName) ->task{
                let! notExisting = if id<=0 then taskTrue else repository.GetCustomer id |> Task.map Option.isNone
                if notExisting then
                    let! _ = repository.Insert({
                                     Id=id
                                     FirstName=firstName
                                     LastName=lastName
                                     Version=version
                                    })
                    ()
                }
            | AddOrderCommand(id=id; version=version; customer=customer; orderDate=orderDate)->task{
                let! notExisting = if id<=0 then taskTrue else repository.GetOrder id |> Task.map Option.isNone
                let! c = repository.GetCustomer customer
                match c, notExisting with
                | Some customer, true->
                    let! _ = repository.Insert({
                                    Id=id
                                    OrderDate=orderDate
                                    Version=version
                                    Customer=customer 
                                    Products=List.empty
                                   })
                    ()
                | _,_ -> () }
            | AddProductCommand(id=id; version=version; name=name; cost=cost)->task{
                let! notExisting = if id<=0 then taskTrue else repository.GetProduct id |> Task.map Option.isNone
                if notExisting then 
                    let! _ = repository.Insert({
                                     Id=id
                                     Version=version
                                     Cost=cost
                                     Name=name
                                    })
                    ()
                }
            | AddProductToOrder(orderId=orderId; productId=productId)->task{
                let! order = repository.GetOrder(orderId)
                let! product = repository.GetProduct(productId)
                match order,product with
                | Some order, Some product ->
                    repository.Insert({order with Products= product :: order.Products}) |> ignore
                | _, _ -> ()
                }
            | Empty -> Task.FromResult ()


