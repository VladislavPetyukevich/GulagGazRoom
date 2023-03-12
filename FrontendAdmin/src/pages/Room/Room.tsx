import React, { FunctionComponent, useCallback, useEffect, useMemo } from 'react';
import { useParams } from 'react-router-dom';
import useWebSocket from 'react-use-websocket';
import { Field } from '../../components/FieldsBlock/Field';
import { Loader } from '../../components/Loader/Loader';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { useCommunist } from './hooks/useCommunist';
import { useRoomGetApi } from './hooks/useRoomGetApi';


export const Room: FunctionComponent = () => {
  let { id } = useParams();
  const communist = useCommunist();
  const socketUrl = useMemo(
    () => `ws://localhost:5043/ws?Authorization=${communist}&roomId=${id}`,
    [id, communist]
  );
  const { lastMessage, readyState } = useWebSocket(socketUrl);
  const { roomState, loadRoom } = useRoomGetApi();
  const { process: { loading, error }, room } = roomState;

  useEffect(() => {
    console.log('id: ', id);
    if (!id) {
      throw new Error('Room id not found');
    }
    loadRoom({ roomId: id });
  }, [id, loadRoom]);

  const renderRoomContent = useCallback(() => {
    if (error) {
      return (
        <Field>
          <div>Error: {error}</div>
        </Field>
      );
    }
    if (loading) {
      return (
        <Field>
          <Loader />
        </Field>
      );
    }
    const questions = (room?.questions && room.questions.length > 0) ?
      room.questions.map(question => question.value).join(', ') :
      'No questions';
    return (
      <>
        <Field>
          <div>{room?.name}</div>
        </Field>
        <Field>
          <div>{questions}</div>
        </Field>
      </>
    );
  }, [loading, error, room]);

  return (
    <MainContentWrapper>
      <Field>
        <div>Room</div>
        <div>readyState: {readyState}</div>
        <div>lastMessage: {lastMessage?.data}</div>
      </Field>
      {renderRoomContent()}
    </MainContentWrapper>
  );
};
