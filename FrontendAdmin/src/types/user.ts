export interface User {
  id: string;
  nickname: string;
  twitchIdentity: string;
  avatar?: string;
  roles: Role[];
}

export interface UserAuth {
  identity: string;
  nickname: string;
  twitchIdentity: string;
  avatar?: string;
  roles: string[];
}

export type ParticipantType = 'Viewer' | 'Expert' | 'Examinee' | null;

export type Role = 'User' | 'Admin';
