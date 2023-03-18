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
    name: 'GasOn 🤿',
    value: 0,
  }
}, {
  id: 'gasReactionOffId',
  type: {
    eventType: 'GasOff',
    name: 'GasOff 👌',
    value: 0,
  }
}];

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

  const {
    apiMethodState: apiSendGasState,
    fetchData: sendRoomGas,
  } = useApiMethod<unknown>({ noParseResponse: true });
  const {
    process: { loading: loadingRoomGas, error: errorRoomGas },
  } = apiSendGasState;

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
    console.log('reaction: ', reaction);
    if (!room) {
      throw new Error('Error sending reaction. Room not found.');
    }
    sendRoomGas(roomsApiDeclaration.sendGasEvent({
      roomId: room.id,
      type: (reaction as GasReaction).type.eventType,
    }));
  }, [room, sendRoomGas]);

  const renderReactionsField = useCallback(() => {
    return (
      <Field>
        <div>Reactions:</div>
        <ReactionsList
          reactions={reactions || []}
          onClick={handleReactionClick}
        />
        <ReactionsList
          reactions={gasReactions}
          onClick={handleGasReactionClick}
        />
        {loadingRoomReaction && <div>Sending reaction...</div>}
        {errorRoomReaction && <div>Error sending reaction</div>}
        {loadingRoomGas && <div>Sending gas event...</div>}
        {errorRoomGas && <div>Error sending gas event</div>}
      </Field>
    );
  }, [
    loadingRoomReaction,
    loadingRoomGas,
    errorRoomReaction,
    errorRoomGas,
    reactions,
    handleReactionClick,
    handleGasReactionClick,
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
