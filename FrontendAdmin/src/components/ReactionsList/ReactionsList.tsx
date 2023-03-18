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
      {reactions.map(reaction => (
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
