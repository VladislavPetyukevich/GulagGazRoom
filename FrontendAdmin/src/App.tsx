import React, { FunctionComponent, useCallback, useEffect } from 'react';
import { BrowserRouter } from 'react-router-dom';
import { AppRoutes } from './routes/AppRoutes';
import { NavMenu } from './components/NavMenu/NavMenu';
import { AuthContext } from './context/AuthContext';
import { useGetMeApi } from './hooks/useGetMeApi';
import { Loader } from './components/Loader/Loader';
import { MainContentWrapper } from './components/MainContentWrapper/MainContentWrapper';
import { Field } from './components/FieldsBlock/Field';
import { useCommunist } from './hooks/useCommunist';

import './App.css';

export const App: FunctionComponent = () => {
  const { getCommunist, resetCommunist } = useCommunist();
  const communist = getCommunist();
  const { getMeState, loadMe } = useGetMeApi();
  const { process: { loading, error }, user } = getMeState;
  const userWillLoad = communist && !user && !error;

  useEffect(() => {
    if (communist) {
      loadMe();
    }
  }, [communist, loadMe]);

  const handlePageReset = useCallback(() => {
    resetCommunist();
  }, [resetCommunist]);

  const renderMainContent = useCallback(() => {
    if (loading || userWillLoad) {
      return (
        <MainContentWrapper>
          <Field>
            <div>Loading user data...</div>
            <Loader/>
          </Field>
        </MainContentWrapper>
      )
    }
    if (error) {
      return (
        <MainContentWrapper>
          <Field>
            <div>Failed to get user data: {error}</div>
            <button onClick={handlePageReset}>Reset page</button>
          </Field>
        </MainContentWrapper>
      );
    }
    return (
      <AppRoutes user={user} />
    );
  }, [loading, userWillLoad, error, user, handlePageReset]);

  return (
    <BrowserRouter>
      <AuthContext.Provider value={user}>
        <div className="App">
          <header>
            <h1>GULAG ADMIN</h1>
            <NavMenu />
          </header>
          {renderMainContent()}
        </div>
      </AuthContext.Provider>
    </BrowserRouter>
  );
};
