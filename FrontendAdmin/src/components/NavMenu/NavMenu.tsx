import React, { FunctionComponent, ReactNode, useContext } from 'react';
import { NavLink, useLocation, matchPath } from 'react-router-dom';
import { AuthContext } from '../../context/AuthContext';
import { FieldsBlock } from '../FieldsBlock/FieldsBlock';
import { Captions, pathnames } from '../../constants';

import './NavMenu.css';
import { checkAdmin } from '../../utils/checkAdmin';

interface MenuItem {
  path: string;
  content: ReactNode;
}

const createMenuItem = (item: MenuItem, isActive: boolean) => (  
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
  const location = useLocation()
  const isPathActive = (pathname: string) => !!matchPath(pathname, location.pathname);

  const userContent = auth ?
    (
      <div className='nav-menu-user-content'>
                {auth.avatar && (
          <img
            src={auth.avatar}
            className='nav-menu-user-avatar'
            alt={`${auth.nickname} avatar`}
          />
        )}
        {auth.nickname}
      </div>
    ) :
    (Captions.UnauthorizedMessage);

  const items: MenuItem[] = admin ? [
    { path: pathnames.home, content: Captions.HomePageName },
    { path: pathnames.rooms, content: Captions.RoomsPageName },
    { path: pathnames.questions, content: Captions.QuestionsPageName },
    { path: pathnames.session, content: userContent },
  ] : [
    { path: pathnames.home, content: Captions.HomePageName },
    { path: pathnames.rooms, content: Captions.RoomsPageName },
    { path: pathnames.session, content: userContent },
  ];

  return (
    <FieldsBlock className="nav-menu">
      <nav>
        {items.map(item => createMenuItem(item, isPathActive(item.path)))}
      </nav>
    </FieldsBlock>
  );
};
