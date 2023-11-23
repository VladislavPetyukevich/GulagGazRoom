import React, { FunctionComponent } from 'react';
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
      <div>
        <div>{Captions.WelcomeMessage}, {auth.nickname}</div>
        <Link to={pathnames.rooms}>{Captions.ToRooms}</Link>
      </div>
    );
  }
  return (
    <div>
      <div>{Captions.WhoAreYou}</div>
      <a href={`${REACT_APP_BACKEND_URL}/login/twitch?redirectUri=${encodeURIComponent(window.location.href)}`}>
        <button className="home-login-button">{Captions.Login}</button>
      </a>
    </div>
  );
};
