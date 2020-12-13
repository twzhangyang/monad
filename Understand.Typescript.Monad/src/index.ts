const world = 'world';

console.log('hello, world')
export function hello(word: string = world): string {
    return `Hello ${world}! `;
}

hello();
