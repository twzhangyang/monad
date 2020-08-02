namespace Understand.Fsharp.Monad.XOption

open Xunit

module option =
  type Option<'a> =
    | Some of 'a
    | None

  let getOrElse value opt =
    match opt with
    | Some x -> x
    | None -> value

  let mapOption f opt =
    match opt with
    | Some x -> Some (f x)
    | None -> None

module Tests =

  open option

  [<Fact>]
  let ``map test`` () =
    let hello = Some "hello"
    let result = hello
                 |> mapOption (fun a -> a + ", world!")
                 |> getOrElse ""

    Assert.Equal(result, "hello, world!")
