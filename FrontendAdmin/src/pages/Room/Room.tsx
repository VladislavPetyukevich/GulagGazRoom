import React, { FunctionComponent, useCallback, useEffect, useMemo } from 'react';
import { useParams } from 'react-router-dom';
import useWebSocket from 'react-use-websocket';
import { reactionsApiDeclaration, roomReactionApiDeclaration, roomsApiDeclaration } from '../../apiDeclarations';
import { Field } from '../../components/FieldsBlock/Field';
import { Loader } from '../../components/Loader/Loader';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { ReactionsList } from '../../components/ReactionsList/ReactionsList';
import { useApiMethod } from '../../hooks/useApiMethod';
import { useCommunist } from '../../hooks/useCommunist';
import { Reaction } from '../../types/reaction';
import { Room as RoomType } from '../../types/room';

import './Room.css';

const reactionsPageSize = 30;
const reactionsPageNumber = 1;

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
    }))
  }, [room, sendRoomReaction]);

  const renderReactionsField = useCallback(() => {
    return (
      <Field>
        <div>Reactions:</div>
        <ReactionsList
          reactions={reactions || []}
          onClick={handleReactionClick}
        />
        {loadingRoomReaction && <div>Sending reaction...</div>}
        {errorRoomReaction && <div>Error sending reaction</div>}
      </Field>
    );
  }, [
    loadingRoomReaction,
    errorRoomReaction,
    reactions,
    handleReactionClick,
  ]);

  const renderRoomContent = useCallback(() => {
    if (error) {
      return (
        <Field>
          <div>Error: {error}</div>
        </Field>
      );
    }
    if (errorReactions) {
      return (
        <Field>
          <div>Reactions loading error: {errorReactions}</div>
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
        {renderReactionsField()}
        <Field className="interviewee-frame-wrapper">
          <iframe
            title="interviewee-client-frame"
            className="interviewee-frame"
            src={`http://localhost:8080/?roomId=${room?.id}&noPointerLock=1&fov=115`}
          >
          </iframe>
        </Field>
      </>
    );
  }, [
    loading,
    loadingReactions,
    error,
    errorReactions,
    room,
    renderReactionsField,
  ]);

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
