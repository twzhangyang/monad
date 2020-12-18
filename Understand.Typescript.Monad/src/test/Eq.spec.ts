import { Eq, fromEquals } from "fp-ts/lib/Eq"

describe("Eq test", () => {
  const getEq: <A>(E: Eq<A>) => Eq<Array<A>> = (E) => {
    return {
      equals: (xs, ys) => xs.length === ys.length && xs.every((x, i) => E.equals(x, ys[i]))
    }
  }

  const map1: <A, B>(f: (b: B) => A) => (fa: Eq<A>) => Eq<B> =
    (f) => (fa) => fromEquals((x, y) => fa.equals(f(x), f(y)))


  it("eq Number", () => {
    const eqNumber: Eq<number> = {
      equals: (x, y) => x === y
    }

    const eqArrayOfNumber: Eq<Array<number>> = getEq(eqNumber)
  })

  it("function signature", () => {

    function add<A>(x: A, y: A): A {
      return x
    }

    const add1: (a: number, b: number) => number = (x: number, y: number): number => x + y

    const add2 = <A>(a: A, b: A): A => a

    const add3: <A>(a: A, b: A) => A = (a, b) => a

    const add4: <A>(a: A) => (b: A) => A = a => b => a

    const result = add4(1)(2)
  })
})
