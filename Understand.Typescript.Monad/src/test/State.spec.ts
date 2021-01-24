import { of, map, execute, chain, get, evaluate, put, modify, gets } from "fp-ts/State";
import { pipe } from "fp-ts/pipeable";
import exp from "constants";

type Result = {
  num: number
}

describe("state monad", () => {
  const init: Result = { num: 1 };

  it("test1", () => {
    const r = pipe(
      of<Result, string>("hello"),
      map(x => "hello1"),
      execute(init)
    );


    expect(r).toEqual({ num: 1 });
  });

  it("test2", () => {
    const state = get<Result>();
    const a = evaluate(init)(state);

    expect(a).toEqual(init);
  });

  it("test3", () => {
    const result = pipe(
      gets<Result, string>(x => "hello1"),
      map(() => "hello"),
      evaluate(init)
    )

    expect(result).toEqual("hello")
  })
});
