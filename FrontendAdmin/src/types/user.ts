export interface User {
  id: string;
  nickname: string;
  twitchIdentity: string;
  avatar?: string;
  roles: string[];
}

export interface UserAuth {
  identity: string;
  nickname: string;
  twitchIdentity: string;
  avatar?: string;
  roles: string[];
}
