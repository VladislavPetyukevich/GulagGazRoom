import React, { FunctionComponent, useContext } from 'react';
import { NavLink, useLocation, matchPath } from 'react-router-dom';
import { AuthContext } from '../../context/AuthContext';
import { FieldsBlock } from '../FieldsBlock/FieldsBlock';
import { Captions, pathnames } from '../../constants';

import './NavMenu.css';
import { checkAdmin } from '../../utils/checkAdmin';

interface MenuItem {
  path: string;
  name: string;
}

const createMenuItem = (item: MenuItem, isActive: boolean) => (  
  <NavLink
    key={item.path}
    to={item.path}
    className="field-wrap"
    data-cy={`nav-${item.path}`}
  >
    {item.name}
  </NavLink>
);

export const NavMenu: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const admin = checkAdmin(auth);
  const location = useLocation()
  const isPathActive = (pathname: string) => !!matchPath(pathname, location.pathname);

  const items: MenuItem[] = admin ? [
    { path: pathnames.home, name: Captions.HomePageName },
    { path: pathnames.rooms, name: Captions.RoomsPageName },
    { path: pathnames.questions, name: Captions.QuestionsPageName },
    { path: pathnames.session, name: auth?.nickname || Captions.UnauthorizedMessage },
  ] : [
    { path: pathnames.home, name: Captions.HomePageName },
    { path: pathnames.rooms, name: Captions.RoomsPageName },
    { path: pathnames.session, name: auth?.nickname || Captions.UnauthorizedMessage },
  ];

  return (
    <FieldsBlock className="nav-menu">
      <nav>
        {items.map(item => createMenuItem(item, isPathActive(item.path)))}
      </nav>
    </FieldsBlock>
  );
};
