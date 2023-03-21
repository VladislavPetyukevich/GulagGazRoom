import { useCallback, useReducer } from 'react';
import { useNavigate } from 'react-router-dom';
import { REACT_APP_BACKEND_URL } from '../config';
import { pathnames } from '../constants';
import { ApiContract } from '../types/apiContracts';
import { useCommunist } from './useCommunist';

interface ApiMethodState<ResponseData = any> {
  process: {
    loading: boolean;
    error: string | null;
  };
  data: ResponseData | null;
}

const initialState: ApiMethodState = {
  process: {
    loading: false,
    error: null,
  },
  data: null,
};

type ApiMethodAction = {
  name: 'startLoad';
} | {
  name: 'setData';
  payload: any;
} | {
  name: 'setError';
  payload: string;
};

const apiMethodReducer = (state: ApiMethodState, action: ApiMethodAction): ApiMethodState => {
  switch (action.name) {
    case 'startLoad':
      return {
        process: {
          loading: true,
          error: null,
        },
        data: null,
      };
    case 'setError':
      return {
        ...state,
        process: {
          loading: false,
          error: action.payload
        }
      };
    case 'setData':
      return {
        process: {
          loading: false,
          error: null,
        },
        data: action.payload
      };
    default:
      return state;
  }
};
const unauthorizedHttpCode = 401;

const createFetchUrl = (apiContract: ApiContract) => {
  if (apiContract.method === 'GET' && apiContract.urlParams) {
    const params =
      Object.entries(apiContract.urlParams)
      .map(([paramName, paramValue]) => `${encodeURIComponent(paramName)}=${encodeURIComponent(paramValue)}`)
      .join('&');
    return `${REACT_APP_BACKEND_URL}${apiContract.baseUrl}?${params}`;
  }
  return `${REACT_APP_BACKEND_URL}${apiContract.baseUrl}`;
};

const createFetchRequestInit = (apiContract: ApiContract) => {
  if (apiContract.method === 'GET') {
    return undefined;
  }
  return {
    method: apiContract.method,
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(apiContract.body),
  };
};

export const useApiMethod = <ResponseData>(options?: { noParseResponse?: boolean }) => {
  const [apiMethodState, dispatch] = useReducer(apiMethodReducer, initialState);
  const navidate = useNavigate();
  const { deleteCommunist } = useCommunist();

  const fetchData = useCallback(async (apiContract: ApiContract) => {
    dispatch({ name: 'startLoad' });
    try {
      const response = await fetch(
        createFetchUrl(apiContract),
        createFetchRequestInit(apiContract),
      );
      if (response.status === unauthorizedHttpCode) {
        deleteCommunist();
        navidate(pathnames.home);
        return;
      }
      if (!response.ok) {
        throw new Error(`${apiContract.method} ${apiContract.baseUrl} ${response.status}`);
      }
      const responseData =
        options?.noParseResponse ?
        response :
        await response.json();
      dispatch({ name: 'setData', payload: responseData });
    } catch (err: any) {
      dispatch({
        name: 'setError',
        payload: err.message || `Failed to fetch ${apiContract.method} ${apiContract.baseUrl}`,
      });
    }
  }, [options?.noParseResponse, deleteCommunist, navidate]);

  return {
    apiMethodState: apiMethodState as ApiMethodState<ResponseData>,
    fetchData,
  };
};
