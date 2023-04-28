import React, { FunctionComponent, MouseEventHandler, useCallback, useContext, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import useWebSocket from 'react-use-websocket';
import { roomQuestionApiDeclaration, roomsApiDeclaration } from '../../apiDeclarations';
import { ActiveQuestionSelector } from '../../components/ActiveQuestionSelector/ActiveQuestionSelector';
import { Field } from '../../components/FieldsBlock/Field';
import { Loader } from '../../components/Loader/Loader';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { REACT_APP_INTERVIEW_FRONTEND_URL, REACT_APP_WS_URL } from '../../config';
import { Captions } from '../../constants';
import { AuthContext } from '../../context/AuthContext';
import { useApiMethod } from '../../hooks/useApiMethod';
import { useCommunist } from '../../hooks/useCommunist';
import { Question } from '../../types/question';
import { Room as RoomType } from '../../types/room';
import { checkAdmin } from '../../utils/checkAdmin';
import { CloseRoom } from './components/CloseRoom/CloseRoom';
import { Twitch } from './components/Twitch/Twitch';
import { Interviewee } from './components/Interviewee/Interviewee';

import './Room.css';
import { Reactions } from './components/Reactions/Reactions';

export const Room: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const admin = checkAdmin(auth);
  const { getCommunist } = useCommunist();
  const communist = getCommunist();
  let { id } = useParams();
  const socketUrl = `${REACT_APP_WS_URL}?Authorization=${communist}&roomId=${id}`;
  const { lastMessage } = useWebSocket(socketUrl);
  const [showClosedQuestions, setShowClosedQuestions] = useState(false);

  const { apiMethodState, fetchData } = useApiMethod<RoomType>();
  const { process: { loading, error }, data: room } = apiMethodState;

  const {
    apiMethodState: apiSendActiveQuestionState,
    fetchData: sendRoomActiveQuestion,
  } = useApiMethod<unknown>();
  const {
    process: { loading: loadingRoomActiveQuestion, error: errorRoomActiveQuestion },
  } = apiSendActiveQuestionState;

  const {
    apiMethodState: apiOpenRoomQuestions,
    fetchData: getRoomOpenQuestions,
  } = useApiMethod<Array<Question['id']>>();
  const {
    data: openRoomQuestions,
  } = apiOpenRoomQuestions;

  useEffect(() => {
    if (!id) {
      return;
    }
    getRoomOpenQuestions(roomQuestionApiDeclaration.getRoomQuestions({
      RoomId: id,
      State: 'Open',
    }))
  }, [id, getRoomOpenQuestions]);

  useEffect(() => {
    if (!id) {
      return;
    }
    try {
      const parsedData = JSON.parse(lastMessage?.data);
      if (parsedData.Type !== 'ChangeRoomQuestionState') {
        return;
      }
      if (parsedData.Value.NewState !== 'Active') {
        return;
      }
      getRoomOpenQuestions(roomQuestionApiDeclaration.getRoomQuestions({
        RoomId: id,
        State: 'Open',
      }))
    } catch { }
  }, [id, lastMessage, getRoomOpenQuestions]);

  useEffect(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchData(roomsApiDeclaration.getById(id));
  }, [id, fetchData]);

  const handleQuestionSelect = useCallback((question: Question) => {
    if (!room) {
      throw new Error('Error sending reaction. Room not found.');
    }
    sendRoomActiveQuestion(roomQuestionApiDeclaration.changeActiveQuestion({
      roomId: room.id,
      questionId: question.id,
    }));
  }, [room, sendRoomActiveQuestion]);

  const handleCopyRoomLink = useCallback(() => {
    navigator.clipboard.writeText(
      `${REACT_APP_INTERVIEW_FRONTEND_URL}/?roomId=${id}`
    );
  }, [id]);

  const handleShowClosedQuestions: MouseEventHandler<HTMLInputElement> = useCallback((e) => {
    setShowClosedQuestions(e.currentTarget.checked);
  }, []);

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
            <div>
              <span>{Captions.ShowClosedQuestions}</span>
              <input type="checkbox" onClick={handleShowClosedQuestions} />
              <ActiveQuestionSelector
                showClosedQuestions={showClosedQuestions}
                questions={room?.questions || []}
                openQuestions={openRoomQuestions || []}
                placeHolder={Captions.SelectActiveQuestion}
                onSelect={handleQuestionSelect}
              />
              {loadingRoomActiveQuestion && <div>{Captions.SendingActiveQuestion}...</div>}
              {errorRoomActiveQuestion && <div>{Captions.ErrorSendingActiveQuestion}...</div>}
            </div>
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
    loadingRoomActiveQuestion,
    error,
    errorRoomActiveQuestion,
    room,
    openRoomQuestions,
    showClosedQuestions,
    handleQuestionSelect,
    handleCopyRoomLink,
    handleShowClosedQuestions,
  ]);

  return (
    <MainContentWrapper className="room-page">
      {renderRoomContent()}
    </MainContentWrapper>
  );
};
