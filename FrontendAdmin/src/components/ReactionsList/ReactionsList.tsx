import React, { FunctionComponent, useCallback } from 'react';
import { Reaction } from '../../types/reaction';
import { SwitchButton } from '../../pages/Room/components/VideoChat/SwitchButton';
import { IconNames, reactionIcon } from '../../constants';

import './ReactionsList.css';

const defaultIconName = IconNames.None;

interface ReactionsListProps {
  reactions: Reaction[];
  loadingReactionName?: string | null;
  sortOrder: 1 | -1;
  onClick: (reaction: Reaction) => void;
}

export const ReactionsList: FunctionComponent<ReactionsListProps> = ({
  reactions,
  loadingReactionName,
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
          <SwitchButton
            key={`${reaction.id}${reaction.type.name}`}
            enabled={true}
            loading={reaction.type.name === loadingReactionName}
            iconEnabledName={reactionIcon[reaction.type.name] || defaultIconName}
            iconDisabledName={reactionIcon[reaction.type.name] || defaultIconName}
            subCaption={reaction.type.name}
            onClick={handleReactionClick(reaction)}
          />
        ))}
    </div>
  );
};
