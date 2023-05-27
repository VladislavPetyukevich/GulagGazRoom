import React, { FormEvent, FunctionComponent, useCallback, useEffect } from 'react';
import toast from 'react-hot-toast';
import { questionsApiDeclaration } from '../../apiDeclarations';
import { Field } from '../../components/FieldsBlock/Field';
import { HeaderWithLink } from '../../components/HeaderWithLink/HeaderWithLink';
import { Loader } from '../../components/Loader/Loader';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { SubmitField } from '../../components/SubmitField/SubmitField';
import { Captions, pathnames, toastSuccessOptions } from '../../constants';
import { useApiMethod } from '../../hooks/useApiMethod';
import { Question } from '../../types/question';

import './QuestionCreate.css';

const valueFieldName = 'qestionText';

export const QuestionCreate: FunctionComponent = () => {
  const {
    apiMethodState: questionState,
    fetchData: fetchCreateQuestion,
  } = useApiMethod<Question['id']>();
  const { process: { loading, error }, data: createdQuestionId } = questionState;

  useEffect(() => {
    if (!createdQuestionId) {
      return;
    }
    toast.success(Captions.QuestionCreatedSuccessfully, toastSuccessOptions);
  }, [createdQuestionId]);

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
    fetchCreateQuestion(questionsApiDeclaration.create({
      value: qestionText,
    }));
  }, [fetchCreateQuestion]);

  const renderStatus = useCallback(() => {
    if (error) {
      return (
        <Field>
          <div>{Captions.Error}: {error}</div>
        </Field>
      );
    }
    if (loading) {
      return (
        <Field>
          <Loader />
        </Field>
      );
    }
    return <></>;
  }, [error, loading]);

  return (
    <MainContentWrapper className="question-create">
      <HeaderWithLink
        title={Captions.CreateQuestion}
        linkVisible={true}
        path={pathnames.questions}
        linkCaption="<"
        linkFloat="left"
      />
      {renderStatus()}
      <form onSubmit={handleSubmit}>
        <Field>
          <label htmlFor="qestionText">{Captions.QuestionText}:</label>
          <input id="qestionText" name={valueFieldName} type="text" />
        </Field>
        <SubmitField caption={Captions.Create} />
      </form>
    </MainContentWrapper>
  );
};
