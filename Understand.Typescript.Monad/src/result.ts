type Curried<T, U> = (a: Result<T>) => U;

export class WrappedResultError extends Error {
  constructor(message: string, public originalError: unknown) {
    super();
  }
}



export class Result<T> {
  // constructor(private value: Under) {
  // }
}
