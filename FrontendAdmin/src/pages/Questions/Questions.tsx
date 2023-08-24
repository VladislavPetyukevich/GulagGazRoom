import React, { FunctionComponent, useCallback, useContext, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
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
import { QuestionTagsView } from '../../components/QuestionTagsView/QuestionTagsView';

import './Questions.css';

const pageSize = 10;
const initialPageNumber = 1;

const fakeTags = Array.from({ length: 7 }, (_, index) => ({ id: `tag-${index}`, value: `Tag ${index}` }));

export const Questions: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const admin = checkAdmin(auth);
  const [pageNumber, setPageNumber] = useState(initialPageNumber);
  const { apiMethodState: questionsState, fetchData: fetchQuestios } = useApiMethod<Question[], PaginationUrlParams>(questionsApiDeclaration.getPage);
  const { process: { loading, error }, data: questions } = questionsState;
  const questionsSafe = questions || [];

  useEffect(() => {
    fetchQuestios({
      PageNumber: pageNumber,
      PageSize: pageSize,
    });
  }, [fetchQuestios, pageNumber]);

  const handleNextPage = useCallback(() => {
    setPageNumber(pageNumber + 1);
  }, [pageNumber]);

  const handlePrevPage = useCallback(() => {
    setPageNumber(pageNumber - 1);
  }, [pageNumber]);

  const createQuestionItem = useCallback((question: Question) => (
    <li key={question.id}>
      <Field className="question-item">
        <span>{question.value}</span>
        <Link to={pathnames.questionsEdit.replace(':id', question.id)}>
          <button
            className="question-edit-button"
          >
            🖊️
          </button>
        </Link>
        <QuestionTagsView
          placeHolder={Captions.NoTags}
          tags={fakeTags}
        />
      </Field>

    </li>
  ), []);

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
        loading={loading}
        error={error}
        loaders={Array.from({ length: pageSize }, () => ({ height: '4.75rem' }))}
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
