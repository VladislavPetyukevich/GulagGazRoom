import React, { FunctionComponent, useContext } from 'react';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { AuthContext } from '../../context/AuthContext';

export const Home: FunctionComponent = () => {
  const auth = useContext(AuthContext);

  return (
    <MainContentWrapper>
      <Field>
        <div>Home</div>
        {!auth && (
          <a href="http://localhost:5043/login/twitch?redirectUri=%2FUser%2FGetMe">Login</a>
        )}
      </Field>
    </MainContentWrapper>
  );
};
