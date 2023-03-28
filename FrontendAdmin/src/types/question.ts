export interface Question {
  id: string;
  value: string;
}

export type QuestionState =
  'Open' |
  'Closed' |
  'Active';
