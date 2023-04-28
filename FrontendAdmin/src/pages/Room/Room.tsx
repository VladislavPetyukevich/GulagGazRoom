import React, { FunctionComponent, useCallback, useContext, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import useWebSocket from 'react-use-websocket';
import { roomsApiDeclaration } from '../../apiDeclarations';
import { Field } from '../../components/FieldsBlock/Field';
import { Loader } from '../../components/Loader/Loader';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { REACT_APP_INTERVIEW_FRONTEND_URL, REACT_APP_WS_URL } from '../../config';
import { Captions } from '../../constants';
import { AuthContext } from '../../context/AuthContext';
import { useApiMethod } from '../../hooks/useApiMethod';
import { useCommunist } from '../../hooks/useCommunist';
import { Room as RoomType } from '../../types/room';
import { checkAdmin } from '../../utils/checkAdmin';
import { CloseRoom } from './components/CloseRoom/CloseRoom';
import { Twitch } from './components/Twitch/Twitch';
import { Interviewee } from './components/Interviewee/Interviewee';
import { Reactions } from './components/Reactions/Reactions';
import { ActiveQuestion } from './components/ActiveQuestion/ActiveQuestion';

import './Room.css';

export const Room: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const admin = checkAdmin(auth);
  const { getCommunist } = useCommunist();
  const communist = getCommunist();
  let { id } = useParams();
  const socketUrl = `${REACT_APP_WS_URL}?Authorization=${communist}&roomId=${id}`;
  const { lastMessage } = useWebSocket(socketUrl);

  const { apiMethodState, fetchData } = useApiMethod<RoomType>();
  const { process: { loading, error }, data: room } = apiMethodState;

  useEffect(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchData(roomsApiDeclaration.getById(id));
  }, [id, fetchData]);

  const handleCopyRoomLink = useCallback(() => {
    navigator.clipboard.writeText(
      `${REACT_APP_INTERVIEW_FRONTEND_URL}/?roomId=${id}`
    );
  }, [id]);

  const renderRoomContent = useCallback(() => {
    if (error) {
      return (
        <Field>
          <div>{Captions.Error}: {error}</div>
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
    return (
      <>
        <Field className='room-title'>
          <h2>{Captions.Room}: {room?.name}</h2>
          <button
            className="copy-link-button"
            onClick={handleCopyRoomLink}
          >
            {Captions.CopyRoomLink}
          </button>
        </Field>
        <Field>
          <CloseRoom />
        </Field>
        <Field className="reactions-field">
          {admin && (
            <ActiveQuestion
              room={room}
              lastWebSocketMessage={lastMessage}
            />
          )}
          <Reactions
            admin={admin}
            room={room}
          />
        </Field>
        <Field className="twitch-embed-field">
          <Twitch
            channel={room?.twitchChannel || ''}
            autoplay={!admin}
          />
        </Field>
        <Field className={`interviewee-frame-wrapper ${admin ? 'admin' : ''}`}>
          <Interviewee
            roomId={room?.id || ''}
            fov={110}
            muted={!admin}
          />
        </Field>
      </>
    );
  }, [
    admin,
    loading,
    error,
    room,
    lastMessage,
    handleCopyRoomLink,
  ]);

  return (
    <MainContentWrapper className="room-page">
      {renderRoomContent()}
    </MainContentWrapper>
  );
};
