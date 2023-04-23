namespace SomeBasicFileStoreApp
open System
open System.Threading.Tasks
[<CLIMutable>]
type Customer = {Id:int; Firstname:string ; Lastname:string; Version:int}

[<CLIMutable>]
type Product = {Id:int; Cost:double; Name: string; Version: int}

[<CLIMutable>]
type Order = {Id:int; CustomerId:int; OrderDate:DateTimeOffset; Version: int}

[<CLIMutable>]
type OrdersToProduct = {OrderId: int; ProductId: int}
type IRepository=
    abstract member GetCustomer: int->Task<Customer option>
    abstract member GetProduct: int->Task<Product option>
    abstract member GetOrder: int->Task<Order option>
    abstract member GetOrderProducts: int->Task<Product seq>

    abstract member Insert: Customer->Task<int>
    abstract member Insert: Product->Task<int>
    abstract member Insert: Order->Task<int>
    abstract member Insert: OrdersToProduct->Task<int>



open Dapper
open System.Data
open System.Linq
open Dapper.FSharp.Builders
open Dapper.FSharp
open Dapper.FSharp.PostgreSQL
open FSharp.Control.Tasks 
module internal R=
    let selectOne<'T> (conn:IDbConnection) (tableName:string) (id:int)=task{
        let! maybeOne= conn.SelectAsync<'T> ( select {
            table tableName
            where (eq "Id" id)
        })
        return Seq.tryHead maybeOne
    }

type Repository(conn:IDbConnection)=

    interface IRepository with
        member __.GetCustomer(customerId) = R.selectOne<Customer> conn "Customers" customerId
        member __.GetProduct(productId) = R.selectOne<Product> conn "Products" productId
        member __.GetOrder(orderId) = R.selectOne<Order> conn "Orders" orderId
        member __.GetOrderProducts(orderId) = 
            let q = """SELECT p.* FROM "OrdersToProducts" otp JOIN "Products" p ON otp."ProductId" = p."Id" WHERE otp."OrderId" = @OrderId"""
            conn.QueryAsync<Product>(q,{| OrderId= orderId |})
        member __.Insert(o: Order)=conn.InsertAsync ( insert { table "Orders"; value o })
        member __.Insert(o: Product)=conn.InsertAsync ( insert { table "Products"; value o })
        member __.Insert(o: Customer)=conn.InsertAsync ( insert { table "Customers"; value o })
        member __.Insert(o: OrdersToProduct)=conn.InsertAsync ( insert { table "OrdersToProducts"; value o })

