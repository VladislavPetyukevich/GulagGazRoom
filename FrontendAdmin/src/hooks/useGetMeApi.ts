import { useCallback, useReducer } from 'react';
import { User } from '../types/user';

interface GetMeState {
  process: {
    loading: boolean;
    error: string | null;
  };
  user: User | null;
}

const initialState: GetMeState = {
  process: {
    loading: false,
    error: null,
  },
  user: null,
};

type GetMeAction = {
  name: 'startLoad';
} | {
  name: 'setUser';
  payload: User;
} | {
  name: 'setError';
  payload: string;
};

const getMeReducer = (state: GetMeState, action: GetMeAction): GetMeState => {
  switch (action.name) {
    case 'startLoad':
      return {
        process: {
          loading: true,
          error: null,
        },
        user: null,
      };
    case 'setError':
      return {
        ...state,
        process: {
          loading: false,
          error: action.payload
        }
      };
    case 'setUser':
      return {
        process: {
          loading: false,
          error: null,
        },
        user: action.payload
      };
    default:
      return state;
  }
};

export const useGetMeApi = () => {
  const [getMeState, dispatch] = useReducer(getMeReducer, initialState);

  const loadMe = useCallback(async () => {
    dispatch({ name: 'startLoad' });
    try {
      const response = await fetch('/User/GetMe');
      if (!response.ok) {
        throw new Error('UserApi error');
      }
      const responseJson = await response.json();
      dispatch({ name: 'setUser', payload: responseJson });
    } catch (err: any) {
      dispatch({
        name: 'setError',
        payload: err.message || 'Failed to get me',
      });
    }
  }, []);

  return {
    getMeState,
    loadMe,
  };
};
