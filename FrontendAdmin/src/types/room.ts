import { Question } from './question';
import { User } from './user';

export interface Room {
  id: string;
  name: string;
  twitchChannel: string;
  questions: Question[];
  users: User[];
}

export interface RoomState {
  id: Room['id'];
  name: Room['name'];
  likeCount: number;
  dislikeCount: number;
  activeQuestion: Question;
}
