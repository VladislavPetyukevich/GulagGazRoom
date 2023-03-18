import { ApiContractGet, ApiContractPost, ApiContractPut } from './types/apiContracts';
import { Question } from './types/question';
import { Room } from './types/room';
import { User } from './types/user';

interface PaginationUrlParams {
  PageSize: number;
  PageNumber: number;
}

interface CreateRoomBody {
  name: string;
  twitchChannel: string;
  questions: Array<Question['id']>;
  users: Array<User['id']>;
}

export const roomsApiDeclaration = {
  getPage: (pagination: PaginationUrlParams): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/Room/GetPage',
    urlParams: pagination,
  }),
  getById: (id: Room['id']): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/Room/GetById',
    urlParams: { id },
  }),
  create: (body: CreateRoomBody): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/Room/Create',
    body,
  }),
};

export const questionsApiDeclaration = {
  getPage: (pagination: PaginationUrlParams): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/Question/GetPage',
    urlParams: pagination,
  }),
  create: (question: Pick<Question, 'value'>): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/Question/Create',
    body: question,
  }),
  update: (question: Question): ApiContractPut => ({
    method: 'PUT',
    baseUrl: '/Question/Update',
    body: question,
  }),
};

export const usersApiDeclaration = {
  getPage: (pagination: PaginationUrlParams): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/User/GetPage',
    urlParams: pagination,
  }),
};
