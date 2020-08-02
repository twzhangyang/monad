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
    | Some x -> Some(f x)
    | None -> None

  let returnOption x = Some x

  let applyOption fOpt xOpt =
    match fOpt, xOpt with
    | Some f, Some x -> Some(f x)
    | _ -> None

  let (<*>) = applyOption
  let (<!>) = mapOption
  let lift2 f opt1 opt2 =
    f <!> opt1 <*> opt2

  let lift3 f opt1 opt2 opt3 =
    f <!> opt1 <*> opt2 <*> opt3

  let bind f opt =
    match opt with
    | Some x -> f x
    | None -> None

module Tests =
  open option

  [<Fact>]
  let ``map test`` () =
    let hello = Some "hello"

    let result =
      hello
      |> mapOption (fun a -> a + ", world!")
      |> getOrElse ""

    Assert.Equal(result, "hello, world!")

  [<Fact>]
  let ``add1 map test`` () =
    let add1 x = x + 1
    let add1IfSomething = add1 |> mapOption

    let three =
      Some 2
      |> add1IfSomething
      |> getOrElse 0

    Assert.Equal(three, 3)

  [<Fact>]
  let ``return test`` () =
    let two = returnOption 2 |> getOrElse 0

    Assert.Equal(two, 2)

  [<Fact>]
  let ``apply test`` () =
    let add x y = x + y
    let (<*>) = applyOption
    let result = (returnOption add) <*> (Some 2) <*> (Some 1)
                  |> getOrElse 0

    Assert.Equal(result, 3)

  [<Fact>]
  let ``lift 2 test`` () =
    let add x y = x + y

    let result = lift2 add (Some 1) (Some 2)
                |> getOrElse 0

    Assert.Equal(result, 3)

  [<Fact>]
  let ``bind test`` () =
    let parseInt str =
      match str with
      | "-1" -> Some(-1)
      | "0" -> Some(0)
      | _ -> None

    let result = bind parseInt (Some "-1")
                |> getOrElse 0

    Assert.Equal(result, -1)
