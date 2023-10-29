import { FunctionComponent, useCallback, useEffect, useState } from 'react';
import { Captions } from '../../../../constants';
import { ReactionsList } from '../../../../components/ReactionsList/ReactionsList';
import { useApiMethod } from '../../../../hooks/useApiMethod';
import { Reaction } from '../../../../types/reaction';
import {
  PaginationUrlParams,
  SendEventBody,
  SendReactionBody,
  eventApiDeclaration,
  reactionsApiDeclaration,
  roomReactionApiDeclaration,
  roomsApiDeclaration,
} from '../../../../apiDeclarations';
import { Room, RoomState, RoomStateAdditionalStatefulPayload } from '../../../../types/room';
import { Event } from '../../../../types/event';
import { UserType } from '../../../../types/user';
import { Loader } from '../../../../components/Loader/Loader';
import { useAdditionalReactions } from '../../hooks/useAdditionalReactions';

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

type ParsedStates = Record<string, boolean>;

export interface ReactionsProps {
  room: Room | null;
  roles: string[];
  participantType: UserType | null;
  lastWsMessage: MessageEvent<any> | null;
}

export const Reactions: FunctionComponent<ReactionsProps> = ({
  room,
  roles,
  participantType,
  lastWsMessage,
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
    apiMethodState: apiRoomStateState,
    fetchData: fetchRoomState,
  } = useApiMethod<RoomState, Room['id']>(roomsApiDeclaration.getState);
  const {
    process: { loading: loadingRoomState, error: errorRoomState },
    data: roomState,
  } = apiRoomStateState;

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

  const [parsedStates, setParsedStates] = useState<ParsedStates>({});

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
        'dislike4',
        'dislike5',
        'dislike6',
        'dislike8',
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
          participantType &&
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
    if (room?.id) {
      fetchRoomState(room.id);
    }
  }, [room?.id, fetchRoomState, fetchReactions, fetchRoomEvents]);

  useEffect(() => {
    if(!roomState) {
      return;
    }
    const parsedStates: ParsedStates = {};
    roomState.states.forEach(roomState =>
      parsedStates[roomState.type] = (JSON.parse(roomState.payload) as RoomStateAdditionalStatefulPayload).enabled
    );
    setParsedStates(parsedStates);
  }, [roomState]);

  useEffect(() => {
    if (!lastWsMessage || !parsedStates) {
      return;
    }
    try {
      const parsedData = JSON.parse(lastWsMessage?.data);
      if (!parsedData?.Stateful) {
        return;
      }
      const stateType = parsedData.Type;
      const stateValue = (parsedData.Value.AdditionalData as RoomStateAdditionalStatefulPayload).enabled;
      const oldValue = parsedStates[stateType];
      if (oldValue !== stateValue) {
        setParsedStates({
          ...parsedStates,
          [stateType]: stateValue,
        });
      }
    } catch {
    }
  }, [lastWsMessage, parsedStates]);

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
    if (!room || !parsedStates) {
      throw new Error('Error sending reaction. Room not found.');
    }
    const prevEnabled = Boolean(parsedStates[event.type.name]);
    sendRoomEvent({
      roomId: room.id,
      type: event.type.name,
      additionalData: { enabled: !prevEnabled },
    });
  }, [room, parsedStates, sendRoomEvent]);

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
      {loadingRoomState && <div>{Captions.LoadingRoomState}...</div>}
      {errorRoomState && <div>{Captions.ErrorLoadingRoomState}...</div>}
      {loadingRoomReaction && <div>{Captions.SendingReaction}...</div>}
      {errorRoomReaction && <div>{Captions.ErrorSendingReaction}</div>}
      {loadingRoomEvent && <div>{Captions.GetRoomEvent}...</div>}
      {errorRoomEvent && <div>{Captions.ErrorGetRoomEvent}</div>}
      {loadingSendRoomEvent && <div>{Captions.SendingRoomEvent}...</div>}
      {errorSendRoomEvent && <div>{Captions.ErrorSendingRoomEvent}</div>}
    </div>
  );
};
