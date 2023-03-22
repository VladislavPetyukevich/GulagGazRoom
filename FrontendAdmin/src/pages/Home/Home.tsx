import React, { FunctionComponent, useContext } from 'react';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { REACT_APP_BACKEND_URL } from '../../config';
import { Captions } from '../../constants';
import { AuthContext } from '../../context/AuthContext';

export const Home: FunctionComponent = () => {
  const auth = useContext(AuthContext);

  return (
    <MainContentWrapper>
      <Field>
        <div>{Captions.WelcomeMessage}</div>
        {!auth && (
          <a href={`${REACT_APP_BACKEND_URL}/login/twitch?redirectUri=%2FUser%2FGetMe`}>Login</a>
        )}
      </Field>
    </MainContentWrapper>
  );
};
