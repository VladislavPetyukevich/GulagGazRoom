import React, { FunctionComponent, useCallback, useEffect, useMemo } from 'react';
import { useParams } from 'react-router-dom';
import useWebSocket from 'react-use-websocket';
import { roomsApiDeclaration } from '../../apiDeclarations';
import { Field } from '../../components/FieldsBlock/Field';
import { Loader } from '../../components/Loader/Loader';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { useApiMethod } from '../../hooks/useApiMethod';
import { useCommunist } from '../../hooks/useCommunist';
import { Room as RoomType } from '../../types/room';

import './Room.css';

export const Room: FunctionComponent = () => {
  let { id } = useParams();
  const { getCommunist } = useCommunist();
  const communist = getCommunist();
  const socketUrl = useMemo(
    () => `ws://localhost:5043/ws?Authorization=${communist}&roomId=${id}`,
    [id, communist]
  );
  const { lastMessage, readyState } = useWebSocket(socketUrl);
  const { apiMethodState, fetchData } = useApiMethod<RoomType>();
  const { process: { loading, error }, data: room } = apiMethodState;

  useEffect(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchData(roomsApiDeclaration.getById(id));
  }, [id, fetchData]);

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
        <Field className="interviewee-frame-wrapper">
          <iframe
            title="interviewee-client-frame"
            className="interviewee-frame"
            src={`http://localhost:8080/?roomId=${room?.id}`}
          >
          </iframe>
        </Field>
      </>
    );
  }, [loading, error, room]);

  return (
    <MainContentWrapper className="room-page">
      <Field>
        <div>Room</div>
        <div>readyState: {readyState}</div>
        <div>lastMessage: {lastMessage?.data}</div>
      </Field>
      {renderRoomContent()}
    </MainContentWrapper>
  );
};
