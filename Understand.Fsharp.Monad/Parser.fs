namespace Understand.Fsharp.Monad

open System
open Understand.Fsharp.Monad.XResult

module Parser =
  let A_Parser str =
    if String.IsNullOrEmpty(str) then
      (false, "")
    else if str.[0] = 'A' then
      let remaining = str.[1..]
      (true, remaining)
    else
      (false, str)

  let pChar charToMatch str =
    if String.IsNullOrEmpty str then
      result.Failure ["No more input"]
    else
      let first = str.[0]
      if first = charToMatch then
        result.Success(first, str.[1..])
      else
        let msg = sprintf "Expecting '%c', Got '%c'" charToMatch first
        result.Failure [msg]

  let pChar' charToMatch =
    let inner str =
      if String.IsNullOrEmpty str then
        result.Failure ["No more input"]
      else
      let first = str.[0]
      if first = charToMatch then
        result.Success(first, str.[1..])
      else
        let msg = sprintf "Expecting '%c', Got '%c'" charToMatch first
        result.Failure [msg]
    inner

  type Parser<'T> = Parser of (string -> result.Result<'T>)

  let pCharFinal charToMatch =
    let inner str =
      if String.IsNullOrEmpty str then
        result.Failure ["No more input"]
      else
      let first = str.[0]
      if first = charToMatch then
        result.Success(first, str.[1..])
      else
        let msg = sprintf "Expecting '%c', Got '%c'" charToMatch first
        result.Failure [msg]
    Parser inner

  let run parser input =
    let (Parser inner) = parser
    inner input


  let parserA = pCharFinal 'A'
  let parserB = pCharFinal 'B'

  
