import { ApiContractGet, ApiContractPatch, ApiContractPost, ApiContractPut } from './types/apiContracts';
import { Question, QuestionState } from './types/question';
import { Reaction } from './types/reaction';
import { Room, RoomReview } from './types/room';
import { User } from './types/user';

export interface PaginationUrlParams {
  PageSize: number;
  PageNumber: number;
}

export interface CreateRoomBody {
  name: string;
  twitchChannel: string;
  questions: Array<Question['id']>;
  experts: Array<User['id']>;
  examinees: Array<User['id']>;
}

export interface SendEventBody {
  roomId: Room['id'];
  type: string;
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
  sendEvent: (body: SendEventBody): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/rooms/event',
    body,
  }),
  close: (id: Room['id']): ApiContractPatch => ({
    method: 'PATCH',
    baseUrl: `/rooms/${id}/close`,
    body: {},
  }),
  startReview: (id: Room['id']): ApiContractPatch => ({
    method: 'PATCH',
    baseUrl: `/rooms/${id}/startReview`,
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
    baseUrl: `/questions/${question.id}`,
    body: { value: question.value },
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
    baseUrl: '/reactions',
    urlParams: pagination,
  }),
};

export interface SendReactionBody {
  reactionId: Reaction['id'];
  roomId: Room['id'];
  payload: string;
}

export const roomReactionApiDeclaration = {
  send: (body: SendReactionBody): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/room-reactions',
    body,
  }),
};

export interface GetParticipantParams {
  userId: Reaction['id'];
  roomId: Room['id'];
}

export interface ChangeParticipantStatusBody {
  userId: Reaction['id'];
  roomId: Room['id'];
  userType: string;
}

export const roomParticipantApiDeclaration = {
  getRoomParticipant: (params: GetParticipantParams): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/room-participants',
    urlParams: params,
  }),
  changeParticipantStatus: (body: ChangeParticipantStatusBody): ApiContractPatch => ({
    method: 'PATCH',
    baseUrl: '/room-participants',
    body,
  }),
};

export interface AddRoomReviewBody {
  roomId: Room['id'],
  review: string;
}

export interface GetRoomReviewsParams {
  'Page.PageSize': number;
  'Page.PageNumber': number;
  'Filter.RoomId': Room['id'],
}

export interface UpdateRoomReviewsParams {
  id: RoomReview['id'];
  review: RoomReview['review'];
  state: RoomReview['state'];
}

export const roomReviewApiDeclaration = {
  addReview: (body: AddRoomReviewBody): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/room-reviews',
    body,
  }),
  getPage: (params: GetRoomReviewsParams): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/room-reviews',
    urlParams: params,
  }),
  update: (params: UpdateRoomReviewsParams): ApiContractPut => ({
    method: 'PUT',
    baseUrl: `/room-reviews/${params.id}`,
    body: { review: params.review, state: params.state },
  }),
};

export const eventApiDeclaration = {
  get: (params: PaginationUrlParams): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/event',
    urlParams: params,
  }),
};
