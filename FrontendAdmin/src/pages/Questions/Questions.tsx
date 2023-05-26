import React, { ChangeEvent, FunctionComponent, useCallback, useContext, useEffect, useState } from 'react';
import { PaginationUrlParams, questionsApiDeclaration } from '../../apiDeclarations';
import { Field } from '../../components/FieldsBlock/Field';
import { HeaderWithLink } from '../../components/HeaderWithLink/HeaderWithLink';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { Paginator } from '../../components/Paginator/Paginator';
import { Captions, pathnames } from '../../constants';
import { AuthContext } from '../../context/AuthContext';
import { useApiMethod } from '../../hooks/useApiMethod';
import { Question } from '../../types/question';
import { checkAdmin } from '../../utils/checkAdmin';
import { ProcessWrapper } from '../../components/ProcessWrapper/ProcessWrapper';

import './Questions.css';

const pageSize = 10;
const initialPageNumber = 1;

export const Questions: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const admin = checkAdmin(auth);
  const [pageNumber, setPageNumber] = useState(initialPageNumber);
  const { apiMethodState: questionsState, fetchData: fetchQuestios } = useApiMethod<Question[], PaginationUrlParams>(questionsApiDeclaration.getPage);
  const { process: { loading, error }, data: questions } = questionsState;
  const { apiMethodState: updatingQuestionState, fetchData: fetchUpdateQuestion } = useApiMethod<Question['id'], Question>(questionsApiDeclaration.update);
  const {
    process: { loading: updatingLoading, error: updatingError },
    data: updatedQuestionId,
  } = updatingQuestionState;
  const [editingQuestion, setEditingQuestion] = useState<Question | null>(null);
  const questionsSafe = questions || [];

  useEffect(() => {
    fetchQuestios({
      PageNumber: pageNumber,
      PageSize: pageSize,
    });
  }, [fetchQuestios, pageNumber]);

  useEffect(() => {
    if (updatedQuestionId) {
      fetchQuestios({
        PageNumber: pageNumber,
        PageSize: pageSize,
      });
    }
  }, [updatedQuestionId, pageNumber, fetchQuestios]);

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
    fetchUpdateQuestion({
      id: editingQuestion.id,
      value: editingQuestion.value,
    });
    setEditingQuestion(null);
  }, [editingQuestion, fetchUpdateQuestion]);

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

  return (
    <MainContentWrapper>
      <HeaderWithLink
        title={`${Captions.QuestionsPageName}:`}
        linkVisible={admin}
        path={pathnames.questionsCreate}
        linkCaption="+"
        linkFloat="right"
      />
      <ProcessWrapper
        loading={loading || updatingLoading}
        error={error || updatingError}
        loaders={Array.from({ length: pageSize }, () => ({}))}
      >
        <>
          <ul className="questions-list">
            {questionsSafe.map(createQuestionItem)}
          </ul>
          <Paginator
            pageNumber={pageNumber}
            prevDisabled={pageNumber === initialPageNumber}
            nextDisabled={questionsSafe.length !== pageSize}
            onPrevClick={handlePrevPage}
            onNextClick={handleNextPage}
          />
        </>
      </ProcessWrapper>
    </MainContentWrapper>
  );
};
