import { chain, Either, getValidation, left, mapLeft, right } from "fp-ts/lib/Either"
import { pipe } from "fp-ts/pipeable"
import { getSemigroup, NonEmptyArray } from "fp-ts/NonEmptyArray"
import { sequenceT } from "fp-ts/Apply"
import { map } from "fp-ts/es6/Either"

const minLength = (s: string): Either<string, string> => s.length >= 6 ? right(s) : left("at least 6 characters")
const oneCapital = (s: string): Either<string, string> => /[A-Z]/g.test(s) ? right(s) : left("at least 6 characters")
const oneNumber = (s: string): Either<string, string> => /[0-9]/g.test(s) ? right(s) : left("at east one number")

const validatePassword = (s: string): Either<string, string> =>
  pipe(minLength(s),
    chain(oneCapital),
    chain(oneNumber)
  )

const applicativeValidation = getValidation(getSemigroup<string>())
const lift = <E, A>(check: (a: A) => Either<E, A>): (a: A) => Either<NonEmptyArray<E>, A> => {
  return a => pipe(
    check(a),
    mapLeft(a => [a]))
}

const minLengthV = lift(minLength)
const oneCapitalV = lift(oneCapital)
const oneNumberV = lift(oneNumber)

const validatePasswordV = (s: string): Either<NonEmptyArray<string>, string> => {
  return pipe(
    sequenceT(getValidation(getSemigroup<string>()))(
      minLengthV(s),
      oneCapitalV(s),
      oneNumberV(s)
    ),
    map(() => s)
  )
}

console.log(validatePassword("ab"))

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
