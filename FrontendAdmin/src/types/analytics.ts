import { Question } from './question';
import { User } from './user';

export interface AnalyticsQuestionsUserReactionSummary {
  id: string;
  type: string;
  count: number;
}

export interface AnalyticsQuestionsUserReaction {
  id: string;
  type: string;
}

export interface AnalyticsQuestionsUser extends User {
  participantType: string;
  reactions: AnalyticsQuestionsUserReaction[];
  reactionsSummary: AnalyticsQuestionsUserReactionSummary[];
}

export interface AnalyticsQuestions extends Question {
  status: string;
  users: AnalyticsQuestionsUser[];
}

interface AnalyticsReaction {
  id: string;
  type: string;
  count: number;
}

export interface AnalyticsSummary {
  questions: AnalyticsQuestions[];
  reactions: AnalyticsReaction[];
}
