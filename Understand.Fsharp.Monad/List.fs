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

