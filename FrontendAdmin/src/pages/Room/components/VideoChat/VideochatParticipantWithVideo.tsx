import { FunctionComponent, ReactNode } from 'react';
import { UserAvatar } from '../../../../components/UserAvatar/UserAvatar';

interface VideochatParticipantWithVideoProps {
  order?: number;
  children: ReactNode;
  avatar?: string;
  nickname?: string;
  videoTrackEnabled?: boolean;
}

export const VideochatParticipantWithVideo: FunctionComponent<VideochatParticipantWithVideoProps> = ({
  order,
  children,
  avatar,
  nickname,
  videoTrackEnabled,
}) => {
  const orderSafe = order || 2;
  return (
    <div
      className={`videochat-participant ${orderSafe === 1 ? 'videochat-participant-big' : 'videochat-participant'}`}
      style={{ order: orderSafe }}
    >
      <div className='videochat-caption videochat-overlay videochat-participant-name'>
        {avatar && (
          <UserAvatar
            src={avatar}
            nickname={nickname || ''}
          />
        )}
        {nickname}
      </div>
      {videoTrackEnabled === false && (
        avatar ? (
          <div className='avatar-wrapper-no-video'>
            <UserAvatar
              src={avatar}
              nickname={nickname || ''}
            />
          </div>
        ) : (
          <div>NO VIDEO</div>
        )
      )}
      <div style={{ ...(videoTrackEnabled === false && { display: 'none' }) }}>
        {children}
      </div>
    </div>
  );
};
