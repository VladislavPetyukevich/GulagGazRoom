import { Question } from './question';
import { User } from './user';

export interface Room {
  id: string;
  name: string;
  twitchChannel: string;
  questions: Question[];
  users: User[];
  roomStatus: 'New' | 'Active' | 'Review' | 'Close';
}

export interface RoomState {
  id: Room['id'];
  name: Room['name'];
  likeCount: number;
  dislikeCount: number;
  activeQuestion: Question;
}

export interface RoomReview {
  id: string;
  user: User;
  roomId: Room['id'],
  review: string;
  state: 'Open' | 'Closed';
}
