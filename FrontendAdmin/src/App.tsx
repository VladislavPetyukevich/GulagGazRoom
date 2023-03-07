import React, { FunctionComponent } from 'react';
import { BrowserRouter } from 'react-router-dom';
import { AppRoutes } from './routes/AppRoutes';
import { NavMenu } from './components/NavMenu/NavMenu';

import './App.css';

export const App: FunctionComponent = () => {
  return (
    <BrowserRouter>
      <div className="App">
        <header>
          <h1>GULAG ADMIN</h1>
          <NavMenu />
        </header>
        <AppRoutes />
      </div>
    </BrowserRouter>
  );
};
