import React, { FunctionComponent, useCallback } from 'react';
import { Reaction } from '../../types/reaction';

const reactionNameReplaces: Record<string, string> = {
  Like: '👍',
  Dislike: '👎',
}

interface ReactionsListProps {
  reactions: Reaction[];
  onClick: (reaction: Reaction) => void;
}

export const ReactionsList: FunctionComponent<ReactionsListProps> = ({
  reactions,
  onClick,
}) => {
  const handleReactionClick = useCallback((reaction: Reaction) => () => {
    onClick(reaction);
  }, [onClick]);

  return (
    <div>
      {reactions
        .sort((reaction1, reaction2) => {
          if (reaction1.type.name > reaction2.type.name) {
            return -1;
          }
          if (reaction1.type.name < reaction2.type.name) {
            return 1;
          }
          return 0;
        })
        .map(reaction => (
          <button
            key={reaction.id}
            onClick={handleReactionClick(reaction)}
          >
            {reactionNameReplaces[reaction.type.name] || reaction.type.name}
          </button>
        ))}
    </div>
  );
};
