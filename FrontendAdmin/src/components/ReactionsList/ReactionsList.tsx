import React, { FunctionComponent, useCallback } from 'react';
import { Reaction } from '../../types/reaction';

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
          {reaction.type.name}
        </button>
      ))}
    </div>
  );
};
