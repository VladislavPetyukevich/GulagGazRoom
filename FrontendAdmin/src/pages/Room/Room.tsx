import React, { FunctionComponent, useCallback, useContext, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { reactionsApiDeclaration, roomQuestionApiDeclaration, roomReactionApiDeclaration, roomsApiDeclaration } from '../../apiDeclarations';
import { ActiveQuestionSelector } from '../../components/ActiveQuestionSelector/ActiveQuestionSelector';
import { Field } from '../../components/FieldsBlock/Field';
import { Loader } from '../../components/Loader/Loader';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { ReactionsList } from '../../components/ReactionsList/ReactionsList';
import { REACT_APP_INTERVIEW_FRONTEND_URL } from '../../config';
import { Captions } from '../../constants';
import { AuthContext } from '../../context/AuthContext';
import { useApiMethod } from '../../hooks/useApiMethod';
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
  let { id } = useParams();
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

  const renderReactionsField = useCallback(() => {
    return (
      <Field>
        <div>
          <span>{Captions.Reactions}:</span>
          <ReactionsList
            reactions={reactions || []}
            onClick={handleReactionClick}
          />
        </div>
        {admin && (
          <div>
            <span>{Captions.Gas}:</span>
            <ReactionsList
              reactions={gasReactions}
              onClick={handleGasReactionClick}
            />
          </div>
        )}
        {loadingRoomReaction && <div>{Captions.SendingReaction}...</div>}
        {errorRoomReaction && <div>{Captions.ErrorSendingReaction}</div>}
        {loadingRoomGas && <div>{Captions.SendingGasEvent}...</div>}
        {errorRoomGas && <div>{Captions.ErrorSendingGasEvent}</div>}
      </Field>
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
        <Field>
          <div>{Captions.Room}: {room?.name}</div>
          <button onClick={handleCopyRoomLink}>{Captions.CopyRoomLink}</button>
        </Field>
        {admin && (
          <Field>
            <div>Установить тему допроса:</div>
            <ActiveQuestionSelector
              questions={room?.questions || []}
              selectButtonLabel={Captions.SetActiveQuestion}
              onSelect={handleQuestionSelect}
            />
            {loadingRoomActiveQuestion && <div>{Captions.SendingActiveQuestion}...</div>}
            {errorRoomActiveQuestion && <div>{Captions.ErrorSendingActiveQuestion}...</div>}
          </Field>
        )}
        {renderReactionsField()}
        <Field className="interviewee-frame-wrapper">
          <iframe
            title="interviewee-client-frame"
            className="interviewee-frame"
            src={`${REACT_APP_INTERVIEW_FRONTEND_URL}/?roomId=${room?.id}&noPointerLock=1&fov=125`}
          >
          </iframe>
        </Field>
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
    renderReactionsField,
    handleQuestionSelect,
    handleCopyRoomLink,
  ]);

  return (
    <MainContentWrapper className="room-page">
      {renderRoomContent()}
    </MainContentWrapper>
  );
};
