import React, { FunctionComponent, useCallback, useContext } from 'react';
import { AuthContext } from '../../context/AuthContext';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { useCommunist } from '../../hooks/useCommunist';
import { Captions } from '../../constants';

export const Session: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const { resetCommunist } = useCommunist();

  const handleLogOut = useCallback(() => {
    resetCommunist();
  }, [resetCommunist]);

  const renderAuth = useCallback(() => {
    if (!auth) {
      return (
        <Field>
          <div>I have nothing to show you... What the heck</div>
        </Field>
      );
    }
    return (
      <>
        <Field>
          <div>{Captions.LoggedInAs}:</div>
        </Field>
        <Field>
          <div>nickname: {auth.nickname}</div>
        </Field>
        <Field>
          <div>roles: {JSON.stringify(auth.roles)}</div>
        </Field>
        <Field>
          <button onClick={handleLogOut}>{Captions.LogOut}🚪</button>
        </Field>
      </>
    );
  }, [auth, handleLogOut]);

  return (
    <MainContentWrapper>
      {renderAuth()}
    </MainContentWrapper>
  );
};
