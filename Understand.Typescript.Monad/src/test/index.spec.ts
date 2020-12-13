import { sayHello } from "../index"

describe("test index", () => {
  it("test hello", () => {
    const hello = sayHello();

    expect(hello).toEqual("Hello world! ");
  })
})
