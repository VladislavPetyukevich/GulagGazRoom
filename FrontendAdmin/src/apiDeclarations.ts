import { ApiContractGet, ApiContractPatch, ApiContractPost, ApiContractPut } from './types/apiContracts';
import { Question, QuestionState, Tag } from './types/question';
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

export interface CreateQuestionBody {
  value: string;
  tags: Array<{
    tagId: string;
    hexColor: string;
  }>;
}

export interface UpdateQuestionBody extends CreateQuestionBody {
  id: string;
}

export interface GetQuestionsParams extends PaginationUrlParams {
  tags?: Array<Tag['id']>;
}

export const questionsApiDeclaration = {
  getPage: (pagination: GetQuestionsParams): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/questions',
    urlParams: pagination,
  }),
  get: (id: Question['id']): ApiContractGet => ({
    method: 'GET',
    baseUrl: `/questions/${id}`,
  }),
  create: (question: CreateQuestionBody): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/questions',
    body: question,
  }),
  update: (question: UpdateQuestionBody): ApiContractPut => ({
    method: 'PUT',
    baseUrl: `/questions/${question.id}`,
    body: { value: question.value, tags: question.tags },
  }),
};

export interface GetTagsParams extends PaginationUrlParams {
  value: string;
}

export interface CreateTagBody {
  value: string;
}

export const tagsApiDeclaration = {
  getPage: (params: GetTagsParams): ApiContractGet => ({
    method: 'GET',
    baseUrl: '/tags/tag',
    urlParams: params,
  }),
  createTag: (body: CreateTagBody): ApiContractPost => ({
    method: 'POST',
    baseUrl: '/tags/tag',
    body,
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
