import { useCallback, useReducer } from 'react';
import { Room } from '../../../types/room';

interface RoomsState {
  process: {
    loading: boolean;
    error: string | null;
  };
  rooms: Room[];
}

const initialState: RoomsState = {
  process: {
    loading: false,
    error: null,
  },
  rooms: [],
};

type RoomsAction = {
  name: 'startLoad';
} | {
  name: 'setRooms';
  payload: Room[];
} | {
  name: 'setError';
  payload: string;
};

const roomsReducer = (state: RoomsState, action: RoomsAction): RoomsState => {
  switch (action.name) {
    case 'startLoad':
      return {
        process: {
          loading: true,
          error: null,
        },
        rooms: [],
      };
    case 'setError':
      return {
        ...state,
        process: {
          loading: false,
          error: action.payload
        }
      };
    case 'setRooms':
      return {
        process: {
          loading: false,
          error: null,
        },
        rooms: action.payload
      };
    default:
      return state;
  }
};

export const useRoomsGetApi = () => {
  const [roomsState, dispatch] = useReducer(roomsReducer, initialState);

  const loadRooms = useCallback(async (options: { pageSize: number; pageNumber: number; }) => {
    dispatch({ name: 'startLoad' });
    try {
      const response = await fetch(`/Room/GetPage?PageSize=${+options.pageSize}&PageNumber=${+options.pageNumber}`);
      if (!response.ok) {
        throw new Error('RoomsApi error');
      }
      const responseJson = await response.json();
      dispatch({ name: 'setRooms', payload: responseJson });
    } catch (err: any) {
      dispatch({
        name: 'setError',
        payload: err.message || 'Failed to fetch rooms',
      });
    }
  }, []);

  return {
    roomsState,
    loadRooms,
  };
};
