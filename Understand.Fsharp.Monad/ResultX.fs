module Understand.Fsharp.Monad.ResultX

type Result<'TSuccess, 'TMessage> =
    | Success of 'TSuccess * 'TMessage list
    | Failure of 'TMessage list

let succeed x =
    Success (x, [])

let succeedWithMsg x msg =
    Success (x, [msg])

let fail msg =
    Failure [msg]

let either fSuccess fFailure = function
    | Success (x, msgs) -> fSuccess (x, msgs)
    | Failure msgs -> fFailure msgs

let mergeMessages msgs result =
    let fSuccess (x, msg2) =
        Success (x, msg2 @ msgs)
    let fFailure errs =
        Failure (errs @ msgs)

    either fSuccess fFailure result

let bind f result =
    let fSuccess (x, msgs) =
        f x |> mergeMessages msgs

    let fFailure errs =
        Failure errs

    either fSuccess fFailure result

let apply f result =
    match f, result with
    | Success (f, msg1), Success (x, msg2) ->
        (f x, msg1 @ msg2) |> Success
    | Failure errs, Success (_, msgs)
    | Success (_, msgs), Failure errs ->
        errs @ msgs |> Failure
    | Failure errs1, Failure errs2 ->
        errs1 @ errs2 |> Failure

let (<*>) = apply

let lift f result =
    let f' = f |> succeed

    apply f' result

let lift2 f result1 result2 =
    let f' = lift f result1

    apply f' result2

let lift3 f result1 result2 result3 =
    let f' = lift2 f result1 result2

    apply f' result3

let lift4 f result1 result2 result3 result4 =
    let f' = lift3 f result1 result2 result3

    apply f' result4

let lift5 f result1 result2 result3 result4 result5 =
    let f' = lift4 f result1 result2 result3 result4

    apply f' result5

let (<!>) = lift
let map = lift

let successTee f result =
    let fSuccess (x, msg) =
        f (x, msg)
        Success (x, msg)

    let fFailure errs = Failure errs
    either fSuccess fFailure result

let failureTee f result =
    let fSuccess (x, msgs) = Success (x, msgs)
    let fFailure errs =
        f errs
        Failure errs

    either fSuccess fFailure result

let mapMessage f result =
    match result with
    | Success (x, msgs) ->
        let msgs' = List.map f msgs
        Success (x, msgs')
    | Failure errs ->
        let errs' = List.map f errs
        Failure errs'

let valueOrDefault f result =
    match result with
    | Success (x, _) -> x
    | Failure errs -> f errs

let failIfNone message = function
    | Some value -> value
    | None -> fail message


