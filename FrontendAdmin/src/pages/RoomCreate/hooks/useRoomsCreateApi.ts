import { useCallback, useReducer } from 'react';
import { Question } from '../../../types/question';
import { User } from '../../../types/user';

interface RoomState {
  process: {
    loading: boolean;
    error: string | null;
  };
  success: boolean;
}

const initialState: RoomState = {
  process: {
    loading: false,
    error: null,
  },
  success: false,
};

type RoomAction = {
  name: 'startCreating';
} | {
  name: 'setError';
  payload: string;
} | {
  name: 'success';
};

const roomReducer = (state: RoomState, action: RoomAction): RoomState => {
  switch (action.name) {
    case 'startCreating':
      return {
        process: {
          loading: true,
          error: null,
        },
        success: false,
      };
    case 'setError':
      return {
        process: {
          loading: false,
          error: action.payload,
        },
        success: false,
      };
    case 'success':
      return {
        process: {
          loading: false,
          error: null,
        },
        success: true,
      };
    default:
      return state;
  }
};

interface CreateRoomOptions {
  name: string;
  twitchChannel: string;
  questions: Array<Question['id']>;
  users: Array<User['id']>;
}

export const useRoomsCreateApi = () => {
  const [roomState, dispatch] = useReducer(roomReducer, initialState);

  const createRoom = useCallback(async (options: CreateRoomOptions) => {
    const { name, twitchChannel, questions, users } = options;
    dispatch({ name: 'startCreating' });
    try {
      const response = await fetch(
        '/Room/Create',
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ name, twitchChannel, questions, users }),
        }
      );
      if (!response.ok) {
        throw new Error('RoomsApi error');
      }
      dispatch({ name: 'success' });
    } catch (err: any) {
      dispatch({
        name: 'setError',
        payload: err.message || 'Failed to create room',
      });
    }
  }, []);

  return {
    roomState,
    createRoom,
  };
};
