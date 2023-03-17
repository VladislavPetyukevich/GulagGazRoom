import React, { FunctionComponent, useContext } from 'react';
import { Link, useLocation, matchPath } from 'react-router-dom';
import { AuthContext } from '../../context/AuthContext';
import { FieldsBlock } from '../FieldsBlock/FieldsBlock';
import { Field } from '../FieldsBlock/Field';
import { pathnames } from '../../constants';

import './NavMenu.css';

interface MenuItem {
  path: string;
  name: string;
}

const createMenuItem = (item: MenuItem, isActive: boolean) => (
  <Field key={item.path} className={isActive ? 'active' : ''}>
    <Link to={item.path}>{item.name}</Link>
  </Field>
);

export const NavMenu: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const location = useLocation()
  const isPathActive = (pathname: string) => !!matchPath(pathname, location.pathname);

  const items: MenuItem[] = [
    { path: pathnames.home, name: 'Home' },
    { path: pathnames.rooms, name: 'Rooms' },
    { path: pathnames.questions, name: 'Questions' },
    { path: pathnames.session, name: auth?.nickname || 'Unauthorized' },
  ];

  return (
    <FieldsBlock className="nav-menu">
      <nav>
        {items.map(item => createMenuItem(item, isPathActive(item.path)))}
      </nav>
    </FieldsBlock>
  );
};
