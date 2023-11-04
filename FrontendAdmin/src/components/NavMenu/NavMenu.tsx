import React, { FunctionComponent, ReactNode, useContext } from 'react';
import { NavLink } from 'react-router-dom';
import { AuthContext } from '../../context/AuthContext';
import { FieldsBlock } from '../FieldsBlock/FieldsBlock';
import { Captions, pathnames } from '../../constants';
import { checkAdmin } from '../../utils/checkAdmin';
import { UserAvatar } from '../UserAvatar/UserAvatar';

import './NavMenu.css';

interface MenuItem {
  path: string;
  content: ReactNode;
}

const createMenuItem = (item: MenuItem) => (
  <NavLink
    key={item.path}
    to={item.path}
    className="field-wrap"
  >
    {item.content}
  </NavLink>
);

export const NavMenu: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const admin = checkAdmin(auth);

  const userContent = auth ?
    (
      <div className='nav-menu-user-content'>
        {auth.avatar && (
          <UserAvatar
            src={auth.avatar}
            nickname={auth.nickname}
          />
        )}
        {auth.nickname}
      </div>
    ) :
    (Captions.UnauthorizedMessage);

  const items: MenuItem[] = admin ? [
    { path: pathnames.home.replace(':redirect?', ''), content: Captions.HomePageName },
    { path: pathnames.rooms, content: Captions.RoomsPageName },
    { path: pathnames.questions, content: Captions.QuestionsPageName },
    { path: pathnames.session, content: userContent },
  ] : [
    { path: pathnames.home.replace(':redirect?', ''), content: Captions.HomePageName },
    { path: pathnames.rooms, content: Captions.RoomsPageName },
    { path: pathnames.session, content: userContent },
  ];

  return (
    <FieldsBlock className="nav-menu">
      <nav>
        {items.map(item => createMenuItem(item))}
      </nav>
    </FieldsBlock>
  );
};
