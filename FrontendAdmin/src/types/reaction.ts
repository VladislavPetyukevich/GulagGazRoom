export interface Reaction {
  id: string;
  type: {
    eventType: string,
    name: string,
    value: number,
  };
}
