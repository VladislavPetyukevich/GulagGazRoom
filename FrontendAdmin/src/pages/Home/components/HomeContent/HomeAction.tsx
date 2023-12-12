import React, { FunctionComponent } from 'react';
import { Captions, pathnames } from '../../../../constants';
import { REACT_APP_BACKEND_URL } from '../../../../config';
import { User } from '../../../../types/user';
import { Link } from 'react-router-dom';

interface HomeActionProps {
  auth: User | null;
}

export const HomeAction: FunctionComponent<HomeActionProps> = ({
  auth,
}) => {
  if (auth) {
    return (
      <Link to={pathnames.rooms}>
        <button>
          {Captions.ToRooms}
        </button>
      </Link>
    );
  }
  return (
    <a href={`${REACT_APP_BACKEND_URL}/login/twitch?redirectUri=${encodeURIComponent(window.location.href)}`}>
      <button>{Captions.Login}</button>
    </a>
  );
};
