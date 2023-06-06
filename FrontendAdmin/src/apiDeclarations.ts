import { ApiContractGet, ApiContractPatch, ApiContractPost, ApiContractPut } from './types/apiContracts';
import { Question, QuestionState } from './types/question';
import { Reaction } from './types/reaction';
import { Room } from './types/room';
import { User } from './types/user';

export interface PaginationUrlParams {
  PageSize: number;
  PageNumber: number;
}

export interface CreateRoomBody {
  name: string;
  twitchChannel: string;
  questions: Array<Question['id']>;
  users: Array<User['id']>;
}

export interface SendGasBody {
  roomId: Room['id'];
  type: 'GasOn' | 'GasOff';
}

export interface SendCodeEditorBody {
  roomId: Room['id'];
  type: 'EnableCodeEditor' | 'DisableCodeEditor';
}

export const roomsApiDeclaration = {
  getPage: (pagination: PaginationUrlParams): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/rooms',
    urlParams: pagination,
  }),
  getById: (id: Room['id']): ApiContractGet => ({
    method: 'GET',
    baseUrl: `/rooms/${id}`,
  }),
  getState: (id: Room['id']): ApiContractGet => ({
    method: 'GET',
    baseUrl: `/rooms/${id}/state`,
  }),
  analyticsSummary: (id: Room['id']): ApiContractGet => ({
    method: 'GET',
    baseUrl: `/rooms/${id}/analytics/summary`,
  }),
  create: (body: CreateRoomBody): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/rooms',
    body,
  }),
  sendGasEvent: (body: SendGasBody): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/rooms/event/gas',
    body,
  }),
  sendCodeEditorEvent: (body: SendCodeEditorBody): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/rooms/event/codeEditor',
    body,
  }),
  close: (id: Room['id']): ApiContractPatch => ({
    method: 'PATCH',
    baseUrl: `/rooms/${id}/close`,
    body: {},
  }),
};

export interface ChangeActiveQuestionBody {
  roomId: Room['id'];
  questionId: Question['id'];
}

export interface GetRoomQuestionsBody {
  RoomId: Room['id'];
  State: QuestionState;
}

export const roomQuestionApiDeclaration = {
  changeActiveQuestion: (body: ChangeActiveQuestionBody): ApiContractPut => ({
    method: 'PUT',
    baseUrl: '/room-questions/active-question',
    body,
  }),
  getRoomQuestions: (params: GetRoomQuestionsBody): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/room-questions',
    urlParams: params,
  }),
};

export const questionsApiDeclaration = {
  getPage: (pagination: PaginationUrlParams): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/questions',
    urlParams: pagination,
  }),
  create: (question: Pick<Question, 'value'>): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/questions',
    body: question,
  }),
  update: (question: Question): ApiContractPut => ({
    method: 'PUT',
    baseUrl: '/questions',
    body: question,
  }),
};

export const usersApiDeclaration = {
  getPage: (pagination: PaginationUrlParams): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/users',
    urlParams: pagination,
  }),
};

export const reactionsApiDeclaration = {
  getPage: (pagination: PaginationUrlParams): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/Reaction/GetPage',
    urlParams: pagination,
  }),
};

export interface SendReactionBody {
  reactionId: Reaction['id'];
  roomId: Room['id'];
}

export const roomReactionApiDeclaration = {
  send: (body: SendReactionBody): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/room-reactions',
    body,
  }),
};

export interface ChangeParticipantStatusBody {
  userId: Reaction['id'];
  roomId: Room['id'];
  userType: string;
}

export const roomParticipantApiDeclaration = {
  changeParticipantStatus: (body: ChangeParticipantStatusBody): ApiContractPatch => ({
    method: 'PATCH',
    baseUrl: '/ChangeParticipantStatus',
    body,
  }),
};
