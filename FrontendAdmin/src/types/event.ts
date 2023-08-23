import { ParticipantType, Role } from './user';

export interface Event {
  id: string;
  type: string;
  participantTypes: ParticipantType[];
  roles: Role[];
}
