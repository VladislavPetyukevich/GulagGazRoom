import React from 'react';
import { UserAuth } from '../types/user';

export const AuthContext = React.createContext<UserAuth | null>(null);
