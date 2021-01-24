import { fold, Monoid } from "fp-ts/Monoid";
import { chain, IO } from "fp-ts/IO";
import { replicate } from "fp-ts/Array";
import { pipe } from "fp-ts/pipeable";
import { type } from "os";

describe("invent monoid", () => {
  it("test1", () => {
    type Money = {
      amount: number
    };

    const a: Money = { amount: 10.2 };
    const b: Money = { amount: 2.8 };
    const c: Money = { amount: a.amount + b.amount };
  });

  it("test2", () => {
    const a = [1, 2, 3];
    const b: number[] = [];
    const c = [...a, ...b];

    expect(c).toEqual(a);
  });

  it("test3", () => {
    const a: number[] = [];
    const result = a.reduce((acc, val) => acc + val, 0);

    expect(result).toBe(0);
  });

  it("test4", () => {
    [1, 2, 3, 4].reduce((acc, val) => acc + val);
    ["a", "b", "c", "d"].reduce((acc, val) => acc + val);

    type concat = <A>(x: A, y: A) => A
    type concat1 = <T1, T2>(x: T1) => T2
  });

  it("test5", () => {
    type OrderLine = {
      name: string,
      quality: number,
      price: number,
      total: number
    }

    const calculateOrderCart = (orderLines: OrderLine[]) => {
      let total = 0;
      for (let i = 0; i < orderLines.length; i++) {
        total += orderLines[i].total;
      }
      return total;
    };

    const orderLines: OrderLine[] = [
      { name: "name1", quality: 1, price: 1.2, total: 1.2 },
      { name: "name2", quality: 2, price: 1.1, total: 2.2 },
      { name: "name3", quality: 2, price: 1.8, total: 3.6 }
    ];

    expect(calculateOrderCart(orderLines)).toBe(7);
    // expect(orderLines.reduce((acc, val) => acc.total + val.total))

    const addTwoOrderLines = (line1: OrderLine, line2: OrderLine): OrderLine => (
      {
        name: "total",
        quality: line1.quality + line2.quality,
        price: 1,
        total: line1.total + line2.total
      }
    );

    expect(orderLines.reduce(addTwoOrderLines)).toEqual({
      name: "total",
      quality: 5,
      price: 1,
      total: 7
    });

  });

  interface Monoid<A> {
    readonly concat: (x: A, y: A) => A
    readonly empty: A
  }
});

describe("Monoid test", () => {
  it("get IO monoid", () => {
    const getIOMonoid: <A>(M: Monoid<A>) => Monoid<IO<A>> = (M) => {
      return {
        concat: (x, y) => () => M.concat(x(), y()),
        empty: () => M.empty
      };
    };

    const monoidVoid: Monoid<void> = {
      concat: () => undefined,
      empty: undefined
    };

    const replicateIO = (n: number, mv: IO<void>): IO<void> => fold(getIOMonoid(monoidVoid))(replicate(n, mv));

    const log = (message: unknown): IO<void> => () => console.log(message);

    const randomInt = (low: number, high: number): IO<number> => {
      return () => Math.floor((high - low + 1) * Math.random() + low);
    };

    const fib = (n: number): number => {
      return n <= 1 ? 1 : fib(n - 1) + fib(n - 2);
    };

    const printFib: IO<void> = pipe(
      randomInt(30, 35),
      chain(n => log(fib(n)))
    );

    replicateIO(3, printFib)();
  });
});
