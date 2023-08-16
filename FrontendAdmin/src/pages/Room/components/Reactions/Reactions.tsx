import { FunctionComponent, useCallback, useEffect } from 'react';
import { Captions } from '../../../../constants';
import { ReactionsList } from '../../../../components/ReactionsList/ReactionsList';
import { useApiMethod } from '../../../../hooks/useApiMethod';
import { Reaction } from '../../../../types/reaction';
import { PaginationUrlParams, SendCodeEditorBody, SendGasBody, SendReactionBody, reactionsApiDeclaration, roomReactionApiDeclaration, roomsApiDeclaration } from '../../../../apiDeclarations';
import { Room } from '../../../../types/room';
import { Loader } from '../../../../components/Loader/Loader';
import { useAdditionalReactions } from '../../hooks/useAdditionalReactions';

const reactionsPageSize = 30;
const reactionsPageNumber = 1;

interface GasReaction extends Reaction {
  type: {
    id: string;
    name: 'GasOn' | 'GasOff';
    value: number;
  }
}

interface CodeEditorReaction extends Reaction {
  type: {
    id: string;
    name: 'EnableCodeEditor' | 'DisableCodeEditor';
    value: number;
  }
}

const gasReactions: GasReaction[] = [{
  id: 'gasReactionOnId',
  type: {
    id: 'GasOn',
    name: 'GasOn',
    value: 0,
  }
}, {
  id: 'gasReactionOffId',
  type: {
    id: 'GasOff',
    name: 'GasOff',
    value: 0,
  }
}];

const codeEditorReactions: CodeEditorReaction[] = [{
  id: 'codeEditorReactionOnId',
  type: {
    id: 'EnableCodeEditor',
    name: 'EnableCodeEditor',
    value: 0,
  }
}, {
  id: 'codeEditorReactionOffId',
  type: {
    id: 'DisableCodeEditor',
    name: 'DisableCodeEditor',
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

  const {
    apiMethodState: apiSendCodeEditorState,
    fetchData: sendRoomCodeEditor,
  } = useApiMethod<unknown, SendCodeEditorBody>(roomsApiDeclaration.sendCodeEditorEvent);
  const {
    process: { loading: loadingRoomCodeEditor, error: errorRoomCodeEditor },
  } = apiSendCodeEditorState;

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
            onClick={handleReactionClick}
          />
        </div>
      )}
      {admin && (
        <div className="reaction-wrapper">
          <span>{Captions.CodeEditor}:</span>
          <ReactionsList
            sortOrder={1}
            reactions={codeEditorReactions}
            onClick={handleReactionClick}
          />
        </div>
      )}
      {loadingRoomReaction && <div>{Captions.SendingReaction}...</div>}
      {errorRoomReaction && <div>{Captions.ErrorSendingReaction}</div>}
      {loadingRoomGas && <div>{Captions.SendingGasEvent}...</div>}
      {errorRoomGas && <div>{Captions.ErrorSendingGasEvent}</div>}
      {loadingRoomCodeEditor && <div>{Captions.SendingCodeEditorEvent}...</div>}
      {errorRoomCodeEditor && <div>{Captions.ErrorSendingCodeEditorEvent}</div>}
    </div>
  );
};
