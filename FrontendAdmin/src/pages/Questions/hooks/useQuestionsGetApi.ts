import { useCallback, useReducer } from 'react';

export interface Question {
  id: string;
  value: string;
}

interface QuestionsState {
  process: {
    loading: boolean;
    error: string | null;
  };
  questions: Question[];
}

const initialState: QuestionsState = {
  process: {
    loading: false,
    error: null,
  },
  questions: [],
};

type QuestionsAction = {
  name: 'startLoad';
} | {
  name: 'setQuestions';
  payload: Question[];
} | {
  name: 'setError';
  payload: string;
};

const questionsReducer = (state: QuestionsState, action: QuestionsAction): QuestionsState => {
  switch (action.name) {
    case 'startLoad':
      return {
        process: {
          loading: true,
          error: null,
        },
        questions: [],
      };
    case 'setError':
      return {
        ...state,
        process: {
          loading: false,
          error: action.payload
        }
      };
    case 'setQuestions':
      return {
        process: {
          loading: false,
          error: null,
        },
        questions: action.payload
      };
    default:
      return state;
  }
};

export const useQuestionsGetApi = () => {
  const [questionsState, dispatch] = useReducer(questionsReducer, initialState);

  const loadQuestions = useCallback(async (options: { pageSize: number; pageNumber: number; }) => {
    dispatch({ name: 'startLoad' });
    try {
      const response = await fetch(`/Question/GetPage?PageSize=${+options.pageSize}&PageNumber=${+options.pageNumber}`);
      if (!response.ok) {
        throw new Error('QuestionsApi error');
      }
      const responseJson = await response.json();
      dispatch({ name: 'setQuestions', payload: responseJson });
    } catch (err: any) {
      dispatch({
        name: 'setError',
        payload: err.message || 'Failed to fetch questions',
      });
    }
  }, []);

  return {
    questionsState,
    loadQuestions,
  };
};
