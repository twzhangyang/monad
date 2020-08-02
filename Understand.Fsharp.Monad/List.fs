namespace Understand.Fsharp.Monad.XList

open Xunit

module list =
  let rec mapList f list =
    match list with
    | head::tail ->
      (f head) :: (mapList f tail)
    | [] -> []


module Tests =
  open list

  [<Fact>]
  let ``test list`` () =
    let list = [1; 2; 3]

    let result = list
                 |> mapList (fun x -> x + 1)

    Assert.StrictEqual(result, [2; 3; 4])


