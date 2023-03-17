import React, { FunctionComponent } from 'react';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';

export const Home: FunctionComponent = () => {
  return (
    <MainContentWrapper>
      <Field>
        <div>Home</div>
        <a href="http://localhost:5043/login/twitch?redirectUri=%2FUser%2FGetMe">Login</a>
      </Field>
    </MainContentWrapper>
  );
};
