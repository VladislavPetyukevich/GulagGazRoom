import React, { FunctionComponent, useCallback, useContext, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { GetQuestionsParams, GetTagsParams, questionsApiDeclaration, tagsApiDeclaration } from '../../apiDeclarations';
import { Field } from '../../components/FieldsBlock/Field';
import { HeaderWithLink } from '../../components/HeaderWithLink/HeaderWithLink';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { Paginator } from '../../components/Paginator/Paginator';
import { Captions, pathnames } from '../../constants';
import { AuthContext } from '../../context/AuthContext';
import { useApiMethod } from '../../hooks/useApiMethod';
import { Question } from '../../types/question';
import { Tag } from '../../types/tag';
import { checkAdmin } from '../../utils/checkAdmin';
import { ProcessWrapper } from '../../components/ProcessWrapper/ProcessWrapper';
import { QuestionTagsView } from '../../components/QuestionTagsView/QuestionTagsView';
import { QuestionTags } from '../QuestionCreate/components/QuestionTags/QuestionTags';

import './Questions.css';

const pageSize = 10;
const initialPageNumber = 1;

export const Questions: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const admin = checkAdmin(auth);
  const [pageNumber, setPageNumber] = useState(initialPageNumber);
  const [tagsSearchValue, setTagsSearchValue] = useState('');
  const [selectedTags, setSelectedTags] = useState<Tag[]>([]);

  const { apiMethodState: questionsState, fetchData: fetchQuestios } = useApiMethod<Question[], GetQuestionsParams>(questionsApiDeclaration.getPage);
  const { process: { loading, error }, data: questions } = questionsState;

  const {
    apiMethodState: tagsState,
    fetchData: fetchTags,
  } = useApiMethod<Tag[], GetTagsParams>(tagsApiDeclaration.getPage);
  const { process: { loading: tagsLoading, error: tagsError }, data: tags } = tagsState;

  const questionsSafe = questions || [];

  useEffect(() => {
    fetchQuestios({
      PageNumber: pageNumber,
      PageSize: pageSize,
      tags: selectedTags.map(tag => tag.id),
    });
  }, [pageNumber, selectedTags, fetchQuestios]);

  useEffect(() => {
    fetchTags({
      PageNumber: initialPageNumber,
      PageSize: pageSize,
      value: tagsSearchValue,
    });
  }, [tagsSearchValue, fetchTags]);

  const handleSelect = (tag: Tag) => {
    setSelectedTags([...selectedTags, tag]);
  };

  const handleUnselect = (tag: Tag) => {
    const newSelectedTags = selectedTags.filter(tg => tg.id !== tag.id);
    setSelectedTags(newSelectedTags);
  };

  const handleTagSearch = (value: string) => {
    setTagsSearchValue(value);
  };

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
          tags={question.tags}
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
      <Field>
        {tagsError ? (
          <div>{Captions.Error}: {tagsError}</div>
        ) : (
          <QuestionTags
            placeHolder={Captions.SearchByTags}
            loading={tagsLoading}
            tags={tags || []}
            selectedTags={selectedTags}
            onSelect={handleSelect}
            onUnselect={handleUnselect}
            onSearch={handleTagSearch}
          />
        )}
      </Field>
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
