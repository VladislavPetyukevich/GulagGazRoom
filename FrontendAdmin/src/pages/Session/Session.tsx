import React, { FunctionComponent, useCallback, useContext } from 'react';
import { AuthContext } from '../../context/AuthContext';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { useCommunist } from '../../hooks/useCommunist';
import { Captions } from '../../constants';
import { checkAdmin } from '../../utils/checkAdmin';
import { ThemeSwitch } from '../../components/ThemeSwitch/ThemeSwitch';

import './Session.css';

export const Session: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const admin = checkAdmin(auth);
  const { resetCommunist } = useCommunist();
  const role = admin ? 'Начальник управления' : 'Очередной штатник';

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
        <Field className="session-info">
          <h2>{Captions.Settings}</h2>
          <ThemeSwitch />
          <table>
            <thead>
              <tr>
                <th>Позывной</th>
                <th>Роль</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td>{auth.nickname}</td>
                <td>{role}</td>
              </tr>
            </tbody>
          </table>
        </Field>
        <Field>
          <button onClick={handleLogOut}>{Captions.LogOut}🚪</button>
        </Field>
      </>
    );
  }, [auth, role, handleLogOut]);

  return (
    <MainContentWrapper>
      {renderAuth()}
    </MainContentWrapper>
  );
};
