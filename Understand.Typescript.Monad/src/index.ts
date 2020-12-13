const world = 'world';

console.log('hello, world')
export function sayHello(word: string = world): string {
    return `Hello ${world}! `;
}
