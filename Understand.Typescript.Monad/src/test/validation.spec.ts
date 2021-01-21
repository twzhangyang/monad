import { Either, getValidation, left, map, mapLeft, right } from "fp-ts/lib/Either"
import { pipe } from "fp-ts/pipeable"
import { chain } from "fp-ts/es6/Either"
import { getSemigroup, NonEmptyArray } from "fp-ts/NonEmptyArray"
import { sequenceT } from "fp-ts/Apply"

const minLength = (s: string): Either<string, string> =>
  s.length >= 6 ? right(s) : left("at least 6 characters")

const oneCapital = (s: string): Either<string, string> =>
  /[A-Z]/g.test(s) ? right(s) : left("at least one capital letter")

const oneNumber = (s: string): Either<string, string> =>
  /[0-9]/g.test(s) ? right(s) : left("at least one number")

const validatePassword = (s: string): Either<string, string> =>
  pipe(
    minLength(s),
    chain(oneCapital),
    chain(oneNumber)
  )

const applicativeValidation = getValidation(getSemigroup<string>())

const lift = <E, A>(check: (a: A) => Either<E, A>): (a: A) => Either<NonEmptyArray<E>, A> => {
  return a => pipe(
    check(a),
    mapLeft(a => [a])
  )
}

const minLengthV = lift(minLength)
const oneCapitalV = lift(oneCapital)
const oneNumberV = lift(oneNumber)

const validatePasswordV = (s: string): Either<NonEmptyArray<string>, string> => {
  return pipe(
    sequenceT(getValidation(getSemigroup<string>()))(
      minLengthV(s),
      oneCapitalV(s),
      oneNumberV(s),
    ),
    map(() => s)
  )
}

interface Person {
  name: string
  age: number
}

// Person constructor
const toPerson = ([name, age]: [string, number]): Person => ({
  name,
  age
})

const validateName = (s: string): Either<NonEmptyArray<string>, string> =>
  s.length === 0 ? left(['Invalid name']) : right(s)

const validateAge = (s: string): Either<NonEmptyArray<string>, number> =>
  isNaN(+s) ? left(['Invalid age']) : right(+s)

function validatePerson(name: string, age: string): Either<NonEmptyArray<string>, Person> {
  return pipe(
    sequenceT(applicativeValidation)(validateName(name), validateAge(age)),
    map(toPerson)
  )
}

describe
("test either validation", () => {
  it("validate password", () => {
    console.log(validatePassword("ab"))
  })
})

function specializedSequenceT(
  firstValidation: Either<NonEmptyArray<string>, string>,
  secondValidation: Either<NonEmptyArray<string>, string>,
  thirdValidation: Either<NonEmptyArray<string>, string>
): Either<NonEmptyArray<string>, [string, string, string]> {
  // Applicative instance for `Either<NonEmptyArray<string>, A>`
  const V = getValidation(getSemigroup<string>())

  // builds a tuple from three strings
  const tuple = (a: string) => (b: string) => (c: string): [string, string, string] => [a, b, c]

  const temp = V.map(firstValidation, tuple)
  // manual lifting, check out the "Lifting" section in "Getting started with fp-ts: Applicative"
  return V.ap(V.ap(V.map(firstValidation, tuple), secondValidation), thirdValidation)
}

function validatePassword3(s: string): Either<NonEmptyArray<string>, string> {
  return pipe(
    specializedSequenceT(minLengthV(s), oneCapitalV(s), oneNumberV(s)),
    map(() => s)
  )
}
