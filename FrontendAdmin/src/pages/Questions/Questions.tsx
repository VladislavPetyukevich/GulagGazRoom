import React, { FunctionComponent } from 'react';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';

import './Questions.css';

const createQuestionItem = (name: string) => (
  <li key={name}>
    <Field>
      {name}
    </Field>
  </li>
);

export const Questions: FunctionComponent = () => {
  return (
    <MainContentWrapper>
      <Field>
        <div>Questions</div>
      </Field>
      <ul className="questions-list">
        {Array.from({ length: 11 }, (_, index) => `Question ${index}`).map(createQuestionItem)}
      </ul>
    </MainContentWrapper>
  );
};
