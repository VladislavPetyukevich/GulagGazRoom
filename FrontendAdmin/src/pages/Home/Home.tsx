import React, { FunctionComponent, useContext } from 'react';
import { Link, Navigate, useParams } from 'react-router-dom';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { Captions, pathnames } from '../../constants';
import { AuthContext } from '../../context/AuthContext';
import { HomeContent } from './components/HomeContent/HomeContent';

import './Home.css';

export const Home: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const { redirect } = useParams();

  if (auth && redirect) {
    return <Navigate to={redirect} replace />;
  }

  return (
    <MainContentWrapper className="home">
      <HomeContent auth={auth} />
      <Field>
        <Link to={pathnames.terms}>{Captions.TermsOfUsage}</Link>
      </Field>
    </MainContentWrapper>
  );
};
