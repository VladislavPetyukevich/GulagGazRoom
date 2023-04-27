import { ApiContractGet, ApiContractPatch, ApiContractPost, ApiContractPut } from './types/apiContracts';
import { Question, QuestionState } from './types/question';
import { Reaction } from './types/reaction';
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
  analyticsSummary: (id: Room['id']): ApiContractGet => ({
    method: 'GET',
    baseUrl: `/Room/${id}/analytics/summary`,
  }),
  create: (body: CreateRoomBody): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/Room/Create',
    body,
  }),
  sendGasEvent: (body: { roomId: Room['id'], type: 'GasOn' | 'GasOff'; }): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/Room/SendGasEvent',
    body,
  }),
  close: (id: Room['id']): ApiContractPatch => ({
    method: 'PATCH',
    baseUrl: `/Room/Close?roomId=${id}`,
    body: {},
  }),
};

export const roomQuestionApiDeclaration = {
  changeActiveQuestion: (body: { roomId: Room['id'], questionId: Question['id']; }): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/RoomQuestion/ChangeActiveQuestion',
    body,
  }),
  getRoomQuestions: (params: { RoomId: Room['id']; State: QuestionState }): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/RoomQuestion/GetRoomQuestions',
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

export const roomReactionApiDeclaration = {
  send: (body: { reactionId: Reaction['id'], roomId: Room['id'] }): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/RoomReaction',
    body,
  }),
};

export const roomParticipantApiDeclaration = {
  changeParticipantStatus: (body: {
    userId: Reaction['id'];
    roomId: Room['id'];
    userType: string;
  }): ApiContractPatch => ({
    method: 'PATCH',
    baseUrl: '/ChangeParticipantStatus',
    body,
  }),
};
