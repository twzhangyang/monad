import { fromNullable, map, some, none, chain } from "fp-ts/Option";
import { pipe } from "fp-ts/pipeable";
import * as Array from "fp-ts/Array";
import * as Option from "fp-ts/Option";

describe("test options", () => {
  const parseId = (idString: string): Option.Option<number> => {
    const id = Number(idString);
    return isNaN(id) ? none : some(id);
  }

  test("some", () => {
    const a = some("hello");
    const b = fromNullable(null);
  });

  test("map", () => {
    const someA = some(1);
    const b = map((x: number) => x + 1)(someA);
  });

  test("pipe", () => {
    const result = pipe(
      some(1),
      map(x => x + 1)
    );
  });

  test("signature", () => {
    // const add = (a: number, b: number): number => a + b;

    // const add = (a: number) => (b:number): number => a + b;
    // const one = add(1);
    // const add10 = one(10);
    //
    // expect(add10).toBe(11);

    const update = (name: string): boolean => true
  });

  test("chain", () => {
    const getId = (id: string): Option<Number> => some(1);
    const result = pipe(
      some("1"),
      chain(getId)
    )
  })

  test("parse int", () => {


    expect(parseId("aa")).toEqual(none);
    expect(parseId("12")).toEqual(some(12));


    pipe(
      some("123"),
      chain(parseId),
      map(x => x + 1),
    )
  })

  test("traversable", () => {
    const ids = Array.of("1");

    const result = pipe(
      Array.of("1"),
      Array.map(parseId)
    )

    const result1 = pipe(
      Array.of("1"),
      Array.map(parseId),
      Option.sequenceArray
    )

    const result2 = pipe(
      Array.of("1"),
      Option.traverseArray(parseId)
    )

  })
});
