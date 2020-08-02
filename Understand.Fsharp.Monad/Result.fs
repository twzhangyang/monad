namespace Understand.Fsharp.Monad.XResult

module result =
  type Result<'a> =
    | Success of 'a
    | Failure of string list

  let map f xResult =
    match xResult with
    | Success x -> Success(f x)
    | Failure errs -> Failure errs

  let returnResult x = Success x

  let apply fResult xResult =
    match fResult, xResult with
    | Success f, Success x ->
        Success(f x)
    | Failure errs, Success x ->
        Failure errs
    | Success f, Failure errs ->
        Failure errs
    | Failure err1, Failure err2 ->
        Failure(List.concat ([ err1; err2 ]))

  let bind f xResult =
    match xResult with
    | Success x -> f x
    | Failure err -> Failure err

  type ResultBuilder() =
    member this.Return x = returnResult x
    member this.Bind(x, f) = bind f x

  let ResultBuilder = new ResultBuilder()

module Tests =
  open result

  let (<!>) = map
  let (<*>) = apply

  type CustomerId = CustomerId of int

  type EmailAddress = EmailAddress of string

  type CustomerInfo =
    { id: CustomerId
      email: EmailAddress }

  let createCustomerId id =
    if id > 0
    then Success(CustomerId id)
    else Failure [ "CustomerId must e positive" ]

  let createEmailAddress str =
    if System.String.IsNullOrEmpty(str) then Failure [ "Email must not be empty" ]
    elif str.Contains("@") then Success(EmailAddress str)
    else Failure [ "Email must contain @" ]

  let createCustomer id email =
    { id = id
      email = email }

  let ``validation using applicative style`` () =
    let idResult = createCustomerId 1
    let emailResult = createEmailAddress "xxx"
    createCustomer <!> idResult <*> emailResult

  let (>>=) x f = bind f x

  let ``validation using monadic style`` () =
    createCustomerId 0
    >>= (fun customerId ->
    createEmailAddress "xxx"
    >>= (fun emailAddress ->
    let customer = createCustomer customerId emailAddress
    Success customer))

  let ``validation using computation expressions`` () =
    ResultBuilder {
      let! customerId = createCustomerId 0
      let! emailAddress = createEmailAddress "xxx"
      let customer = createCustomer customerId emailAddress
      return customer
    }
