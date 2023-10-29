import { UserType, Role } from './user';

export interface Event {
  id: string;
  type: string;
  participantTypes: UserType[];
  roles: Role[];
}
