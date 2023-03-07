import React, { FormEvent, FunctionComponent, useCallback } from 'react';
import { Field } from '../../components/FieldsBlock/Field';
import { HeaderWithLink } from '../../components/HeaderWithLink/HeaderWithLink';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { SubmitField } from '../../components/SubmitField/SubmitField';
import { pathnames } from '../../constants';
import { useQuestionsCreateApi } from './hooks/useQuestionsCreateApi';

import './QuestionCreate.css';

const valueFieldName = 'qestionText';

export const QuestionCreate: FunctionComponent = () => {
  const { questionState, createQuestion } = useQuestionsCreateApi();
  const { process: { loading, error }, success } = questionState;

  const handleSubmit = useCallback(async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const form = event.target as HTMLFormElement;
    const data = new FormData(form);
    const qestionText = data.get(valueFieldName);
    if (!qestionText) {
      return;
    }
    if (typeof qestionText !== 'string') {
      throw new Error('qestionText field type error');
    }
    createQuestion({ value: qestionText });
  }, [createQuestion]);

  const renderStatus = useCallback(() => {
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
    if (success) {
      return (
        <Field>
          <div>Question created successfully</div>
        </Field>
      );
    }
    return <></>;
  }, [error, loading, success]);

  return (
    <MainContentWrapper className="question-create">
      <HeaderWithLink
        title="Create question"
        path={pathnames.questions}
        linkCaption="<"
        linkFloat="left"
      />
      {renderStatus()}
      <form onSubmit={handleSubmit}>
        <Field>
          <label htmlFor="qestionText">Question text:</label>
          <input id="qestionText" name={valueFieldName} type="text" />
        </Field>
        <SubmitField caption="Create" />
      </form>
    </MainContentWrapper>
  );
};
