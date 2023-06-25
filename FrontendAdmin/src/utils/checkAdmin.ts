import { UserAuth } from '../types/user';

export const checkAdmin = (user: UserAuth | null) =>
  !!user && user.roles.includes('Admin');
