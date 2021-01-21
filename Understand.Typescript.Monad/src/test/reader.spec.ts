import * as E from "fp-ts/lib/Either";
import { pipe } from "fp-ts/lib/pipeable";
import { flow } from "fp-ts/function";
import * as RE from "fp-ts/lib/ReaderEither";
import { ask, chain, chainW, Reader } from "fp-ts/lib/Reader";

describe("reader monad", () => {

  it("test", () => {

    const f = (b: boolean): ((deps: Dependencies) => string) => deps => (b ? deps.i18n.true : deps.i18n.false);

    const g = (n: number): ((deps: Dependencies) => string) => f(n > 2);

    const h = (s: string): ((deps: Dependencies) => string) => g(s.length + 1);

    interface Dependencies {
      i18n: {
        true: string
        false: string
      }
      lowerBound: number
    }

    const instance: Dependencies = {
      i18n: {
        true: "vero",
        false: "falso"
      },
      lowerBound: 2
    };

    const com = (n: number): Reader<Dependencies, string> =>
      pipe(
        ask<Dependencies>(),
        chain(deps => f(n > deps.lowerBound))
      );

    const result = g(1)(instance);
  });

});

describe("either monad", () => {
  it("test", () => {
    declare function f(s: string): E.Either<Error, number>

    declare function g(n: number): boolean

    declare function h(b: boolean): E.Either<Error, Date>

// composing `f`, `g`, and `h` -------------v---------v-----------v
    const result = pipe(E.right("foo"), E.chain(f), E.map(g), E.chain(h));

    const pointFreeVersion = flow(f, E.map(g), E.chain(h));

  });
});


describe("reader either monad", () => {
  it("test", () => {
    interface Dependencies {
      foo: string
    }

    declare function f(s: string): RE.ReaderEither<Dependencies, Error, number>

    declare function g(n: number): boolean

    declare function h(b: boolean): E.Either<Error, Date>

    const result = pipe(
      RE.right("foo"),
      RE.chain(f),
      RE.map(g),
      RE.chain(b => RE.fromEither(h(b)))
    );

    const pointFreeVersion = flow(
      f,
      RE.map(g),
      RE.chain(b => RE.fromEither(h(b)))
    );

    const dep: Dependencies = { foo: "yang" };
    const result1 = result(dep);

  });

  it("test2", () => {
    interface Dependencies {
      readonly i18n: {
        readonly true: string
        readonly false: string
      }
      readonly lowerBound: number
    }

    interface OtherDependencies {
      readonly semicolon: boolean
    }

    declare function transform(a: string): Reader<OtherDependencies, string>

    declare function anotherTransform(a: string): Reader<OtherDependencies, string>

    declare function f(b: boolean): Reader<Dependencies, string>

    const g = (n: number) => pipe(f(n > 2), chainW(transform), chainW(anotherTransform));
  });

});

