import { FunctionComponent, useCallback, useEffect } from 'react';
import { Captions } from '../../../../constants';
import { ReactionsList } from '../../../../components/ReactionsList/ReactionsList';
import { useApiMethod } from '../../../../hooks/useApiMethod';
import { Reaction } from '../../../../types/reaction';
import { reactionsApiDeclaration, roomReactionApiDeclaration, roomsApiDeclaration } from '../../../../apiDeclarations';
import { Room } from '../../../../types/room';
import { Loader } from '../../../../components/Loader/Loader';

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
};
