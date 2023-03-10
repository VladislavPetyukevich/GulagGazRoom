import React, { ChangeEvent, FunctionComponent, useCallback, useEffect, useState } from 'react';
import { Loader } from '../../components/Loader/Loader';
import { Paginator } from '../../components/Paginator/Paginator';
import { useQuestionsGetApi } from '../../pages/Questions/hooks/useQuestionsGetApi';
import { Question } from '../../types/question';

import './QuestionsSelector.css';

const pageSize = 10;
const initialPageNumber = 1;

interface QuestionsSelectorProps {
  selected: Question[];
  onSelect: (question: Question) => void;
  onUnselect: (question: Question) => void;
}

export const QuestionsSelector: FunctionComponent<QuestionsSelectorProps> = ({
  selected,
  onSelect,
  onUnselect,
}) => {
  const [pageNumber, setPageNumber] = useState(initialPageNumber);
  const {
    questionsState,
    loadQuestions,
  } = useQuestionsGetApi();
  const { process: { loading, error }, questions } = questionsState;

  useEffect(() => {
    loadQuestions({ pageSize, pageNumber });
  }, [loadQuestions, pageNumber]);

  const handleCheckboxChange = useCallback((event: ChangeEvent<HTMLInputElement>) => {
    const { value, checked } = event.target;
    const questionItem = questions.find(
      question => question.id === value
    );
    if (!questionItem) {
      throw new Error('Question item not found in state');
    }
    if (checked) {
      onSelect(questionItem);
    } else {
      onUnselect(questionItem);
    }
  }, [questions, onSelect, onUnselect]);

  const createQuestionItem = useCallback((question: Question) => (
    <li key={question.id}>
      <label htmlFor={`input-${question.id}`}>{question.value}</label>
      <input
        id={`input-${question.id}`}
        type="checkbox"
        value={question.id}
        checked={selected.some(que => que.id === question.id)}
        onChange={handleCheckboxChange}
      />
    </li>
  ), [selected, handleCheckboxChange]);

  const handleNextPage = useCallback(() => {
    setPageNumber(pageNumber + 1);
  }, [pageNumber]);

  const handlePrevPage = useCallback(() => {
    setPageNumber(pageNumber - 1);
  }, [pageNumber]);

  if (error) {
    return (
      <div>Error: {error}</div>
    );
  }
  if (loading) {
    return (
      <>
        {Array.from({ length: pageSize + 1 }, (_, index) => (
          <div key={index}>
            <Loader />
          </div>
        ))}
      </>
    );
  }
  return (
    <>
      <ul className="questions-selector">
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
};
