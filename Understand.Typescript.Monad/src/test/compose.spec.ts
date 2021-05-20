import * as _ from "lodash";
import * as fp from "lodash/fp";
import { flow } from "fp-ts/function";
import exp from "constants";

describe("compose", () => {
  const add10 = (a: number) => a + 10;
  const sub10 = (a: number) => a - 10;

  test("add and sub", () => {
    const value11 = add10(1);
    const value1 = sub10(value11);

    expect(value1).toBe(1);
  });

  test("lodash compose", () => {
    const add = (a: number, b: number) => a + b;
    const sub1 = (a: number) => a - 1;

    const addThenSub1 = flow(add, sub1);
    expect(addThenSub1(1, 2)).toBe(2);

    const addThenSub10 = _.flow(add, sub10);
    expect(addThenSub10(1, 10)).toBe(1);

    expect(fp.flow(add, sub10)(1, 10)).toBe(1);

  });

  test("compose1", () => {
    const compose = (f: ((a: number) => number), g: (b: number) => number) => (x: number) => f(g(x));
    const double = (n: number) => n * 2;
    const inc = (n: number) => n + 1;
    const sub = (n: number) => n - 1;

    const incThenDouble = compose(double, inc);
    const incThenDoubleThenSub = compose(sub, incThenDouble)
    expect(incThenDoubleThenSub(3)).toBe(sub(double(inc(3))));
  });
});

