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

  toString() {
    return `${this.count.like} - 😍\n\n${this.count.dislike} - 😈`;
  }
}
