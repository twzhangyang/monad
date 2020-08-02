namespace Understand.Fsharp.Monad.ReaderX

open Understand.Fsharp.Monad.XResult
open Understand.Fsharp.Monad.XResult.result

type CustId = CustId of string

type ProductId = ProductId of string

type ProductInfo =
  { ProductName: string }

type ApiClient() =
  static let mutable data = Map.empty<string, obj>

  member private this.TryCast<'a> key (value: obj) =
    match value with
    | :? 'a as a ->
        result.Success a
    | _ ->
        let typeName = typeof<'a>.Name
        result.Failure [ typeName ]

  member this.Get<'a>(id: obj) =
    let key = sprintf "%A" id
    match Map.tryFind key data with
    | Some o ->
        this.TryCast<'a> key o
    | None ->
        result.Failure [ "Key %s not found" ]

  member this.Set (id: obj) (value: obj) =
    let key = sprintf "%A" id
    printfn "[API] set %s" key
    if key = "bad" then
      result.Failure [ sprintf "Bad key %s" key ]
    else
      data <- Map.add key value data
      result.Success()

  member this.Open() = printfn "[API] Opening"

  member this.Close() = printfn "[API] Closing"

  interface System.IDisposable with
    member this.Dispose() = printfn "[API] Disposing"

type ApiAction<'a> = ApiAction of (ApiClient -> 'a)

module ApiAction =
  let run api (ApiAction action) =
    let resultOfAction = action api
    resultOfAction

  let map f action =
    let newAction api =
      let x = run api action
      f x
    ApiAction newAction

  let retn x =
    let newAction api =
      x
    ApiAction newAction

  let apply fAction xAction =
    let newAction api =
      let f = run api fAction
      let x = run api xAction
      f x
    ApiAction newAction

  let execute action =
    use api = new ApiClient()
    api.Open()
    let result = run api action
    api.Close()
    result


module ApiActionResult =

  let map f =
    ApiAction.map (result.map f)

  let retn x =
    ApiAction.retn (result.returnResult x)

  let apply fActionResult xActionResult =
    let newAction api =
      let fResult = ApiAction.run api fActionResult
      let xResult = ApiAction.run api xActionResult
      result.apply fResult xResult
    ApiAction newAction

  let bind f xActionResult =
    let newAction api =
      let xResult = ApiAction.run api xActionResult
      // create a new action based on what xResult is
      let yAction =
        match xResult with
        | Success x ->
            // Success? Run the function
            f x
        | Failure err ->
            // Failure? wrap the error in an ApiAction
            (Failure err) |> ApiAction.retn
      ApiAction.run api yAction
    ApiAction newAction

  let traverse f list =
    // define the applicative functions
    let (<*>) = apply

    // define a "cons" function
    let cons head tail = head :: tail

    // right fold over the list
    let initState = retn []
    let folder head tail =
      retn cons <*> f head <*> tail

    List.foldBack folder list initState

module Reader =
  let getPurchaseInfo (custId: CustId): Result<ProductInfo list> =
    use api = new ApiClient()
    api.Open()

    let producIdsResult = api.Get<ProductId list> custId

    //    let productInfosResult=

    api.Close()
    result.Failure [ "hello" ]

  let getPurchaseInfo' (custId: CustId): Result<ProductInfo list> =
    use api = new ApiClient()
    api.Open()

    let productInfosResult =
      result.ResultBuilder {
        let! productIds = api.Get<ProductId list> custId
        let productInfos = ResizeArray()
        for productId in productIds do
          let! productInfo = api.Get<ProductInfo> productId
          productInfos.Add productInfo
        return productInfos |> List.ofSeq
      }

    api.Close()
    productInfosResult ()

  let executeApiAction apiAction =
    use api = new ApiClient()
    api.Open()
    let result = apiAction api
    api.Close()
    result


  let getPurchaseIds (custId: CustId) =
    let action (api: ApiClient) =
      api.Get<ProductId list> custId

    ApiAction action

  let getProductInfo (productId: ProductId) =
    let action (api: ApiClient) =
      api.Get<ProductInfo> productId

    ApiAction action

  let getPurchaseInfo'' =
      let getProductInfo1 = ApiActionResult.traverse getProductInfo
      let getProductInfo2 = ApiActionResult.bind getProductInfo1
      getPurchaseIds >> getProductInfo2


  module Tests =
    let ``get set dic tests`` () =
      use api = new ApiClient()
      api.Get "K1" |> printfn "[K1] %A"
      api.Set "K2" "hello" |> ignore
      api.Get<string> "K2" |> printfn "[K2] %A"




