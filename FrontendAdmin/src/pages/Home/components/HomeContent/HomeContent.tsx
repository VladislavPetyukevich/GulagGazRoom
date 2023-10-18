import React, { FunctionComponent } from 'react';
import { Field } from '../../../../components/FieldsBlock/Field';
import { Captions, pathnames } from '../../../../constants';
import { REACT_APP_BACKEND_URL } from '../../../../config';
import { User } from '../../../../types/user';
import { Link } from 'react-router-dom';

interface HomeContentProps {
  auth: User | null;
}

export const HomeContent: FunctionComponent<HomeContentProps> = ({
  auth,
}) => {
  if (auth) {
    return (
      <Field>
        <div>{Captions.WelcomeMessage}, {auth.nickname}</div>
        <Link to={pathnames.rooms}>{Captions.ToRooms}</Link>
      </Field>
    );
  }
  return (
    <Field>
      <div>{Captions.WhoAreYou}</div>
      <a href={`${REACT_APP_BACKEND_URL}/login/twitch?redirectUri=${encodeURIComponent(window.location.href)}`}>
        <button className="home-login-button">{Captions.Login}</button>
      </a>
    </Field>
  );
};
