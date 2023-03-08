import { Question } from './question';
import { User } from './user';

export interface Room {
  id: string;
  name: string;
  questions: Question[];
  users: User[];
}
