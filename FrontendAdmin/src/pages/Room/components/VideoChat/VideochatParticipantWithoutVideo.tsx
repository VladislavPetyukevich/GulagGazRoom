import { FunctionComponent } from 'react';
import { UserAvatar } from '../../../../components/UserAvatar/UserAvatar';

interface VideochatParticipantWithoutVideoProps {
  order?: number;
  avatar?: string;
  nickname?: string;
}

export const VideochatParticipantWithoutVideo: FunctionComponent<VideochatParticipantWithoutVideoProps> = ({
  order,
  avatar,
  nickname,
}) => {
  const orderSafe = order || 2;
  return (
    <div
      className='videochat-participant-viewer-wrapper'
      style={{ order: orderSafe }}
    >
      <div
        className='videochat-participant-viewer'
      >
        {avatar && (
          <UserAvatar
            src={avatar}
            nickname={nickname || ''}
          />
        )}
        <div>{nickname}</div>
      </div>
    </div>
  );
};
