export interface Question {
  id: string;
  value: string;
  tags: Tag[];
}

export type QuestionState =
  'Open' |
  'Closed' |
  'Active';

export interface Tag {
  id: string;
  value: string;
}
