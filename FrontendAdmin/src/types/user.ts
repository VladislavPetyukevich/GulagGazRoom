export interface User {
  id: string;
  nickname: string;
  twitchIdentity: string;
  avatar?: string;
  roles: string[];
}

export type UserType = 'Viewer' | 'Expert' | 'Examinee';
