export class Stats {
  count: {
    like: number;
    dislike: number;
  };

  constructor() {
    this.count = {
      like: 0,
      dislike: 0,
    };
  }

  increaseCount(stat: keyof Stats['count']) {
    this.count[stat]++;
  }

  setCount(stat: keyof Stats['count'], value: number) {
    this.count[stat] = value;
  }

  toString() {
    return `${this.count.like} - 😍\n${this.count.dislike} - 😈`;
  }
}
