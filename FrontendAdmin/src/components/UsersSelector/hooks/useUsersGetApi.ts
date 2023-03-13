import { useCallback, useReducer } from 'react';
import { User } from '../../../types/user';

interface UsersState {
  process: {
    loading: boolean;
    error: string | null;
  };
  users: User[];
}

const initialState: UsersState = {
  process: {
    loading: false,
    error: null,
  },
  users: [],
};

type UsersAction = {
  name: 'startLoad';
} | {
  name: 'setUsers';
  payload: User[];
} | {
  name: 'setError';
  payload: string;
};

const usersReducer = (state: UsersState, action: UsersAction): UsersState => {
  switch (action.name) {
    case 'startLoad':
      return {
        process: {
          loading: true,
          error: null,
        },
        users: [],
      };
    case 'setError':
      return {
        ...state,
        process: {
          loading: false,
          error: action.payload
        }
      };
    case 'setUsers':
      return {
        process: {
          loading: false,
          error: null,
        },
        users: action.payload
      };
    default:
      return state;
  }
};

export const useUsersGetApi = () => {
  const [usersState, dispatch] = useReducer(usersReducer, initialState);

  const loadUsers = useCallback(async (options: { pageSize: number; pageNumber: number; }) => {
    dispatch({ name: 'startLoad' });
    try {
      const response = await fetch(`/User/GetPage?PageSize=${+options.pageSize}&PageNumber=${+options.pageNumber}`);
      if (!response.ok) {
        throw new Error('UsersApi error');
      }
      const responseJson = await response.json();
      dispatch({ name: 'setUsers', payload: responseJson });
    } catch (err: any) {
      dispatch({
        name: 'setError',
        payload: err.message || 'Failed to fetch users',
      });
    }
  }, []);

  return {
    usersState,
    loadUsers,
  };
};
