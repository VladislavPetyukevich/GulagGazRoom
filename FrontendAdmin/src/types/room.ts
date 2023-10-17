import { Question } from './question';
import { Tag } from './tag';
import { User, UserType } from './user';

export type RoomStatus = 'New' | 'Active' | 'Review' | 'Close';

export interface Room {
  id: string;
  name: string;
  twitchChannel: string;
  questions: Question[];
  users: User[];
  tags: Tag[];
  roomStatus: RoomStatus;
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

export interface RoomParticipant {
  id: string;
  roomId: Room['id'];
  userId: User['id'];
  userType: UserType;
}
