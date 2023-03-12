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
  success: boolean;
}

const initialState: QuestionsState = {
  process: {
    loading: false,
    error: null,
  },
  success: false,
};

type QuestionsAction = {
  name: 'startCreating';
} | {
  name: 'setError';
  payload: string;
} | {
  name: 'success';
};

const questionsReducer = (state: QuestionsState, action: QuestionsAction): QuestionsState => {
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

export const useQuestionsCreateApi = () => {
  const [questionState, dispatch] = useReducer(questionsReducer, initialState);

  const createQuestion = useCallback(async (options: { value: string; }) => {
    dispatch({ name: 'startCreating' });
    try {
      const response = await fetch(
        '/Question/Create',
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ value: options.value }),
        }
      );
      if (!response.ok) {
        throw new Error('QuestionsApi error');
      }
      dispatch({ name: 'success' });
    } catch (err: any) {
      dispatch({
        name: 'setError',
        payload: err.message || 'Failed to fetch questions',
      });
    }
  }, []);

  return {
    questionState,
    createQuestion,
  };
};
