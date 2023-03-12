import { useCallback, useReducer } from 'react';
import { Room } from '../../../types/room';

interface RoomState {
  process: {
    loading: boolean;
    error: string | null;
  };
  room: Room | null;
}

const initialState: RoomState = {
  process: {
    loading: false,
    error: null,
  },
  room: null,
};

type RoomAction = {
  name: 'startLoad';
} | {
  name: 'setRoom';
  payload: Room;
} | {
  name: 'setError';
  payload: string;
};

const roomReducer = (state: RoomState, action: RoomAction): RoomState => {
  switch (action.name) {
    case 'startLoad':
      return {
        process: {
          loading: true,
          error: null,
        },
        room: null,
      };
    case 'setError':
      return {
        ...state,
        process: {
          loading: false,
          error: action.payload
        }
      };
    case 'setRoom':
      return {
        process: {
          loading: false,
          error: null,
        },
        room: action.payload
      };
    default:
      return state;
  }
};

export const useRoomGetApi = () => {
  const [roomState, dispatch] = useReducer(roomReducer, initialState);

  const loadRoom = useCallback(async (options: { roomId: Room['id']; }) => {
    dispatch({ name: 'startLoad' });
    try {
      const response = await fetch(`/Room/GetById?id=${options.roomId}`);
      if (!response.ok) {
        throw new Error('RoomApi error');
      }
      const responseJson = await response.json();
      dispatch({ name: 'setRoom', payload: responseJson });
    } catch (err: any) {
      dispatch({
        name: 'setError',
        payload: err.message || 'Failed to fetch room',
      });
    }
  }, []);

  return {
    roomState,
    loadRoom,
  };
};
