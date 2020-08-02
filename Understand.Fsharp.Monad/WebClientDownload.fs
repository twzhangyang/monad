namespace Understand.Fsharp.Monad.WebClientDownload

open System.Collections.Generic
open Understand.Fsharp.Monad.XResult

[<Measure>]
type ms

type WebClientWithTimeout(timeout: int<ms>) =
  inherit System.Net.WebClient()

  override this.GetWebRequest(address) =
    let result = base.GetWebRequest(address)
    result.Timeout <- int timeout
    result

type UriContent = UriContent of System.Uri * string

type UriContentSize = UriContentSize of System.Uri * int

module Asyncx =
  let map f xAsync = async {
    let! x = xAsync
    return f x
  }

  let bind f xAsync = async {
    let! x = xAsync
    return! f x
  }

  let retn x = async {
    return x
  }

  let apply fAsync xAsync = async {
    let! fChild = Async.StartChild fAsync
    let! xChild = Async.StartChild xAsync

    let! f = fChild
    let! x = xChild

    return f x
  }

  let rec traverseAsyncA f list =
    let (<*>) = apply
    let cons head tail = head :: tail
    let initState = retn []
    let fold head tail =
      retn cons <*> (f head) <*> tail

    List.foldBack fold list initState

  let sequenceAsyncA x = traverseAsyncA id x


module download =
  let getUriContent (uri: System.Uri) =
    async {
      use client = new WebClientWithTimeout(1000<ms>)
      try
        printfn " [%s] Started ..." uri.Host
        let! html = client.AsyncDownloadString(uri)
        printfn " [%s] ... finished" uri.Host
        let uriContent = UriContent(uri, html)
        return result.Success uriContent
      with ex ->
        printfn " [%s] ... exception" uri.Host
        let err = sprintf " [%s] %A" uri.Host ex.Message
        return result.Failure [ err ]
    }

  let showContentSizeResult result =
    match result with
    | result.Success (UriContentSize (uri, length)) ->
        printfn "Success: [%s] content size is %i" uri.Host length
    | result.Failure errs ->
        printfn "Failure: %A" errs

  let getContentSize (UriContent (uri, html)) =
    if System.String.IsNullOrEmpty html then
      result.Failure [ "empty page" ]
    else
      let content = UriContentSize(uri, html.Length)
      result.Success content

  let run1 =
    System.Uri("http://xxx")
    |> getUriContent
    |> Asyncx.map (result.bind getContentSize)
    |> Async.RunSynchronously
    |> showContentSizeResult

  let maxContentSize list =
    let contentSize (UriContentSize (_, len)) = len
    list |> List.maxBy contentSize

  let largestPageSizeA urls =
    let urls = [""; ""]
    let size = urls
               |> List.map (fun u -> System.Uri(u))
               |> List.map getUriContent
               |> List.map (Asyncx.map (result.bind getContentSize))
               |> Asyncx.sequenceAsyncA
               |> Asyncx.map result.sequenceResultA
               |> Asyncx.map (result.map maxContentSize)
    size

  let largestPageSizeA' urls =
    let urls = [""; ""]
    let size = urls
              |> Asyncx.traverseAsyncA (fun x -> System.Uri(x) |> getUriContent |> (Asyncx.map (result.bind getContentSize)))
              |> Asyncx.map (result.sequenceResultA >> result.map maxContentSize)
    size
