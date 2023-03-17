import React, { FunctionComponent, ReactElement } from 'react';
import { RouteProps, Navigate, } from 'react-router-dom';
import { pathnames } from '../constants';

type PrivateRouteProps = RouteProps & {
  allowed: boolean;
  redirectPathname?: keyof typeof pathnames;
  children: ReactElement<any, any> | null;
};

export const ProtectedRoute: FunctionComponent<PrivateRouteProps> = ({
  allowed,
  redirectPathname = 'home',
  children,
}) => {
  if (!allowed) {
    return <Navigate to={pathnames[redirectPathname]} replace />;
  }

  return children;
};
