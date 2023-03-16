import React, { ChangeEvent, FunctionComponent, useCallback, useEffect, useState } from 'react';
import { Field } from '../../components/FieldsBlock/Field';
import { HeaderWithLink } from '../../components/HeaderWithLink/HeaderWithLink';
import { Loader } from '../../components/Loader/Loader';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { Paginator } from '../../components/Paginator/Paginator';
import { pathnames } from '../../constants';
import { Question } from '../../types/question';
import { useQuestionsGetApi } from './hooks/useQuestionsGetApi';
import { useQuestionsUpdateApi } from './hooks/useQuestionsUpdateApi';

import './Questions.css';

const pageSize = 10;
const initialPageNumber = 1;

export const Questions: FunctionComponent = () => {
  const [pageNumber, setPageNumber] = useState(initialPageNumber);
  const {
    questionsState,
    loadQuestions,
  } = useQuestionsGetApi();
  const { process: { loading, error }, questions } = questionsState;
  const {
    questionState: updatingQuestionState,
    updateQuestion,
  } = useQuestionsUpdateApi();
  const {
    process: { loading: updatingLoading, error: updatingError },
    success: updatingSuccess,
  } = updatingQuestionState;
  const [editingQuestion, setEditingQuestion] = useState<Question | null>(null);

  useEffect(() => {
    loadQuestions({ pageSize, pageNumber });
  }, [loadQuestions, pageNumber]);

  useEffect(() => {
    if (updatingSuccess) {
      loadQuestions({ pageSize, pageNumber });
    }
  }, [updatingSuccess, pageNumber, loadQuestions]);

  const handleNextPage = useCallback(() => {
    setPageNumber(pageNumber + 1);
  }, [pageNumber]);

  const handlePrevPage = useCallback(() => {
    setPageNumber(pageNumber - 1);
  }, [pageNumber]);

  const handleQuestionEdit = useCallback((question: Question) => () => {
    setEditingQuestion(question);
  }, []);

  const handleEditingQuestionValueChange = useCallback((event: ChangeEvent<HTMLInputElement>) => {
    if (!editingQuestion) {
      console.error('handleEditingQuestionValueChange without editingQuestion');
      return;
    }
    setEditingQuestion({
      ...editingQuestion,
      value: event.target.value,
    });
  }, [editingQuestion]);

  const handleEditingQuestionSubmit = useCallback(() => {
    if (!editingQuestion) {
      console.error('handleEditingQuestionSubmit without editingQuestion');
      return;
    }
    updateQuestion(editingQuestion);
    setEditingQuestion(null);
  }, [editingQuestion, updateQuestion]);

  const createQuestionItem = useCallback((question: Question) => (
    <li key={question.id}>
      {question.id === editingQuestion?.id ? (
        <Field className="question-item">
          <input
            type="text"
            value={editingQuestion.value}
            onChange={handleEditingQuestionValueChange}
          />
          <button
            className="question-edit-button"
            onClick={handleEditingQuestionSubmit}
          >
            ✔️
          </button>
        </Field>
      ) : (
        <Field className="question-item">
          <span>{question.value}</span>
          <button
            className="question-edit-button"
            onClick={handleQuestionEdit(question)}
          >
            🖊️
          </button>
        </Field>
      )}

    </li>
  ), [
    editingQuestion,
    handleQuestionEdit,
    handleEditingQuestionValueChange,
    handleEditingQuestionSubmit,
  ]);

  const renderMainContent = useCallback(() => {
    if (error) {
      return (
        <Field>
          <div>Error: {error}</div>
        </Field>
      );
    }
    if (updatingError) {
      return (
        <Field>
          <div>Updating error: {updatingError}</div>
        </Field>
      );
    }
    if (loading || updatingLoading) {
      return (
        Array.from({ length: pageSize + 1 }, (_, index) => (
          <Field key={index}>
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
  }, [
    error,
    loading,
    updatingError,
    updatingLoading,
    pageNumber,
    questions,
    handleNextPage,
    handlePrevPage,
    createQuestionItem,
  ]);

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
