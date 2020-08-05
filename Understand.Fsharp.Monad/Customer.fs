namespace Understand.Fsharp.Monad

open System
open System.Collections.Generic
open Understand.Fsharp.Monad.ResultX

type CustomerInput =
  { FirstName: string
    LastName: string
    Email: string
    Address: string }


module CustomerDomain =
  type Customer =
    { Id: Guid
      FirstName: string
      LastName: string
      Email: string
      Address: string }

  type ResponseException =
    | NotFound of int * string
    | BadRequest of int * string
    | InternalServerError of int * string
    | UnAuthorized of int * string

  let validateName name =
    if System.String.IsNullOrEmpty name
    then fail (BadRequest(400, "Name is required"))
    elif name.Length > 10
    then fail (BadRequest(400, "Name no more than 10 chars"))
    else succeed name

  let validateEmail email =
    if System.String.IsNullOrEmpty email
    then fail (BadRequest(400, "Email is required"))
    elif not (email.Contains('@'))
    then fail (BadRequest(400, "Invalid email"))
    else succeed email

  let createCustomer id firstName lastName email address =
    { Id = id
      FirstName = firstName
      LastName = lastName
      Email = email
      Address = address }

  let inputToDomain (input: CustomerInput) =
    let firstName = validateName input.FirstName
    let lastName = validateName input.LastName
    let email = validateEmail input.Email
    let address = succeed input.Address
    let id = succeed (Guid.NewGuid())
    lift5 createCustomer id firstName lastName email address

module DbService =
  open CustomerDomain

  type ICustomerDbService =
    abstract Insert: Customer -> Result<Customer, ResponseException>

  type CustomerDbService() =
    static let _data = new Dictionary<Guid, Customer>()

    interface ICustomerDbService with
      member this.Insert(customer: Customer) =
        _data.[customer.Id] <- customer
        succeed (customer)

module Controller =
  open CustomerDomain
  open DbService

  type CreateCustomerResponse =
    { id: Guid }

  let log format (objs: obj []) =
    Console.WriteLine("log" + String.Format(format, objs))

  let logFailure result =
    let logError err = log "Error: {0}" [| sprintf "%A" err |]
    failureTee (Seq.iter logError) result

  let createCustomerResponse (customer: Customer): CreateCustomerResponse =
    { id = customer.Id }

  let throwExceptions exceptions =
    exceptions |> Seq.iter (fun ex -> failwith "exception")
    { id = Guid.Empty }


  let createCustomer (input: CustomerInput): CreateCustomerResponse =
    let customerDbService: ICustomerDbService = CustomerDbService() :> ICustomerDbService

    let result =
      succeed input
      |> bind inputToDomain
      |> bind customerDbService.Insert
      |> logFailure
      |> map createCustomerResponse
      |> valueOrDefault throwExceptions

    result
