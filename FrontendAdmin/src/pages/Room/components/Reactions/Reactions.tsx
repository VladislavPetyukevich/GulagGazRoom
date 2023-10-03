import { FunctionComponent, useCallback, useEffect } from 'react';
import { Captions } from '../../../../constants';
import { ReactionsList } from '../../../../components/ReactionsList/ReactionsList';
import { useApiMethod } from '../../../../hooks/useApiMethod';
import { Reaction } from '../../../../types/reaction';
import { PaginationUrlParams, SendGasBody, SendReactionBody, reactionsApiDeclaration, roomReactionApiDeclaration, roomsApiDeclaration } from '../../../../apiDeclarations';
import { Room } from '../../../../types/room';
import { Loader } from '../../../../components/Loader/Loader';
import { useAdditionalReactions } from '../../hooks/useAdditionalReactions';

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
    name: `${Captions.On} 🤿`,
    value: 0,
  }
}, {
  id: 'gasReactionOffId',
  type: {
    eventType: 'GasOff',
    name: `${Captions.Off} 👌`,
    value: 0,
  }
}];

export interface ReactionsProps {
  room: Room | null;
  admin: boolean;
}

export const Reactions: FunctionComponent<ReactionsProps> = ({
  room,
  admin,
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
    apiMethodState: apiSendGasState,
    fetchData: sendRoomGas,
  } = useApiMethod<unknown, SendGasBody>(roomsApiDeclaration.sendGasEvent);
  const {
    process: { loading: loadingRoomGas, error: errorRoomGas },
  } = apiSendGasState;

  const reactionsSafe = reactions || [];
  const additionalReactionsLike = useAdditionalReactions({
    reactions: reactionsSafe,
    eventTypeAdditionalNames: {
      ReactionLike: ['like1', 'like2'],
    },
  });
  const additionalReactionsDisLike = useAdditionalReactions({
    reactions: reactionsSafe,
    eventTypeAdditionalNames: {
      ReactionDislike: [
        'dislike1',
        'dislike2',
        'dislike3',
        'dislike4',
        'dislike5',
        // 'dislike6',
        // 'dislike7',
        // 'dislike8',
        // 'dislike9',
        // 'dislike10',
        // 'dislike11',
      ],
    },
  });

  useEffect(() => {
    fetchReactions({
      PageSize: reactionsPageSize,
      PageNumber: reactionsPageNumber,
    });
  }, [fetchReactions]);

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

  const handleGasReactionClick = useCallback((reaction: Reaction) => {
    if (!room) {
      throw new Error('Error sending reaction. Room not found.');
    }
    sendRoomGas({
      roomId: room.id,
      type: (reaction as GasReaction).type.eventType,
    });
  }, [room, sendRoomGas]);

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
};
