import React, { FunctionComponent, MouseEventHandler, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react';
import { useParams } from 'react-router-dom';
import { TwitchEmbed, TwitchEmbedInstance } from 'react-twitch-embed';
import useWebSocket from 'react-use-websocket';
import { reactionsApiDeclaration, roomQuestionApiDeclaration, roomReactionApiDeclaration, roomsApiDeclaration } from '../../apiDeclarations';
import { ActiveQuestionSelector } from '../../components/ActiveQuestionSelector/ActiveQuestionSelector';
import { Field } from '../../components/FieldsBlock/Field';
import { Loader } from '../../components/Loader/Loader';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { ReactionsList } from '../../components/ReactionsList/ReactionsList';
import { REACT_APP_INTERVIEW_FRONTEND_URL, REACT_APP_WS_URL } from '../../config';
import { Captions } from '../../constants';
import { AuthContext } from '../../context/AuthContext';
import { useApiMethod } from '../../hooks/useApiMethod';
import { useCommunist } from '../../hooks/useCommunist';
import { Question } from '../../types/question';
import { Reaction } from '../../types/reaction';
import { Room as RoomType } from '../../types/room';
import { checkAdmin } from '../../utils/checkAdmin';

import './Room.css';

const reactionsPageSize = 30;
const reactionsPageNumber = 1;

interface GasReaction extends Reaction {
  type: {
    eventType: 'GasOn' | 'GasOff';
    name: string;
    value: number;
  }
}

const gasReactions: GasReaction[] = [{
  id: 'gasReactionOnId',
  type: {
    eventType: 'GasOn',
    name: `${Captions.GasOn} 🤿`,
    value: 0,
  }
}, {
  id: 'gasReactionOffId',
  type: {
    eventType: 'GasOff',
    name: `${Captions.GasOff} 👌`,
    value: 0,
  }
}];

export const Room: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const admin = checkAdmin(auth);
  const embed = useRef<TwitchEmbedInstance>();
  const { getCommunist } = useCommunist();
  const communist = getCommunist();
  let { id } = useParams();
  const socketUrl = `${REACT_APP_WS_URL}?Authorization=${communist}&roomId=${id}`;
  const { lastMessage } = useWebSocket(socketUrl);
  const [showClosedQuestions, setShowClosedQuestions] = useState(false);

  const { apiMethodState, fetchData } = useApiMethod<RoomType>();
  const { process: { loading, error }, data: room } = apiMethodState;

  const {
    apiMethodState: apiReactionsState,
    fetchData: fetchReactions,
  } = useApiMethod<Reaction[]>();
  const {
    process: { loading: loadingReactions, error: errorReactions },
    data: reactions,
  } = apiReactionsState;

  const {
    apiMethodState: apiRoomReactionState,
    fetchData: sendRoomReaction,
  } = useApiMethod<unknown>();
  const {
    process: { loading: loadingRoomReaction, error: errorRoomReaction },
  } = apiRoomReactionState;

  const {
    apiMethodState: apiSendGasState,
    fetchData: sendRoomGas,
  } = useApiMethod<unknown>({ noParseResponse: true });
  const {
    process: { loading: loadingRoomGas, error: errorRoomGas },
  } = apiSendGasState;

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

  const {
    apiMethodState: closeRoomState,
    fetchData: closeRoom,
  } = useApiMethod<unknown>({ noParseResponse: true });
  const {
    process: { loading: closeRoomLoading, error: closeRoomError },
  } = closeRoomState;

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

  useEffect(() => {
    fetchReactions(reactionsApiDeclaration.getPage({
      PageSize: reactionsPageSize,
      PageNumber: reactionsPageNumber,
    }));
  }, [fetchReactions]);

  const handleReactionClick = useCallback((reaction: Reaction) => {
    if (!room) {
      throw new Error('Error sending reaction. Room not found.');
    }
    sendRoomReaction(roomReactionApiDeclaration.send({
      reactionId: reaction.id,
      roomId: room.id,
    }));
  }, [room, sendRoomReaction]);

  const handleGasReactionClick = useCallback((reaction: Reaction) => {
    if (!room) {
      throw new Error('Error sending reaction. Room not found.');
    }
    sendRoomGas(roomsApiDeclaration.sendGasEvent({
      roomId: room.id,
      type: (reaction as GasReaction).type.eventType,
    }));
  }, [room, sendRoomGas]);

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

  const handleCloseRoom = useCallback(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    closeRoom(roomsApiDeclaration.close(id));
  }, [id, closeRoom]);

  const handleShowClosedQuestions: MouseEventHandler<HTMLInputElement> = useCallback((e) => {
    setShowClosedQuestions(e.currentTarget.checked);
  }, []);

  const renderReactions = useCallback(() => {
    return (
      <div>
        <div className="reaction-wrapper">
          <span>{Captions.Reactions}:</span>
          <ReactionsList
            sortOrder={-1}
            reactions={reactions || []}
            onClick={handleReactionClick}
          />
        </div>
        {admin && (
          <div className="reaction-wrapper">
            <span>{Captions.Gas}:</span>
            <ReactionsList
              sortOrder={1}
              reactions={gasReactions}
              onClick={handleGasReactionClick}
            />
          </div>
        )}
        {loadingRoomReaction && <div>{Captions.SendingReaction}...</div>}
        {errorRoomReaction && <div>{Captions.ErrorSendingReaction}</div>}
        {loadingRoomGas && <div>{Captions.SendingGasEvent}...</div>}
        {errorRoomGas && <div>{Captions.ErrorSendingGasEvent}</div>}
      </div>
    );
  }, [
    admin,
    loadingRoomReaction,
    loadingRoomGas,
    errorRoomReaction,
    errorRoomGas,
    reactions,
    handleReactionClick,
    handleGasReactionClick,
  ]);

  const handleReady = (e: TwitchEmbedInstance) => {
    embed.current = e;
  };

  const twitch = useMemo(() => {
    if (loading || !room) {
      return <div>Loaing twitch...</div>;
    }
    return (
      <Field className="twitch-embed-field">
        <TwitchEmbed
          channel={room.twitchChannel}
          autoplay={!admin}
          withChat
          darkMode={true}
          onVideoReady={handleReady}
        />
      </Field>
    );
  }, [loading, room, admin]);

  const interviewee = useMemo(() => (
    <Field className={`interviewee-frame-wrapper ${admin ? 'admin' : ''}`}>
      <iframe
        title="interviewee-client-frame"
        className="interviewee-frame"
        src={`${REACT_APP_INTERVIEW_FRONTEND_URL}/?roomId=${room?.id}&${admin ? '' : 'muted=1'}&fov=${admin ? '110' : '110'}`}
      >
      </iframe>
    </Field>
  ), [admin, room?.id]);

  const renderRoomContent = useCallback(() => {
    if (error) {
      return (
        <Field>
          <div>{Captions.Error}: {error}</div>
        </Field>
      );
    }
    if (errorReactions) {
      return (
        <Field>
          <div>{Captions.ReactionsLoadingError}: {errorReactions}</div>
        </Field>
      );
    }
    if (loading || loadingReactions) {
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
            data-cy="copy-link-button"
            onClick={handleCopyRoomLink}
          >
            {Captions.CopyRoomLink}
          </button>
        </Field>
        <Field>
          <button
            onClick={handleCloseRoom}
          >
            {Captions.CloseRoom}
          </button>
          {closeRoomLoading && (<div>{Captions.CloseRoomLoading}...</div>)}
          {closeRoomError && (<div>{Captions.Error}: {closeRoomError}</div>)}
        </Field>
        <Field className="reactions-field">
          {admin && (
            <div>
              <span>{Captions.ShowClosedQuestions}</span>
              <input
                type="checkbox"
                data-cy="checkbox-closed-questions"
                onClick={handleShowClosedQuestions}
              />
              <ActiveQuestionSelector
                showClosedQuestions={showClosedQuestions}
                questions={room?.questions || []}
                openQuestions={openRoomQuestions || []}
                placeHolder={Captions.SelectActiveQuestion}
                onSelect={handleQuestionSelect}
              />
              {loadingRoomActiveQuestion && <div>{Captions.SendingActiveQuestion}...</div>}
              {errorRoomActiveQuestion && (
                <div
                  data-cy="error-sending-active-question"
                >
                  {Captions.ErrorSendingActiveQuestion}...
                </div>
              )}
            </div>
          )}
          {renderReactions()}
        </Field>
        {admin ? interviewee : twitch}
        {admin ? twitch : interviewee}
      </>
    );
  }, [
    admin,
    loading,
    loadingReactions,
    loadingRoomActiveQuestion,
    error,
    errorReactions,
    errorRoomActiveQuestion,
    room,
    twitch,
    interviewee,
    openRoomQuestions,
    showClosedQuestions,
    closeRoomLoading,
    closeRoomError,
    renderReactions,
    handleQuestionSelect,
    handleCopyRoomLink,
    handleShowClosedQuestions,
    handleCloseRoom,
  ]);

  return (
    <MainContentWrapper className="room-page">
      {renderRoomContent()}
    </MainContentWrapper>
  );
};
