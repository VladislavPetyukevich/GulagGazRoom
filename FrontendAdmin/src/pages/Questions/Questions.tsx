import React, { FunctionComponent, useCallback, useEffect, useState } from 'react';
import { Field } from '../../components/FieldsBlock/Field';
import { HeaderWithLink } from '../../components/HeaderWithLink/HeaderWithLink';
import { Loader } from '../../components/Loader/Loader';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { Paginator } from '../../components/Paginator/Paginator';
import { pathnames } from '../../constants';
import { Question, useQuestionsGetApi } from './hooks/useQuestionsGetApi';

import './Questions.css';

const pageSize = 10;
const initialPageNumber = 1;

const createQuestionItem = (question: Question) => (
  <li key={question.id}>
    <Field>
      {question.value}
    </Field>
  </li>
);

export const Questions: FunctionComponent = () => {
  const [pageNumber, setPageNumber] = useState(initialPageNumber);
  const {
    questionsState,
    loadQuestions,
  } = useQuestionsGetApi();
  const { process: { loading, error }, questions } = questionsState;

  useEffect(() => {
    loadQuestions({ pageSize, pageNumber });
  }, [loadQuestions, pageNumber]);

  const handleNextPage = useCallback(() => {
    setPageNumber(pageNumber + 1);
  }, [pageNumber]);

  const handlePrevPage = useCallback(() => {
    setPageNumber(pageNumber - 1);
  }, [pageNumber]);

  const renderMainContent = useCallback(() => {
    if (error) {
      return (
        <Field>
          <div>Error: {error}</div>
        </Field>
      );
    }
    if (loading) {
      return (
        Array.from({ length: pageSize + 1 }, () => (
          <Field>
            <Loader />
          </Field>
        ))
      );
    }
    return (
      <>
        <ul className="questions-list">
          {questions.map(createQuestionItem)}
        </ul>
        <Paginator
          pageNumber={pageNumber}
          prevDisabled={pageNumber === initialPageNumber}
          nextDisabled={questions.length !== pageSize}
          onPrevClick={handlePrevPage}
          onNextClick={handleNextPage}
        />
      </>
    );
  }, [error, loading, pageNumber, questions, handleNextPage, handlePrevPage]);

  return (
    <MainContentWrapper>
      <HeaderWithLink
        title="Questions:"
        path={pathnames.questionsCreate}
        linkCaption="+"
        linkFloat="right"
      />
      {renderMainContent()}
    </MainContentWrapper>
  );
};
