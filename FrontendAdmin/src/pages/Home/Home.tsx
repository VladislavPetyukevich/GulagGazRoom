import React, { FunctionComponent, useContext } from 'react';
import { Link } from 'react-router-dom';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { REACT_APP_BACKEND_URL } from '../../config';
import { Captions, pathnames } from '../../constants';
import { AuthContext } from '../../context/AuthContext';

import './Home.css';

export const Home: FunctionComponent = () => {
  const auth = useContext(AuthContext);

  return (
    <MainContentWrapper className="home">
      <Field>
        {auth ? (
          <div>{Captions.WelcomeMessage}, {auth.nickname}</div>
        ) : (
          <>
            <div>{Captions.WhoAreYou}</div>
            <a href={`${REACT_APP_BACKEND_URL}/login/twitch?redirectUri=${encodeURIComponent(window.location.href)}`}>
              <button className="home-login-button">{Captions.Login}</button>
            </a>
            <br/>
          </>
        )}
        <Link to={pathnames.terms}>{Captions.TermsOfUsage}</Link>
      </Field>
    </MainContentWrapper>
  );
};
