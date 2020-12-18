import { contramap, Eq, fromEquals } from "fp-ts/lib/Eq"
import { type } from "os"

type User = {
  id: number
  name: string
}

describe("Eq test", () => {
  const getEq: <A>(E: Eq<A>) => Eq<Array<A>> = (E) => {
    return {
      equals: (xs, ys) => xs.length === ys.length && xs.every((x, i) => E.equals(x, ys[i]))
    }
  }

  const eqNumber: Eq<number> = {
    equals: (x, y) => x === y
  }

  it("eq Number", () => {
    const eqArrayOfNumber: Eq<Array<number>> = getEq(eqNumber)
  })

  it("eq user", () => {
    const eqUser: Eq<User> = contramap((user: User) => user.id)(eqNumber)
    const userA = {id: 1, name: "userA"}
    const userB = {id: 2, name: "userB"}

    const equal = eqUser.equals(userA, userB)

    expect(equal).toBeFalsy()
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
