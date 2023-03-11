import React, { FunctionComponent } from 'react';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';

export const Home: FunctionComponent = () => {
  return (
    <MainContentWrapper>
      <Field>
        <div>Home</div>
      </Field>
    </MainContentWrapper>
  );
};
