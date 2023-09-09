import { Question } from './question';
import { ParticipantType, User } from './user';
import { Tag } from './tag';

export interface Room {
  id: string;
  name: string;
  twitchChannel: string;
  questions: Question[];
  users: User[];
  tags: Tag[];
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

export interface RoomParticipant {
  id: string;
  roomId: Room['id'];
  userId: User['id'];
  userType: ParticipantType;
}