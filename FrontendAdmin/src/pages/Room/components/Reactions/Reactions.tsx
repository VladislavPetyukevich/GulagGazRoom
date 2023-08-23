import { FunctionComponent, useCallback, useEffect } from 'react';
import { Captions } from '../../../../constants';
import { ReactionsList } from '../../../../components/ReactionsList/ReactionsList';
import { useApiMethod } from '../../../../hooks/useApiMethod';
import { Reaction } from '../../../../types/reaction';
import { PaginationUrlParams, SendEventBody, SendReactionBody, eventApiDeclaration, reactionsApiDeclaration, roomReactionApiDeclaration, roomsApiDeclaration } from '../../../../apiDeclarations';
import { Room } from '../../../../types/room';
import { Event } from '../../../../types/event';
import { Loader } from '../../../../components/Loader/Loader';
import { useAdditionalReactions } from '../../hooks/useAdditionalReactions';
import { ParticipantType } from '../../../../types/user';

const reactionsPageSize = 30;
const reactionsPageNumber = 1;

const eventToReaction = (event: Event): Reaction => ({
  id: event.id,
  type: {
    id: event.id,
    name: event.type,
    value: 0,
  }
});

export interface ReactionsProps {
  room: Room | null;
  roles: string[];
  participantType: ParticipantType | null;
}

export const Reactions: FunctionComponent<ReactionsProps> = ({
  room,
  roles,
  participantType,
}) => {
  const {
    apiMethodState: apiReactionsState,
    fetchData: fetchReactions,
  } = useApiMethod<Reaction[], PaginationUrlParams>(reactionsApiDeclaration.getPage);
  const {
    process: { loading: loadingReactions, error: errorReactions },
    data: reactions,
  } = apiReactionsState;

  const {
    apiMethodState: apiRoomReactionState,
    fetchData: sendRoomReaction,
  } = useApiMethod<unknown, SendReactionBody>(roomReactionApiDeclaration.send);
  const {
    process: { loading: loadingRoomReaction, error: errorRoomReaction },
  } = apiRoomReactionState;

  const {
    apiMethodState: apiGetEventState,
    fetchData: fetchRoomEvents,
  } = useApiMethod<Event[], PaginationUrlParams>(eventApiDeclaration.get);
  const {
    process: { loading: loadingRoomEvent, error: errorRoomEvent },
    data: events,
  } = apiGetEventState;

  const {
    apiMethodState: apiSendEventState,
    fetchData: sendRoomEvent,
  } = useApiMethod<unknown, SendEventBody>(roomsApiDeclaration.sendEvent);
  const {
    process: { loading: loadingSendRoomEvent, error: errorSendRoomEvent },
  } = apiSendEventState;

  const reactionsSafe = reactions || [];
  const additionalReactionsLike = useAdditionalReactions({
    reactions: reactionsSafe,
    eventTypeAdditionalNames: {
      Like: ['like1', 'like2'],
    },
  });
  const additionalReactionsDisLike = useAdditionalReactions({
    reactions: reactionsSafe,
    eventTypeAdditionalNames: {
      Dislike: [
        'dislike1',
        'dislike2',
        'dislike3',
        'dislike4',
        'dislike5',
        'dislike6',
        'dislike7',
        'dislike8',
        'dislike9',
        'dislike10',
        'dislike11',
      ],
    },
  });

  const eventsReationsFiltered =
    !events ?
      [] :
      events
        .filter(event =>
          event.roles.some(role => roles.includes(role)) &&
          event.participantTypes.includes(participantType)
        )
        .map(eventToReaction);

  useEffect(() => {
    fetchReactions({
      PageSize: reactionsPageSize,
      PageNumber: reactionsPageNumber,
    });
    fetchRoomEvents({
      PageSize: reactionsPageSize,
      PageNumber: reactionsPageNumber,
    });
  }, [fetchReactions, fetchRoomEvents]);

  const handleReactionClick = useCallback((reaction: Reaction) => {
    if (!room) {
      throw new Error('Error sending reaction. Room not found.');
    }
    sendRoomReaction({
      reactionId: reaction.id,
      roomId: room.id,
      payload: reaction.type.name,
    });
  }, [room, sendRoomReaction]);

  const handleEventClick = useCallback((event: Reaction) => {
    if (!room) {
      throw new Error('Error sending reaction. Room not found.');
    }
    sendRoomEvent({
      roomId: room.id,
      type: event.type.name,
    });
  }, [room, sendRoomEvent]);

  if (errorReactions) {
    return (
      <div>{Captions.ReactionsLoadingError}: {errorReactions}</div>
    );
  }
  if (loadingReactions) {
    return (
      <Loader />
    );
  }

  return (
    <div>
      <div className="reaction-wrapper">
        <span>{Captions.LikeReactions}</span>
        <ReactionsList
          sortOrder={-1}
          reactions={additionalReactionsLike}
          onClick={handleReactionClick}
        />
      </div>
      <div className="reaction-wrapper">
        <span>{Captions.DislikeReactions}</span>
        <ReactionsList
          sortOrder={-1}
          reactions={additionalReactionsDisLike}
          onClick={handleReactionClick}
        />
      </div>
      <div className="reaction-wrapper">
        <span>{Captions.Events}</span>
        <ReactionsList
          sortOrder={1}
          reactions={eventsReationsFiltered}
          onClick={handleEventClick}
        />
      </div>
      {loadingRoomReaction && <div>{Captions.SendingReaction}...</div>}
      {errorRoomReaction && <div>{Captions.ErrorSendingReaction}</div>}
      {loadingRoomEvent && <div>{Captions.GetRoomEvent}...</div>}
      {errorRoomEvent && <div>{Captions.ErrorGetRoomEvent}</div>}
      {loadingSendRoomEvent && <div>{Captions.SendingRoomEvent}...</div>}
      {errorSendRoomEvent && <div>{Captions.ErrorSendingRoomEvent}</div>}
    </div>
  );
};
