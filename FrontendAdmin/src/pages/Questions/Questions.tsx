import React, { FunctionComponent, useEffect } from 'react';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { Question, useQuestionsApi } from './hooks/useQuestionsApi';

import './Questions.css';

const createQuestionItem = (question: Question) => (
  <li key={question.id}>
    <Field>
      {question.value}
    </Field>
  </li>
);

export const Questions: FunctionComponent = () => {
  const {
    questionsState,
    loadQuestions,
  } = useQuestionsApi();
  const { process: { loading, error }, questions } = questionsState;

  useEffect(() => {
    loadQuestions({ pageSize: 10, pageNumber: 1 });
  }, [loadQuestions]);

  const renderMainContent = () => {
    if (error) {
      return (
        <Field>
          <div>Error: {error}</div>
        </Field>
      );
    }
    if (loading) {
      return (
        <Field>
          <div>Loading...</div>
        </Field>
      );
    }
    return (
      <ul className="questions-list">
        {questions.map(createQuestionItem)}
      </ul>
    );
  };

  return (
    <MainContentWrapper>
      <Field>
        <div>Questions:</div>
      </Field>
      {renderMainContent()}
    </MainContentWrapper>
  );
};
