import React, { FunctionComponent, useCallback } from 'react';
import { Reaction } from '../../types/reaction';

import './ReactionsList.css';

const reactionNameReplaces: Record<string, string> = {
  Like: '👍',
  Dislike: '👎',
}

interface ReactionsListProps {
  reactions: Reaction[];
  sortOrder: 1 | -1;
  onClick: (reaction: Reaction) => void;
}

export const ReactionsList: FunctionComponent<ReactionsListProps> = ({
  reactions,
  sortOrder,
  onClick,
}) => {
  const handleReactionClick = useCallback((reaction: Reaction) => () => {
    onClick(reaction);
  }, [onClick]);

  return (
    <div className='reactions-list'>
      {reactions
        .sort((reaction1, reaction2) => {
          if (reaction1.type.name > reaction2.type.name) {
            return 1 * sortOrder;
          }
          if (reaction1.type.name < reaction2.type.name) {
            return -1 * sortOrder;
          }
          return 0;
        })
        .map(reaction => (
          <button
            key={reaction.id}
            className='reaction'
            onClick={handleReactionClick(reaction)}
          >
            {reactionNameReplaces[reaction.type.name] || reaction.type.name}
          </button>
        ))}
    </div>
  );
};
