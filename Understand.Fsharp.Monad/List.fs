namespace Understand.Fsharp.Monad.XList

open Xunit

module list =
  let rec mapList f list =
    match list with
    | head::tail ->
      (f head) :: (mapList f tail)
    | [] -> []

  let apply (fList: ('a -> 'b) list) (xList: 'a list) =
    [ for f in fList do
      for x in xList do
        yield f x ]

  let rec zipList fList xList =
    match fList, xList with
    | [], _
    | _, [] -> []
    | (f::fTail), (x::xTail) ->
      (f x) :: (zipList fTail xTail)

  let rec bind f xList =
    match xList with
    | [] -> []
    | head::tail -> (f head) :: (bind f tail)


module Tests =
  open list

  [<Fact>]
  let ``test list`` () =
    let list = [1; 2; 3]

    let result = list
                 |> mapList (fun x -> x + 1)

    Assert.StrictEqual(result, [2; 3; 4])

  [<Fact>]
  let ``test apply`` () =
    let list = [1; 2; 3]
    let (<*>) = apply
    let add x y = x + y
    let result =[add] <*> [1; 2] <*> [10; 20]

    Assert.StrictEqual(result, [11; 21; 12; 22])


  [<Fact>]
  let ``zipList test`` () =
    let add10 x = x + 10
    let add20 y = y + 20
    let (<*>) = zipList

    let result = [add10; add20] <*> [1; 2]
    Assert.StrictEqual(result, [11; 22])


  [<Fact>]
  let ``bind test`` () =
    let list = [[1; 2]; [1; 2]]
    let result = bind (fun x -> [ for i in x do yield i]) list

    Assert.StrictEqual(result, [[1; 2];[1; 2]])
