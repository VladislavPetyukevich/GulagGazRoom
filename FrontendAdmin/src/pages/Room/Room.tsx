import React, { FunctionComponent, useMemo } from 'react';
import { useParams } from 'react-router-dom';
import useWebSocket from 'react-use-websocket';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { useCommunist } from './hooks/useCommunist';


export const Room: FunctionComponent = () => {
  let { id } = useParams();
  const communist = useCommunist();
  const socketUrl = useMemo(
    () => `ws://localhost:5043/ws?Authorization=${communist}&roomId=${id}`,
    [id, communist]
  );
  const { lastMessage, readyState } = useWebSocket(socketUrl);

  return (
    <MainContentWrapper>
      <Field>
        <div>Room</div>
        <div>readyState: {readyState}</div>
        <div>lastMessage: {lastMessage?.data}</div>
      </Field>
    </MainContentWrapper>
  );
};
