import React, { FunctionComponent, useContext } from 'react';
import { Link, Navigate, useParams } from 'react-router-dom';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { Captions, pathnames } from '../../constants';
import { AuthContext } from '../../context/AuthContext';
import { HomeAction } from './components/HomeContent/HomeAction';

import './Home.css';

export const Home: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const { redirect } = useParams();

  if (auth && redirect) {
    return <Navigate to={redirect} replace />;
  }

  return (
    <MainContentWrapper thin>
      <Field>
        <h1>{Captions.AppName}</h1>
        <p>{Captions.AppDescription}</p>
        <div className="home-action">
          <HomeAction auth={auth} />
        </div>
        <Link
          className="home-terms-link"
          to={pathnames.terms}
        >
          {Captions.TermsOfUsage}
        </Link>
      </Field>
    </MainContentWrapper>
  );
};
