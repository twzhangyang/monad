import { fold, Monoid } from "fp-ts/Monoid"
import { chain, IO } from "fp-ts/IO"
import { replicate } from "fp-ts/Array"
import { pipe } from "fp-ts/pipeable"

describe("Monoid test", () => {
  it("get IO monoid", () => {
    const getIOMonoid: <A>(M: Monoid<A>) => Monoid<IO<A>> = (M) => {
      return {
        concat: (x, y) => () => M.concat(x(), y()),
        empty: () => M.empty
      }
    }

    const monoidVoid: Monoid<void> = {
      concat: () => undefined,
      empty: undefined
    }

    const replicateIO = (n: number, mv: IO<void>): IO<void> => fold(getIOMonoid(monoidVoid))(replicate(n, mv))

    const log = (message: unknown): IO<void> => () => console.log(message)

    const randomInt = (low: number, high: number): IO<number> => {
      return () => Math.floor((high - low + 1) * Math.random() + low)
    }

    const fib = (n: number): number => {
      return n <= 1 ? 1 : fib(n - 1) + fib(n - 2)
    }

    const printFib: IO<void> = pipe(
      randomInt(30, 35),
      chain(n => log(fib(n)))
    )

    replicateIO(3, printFib)()
  })
})
