import { FunctionComponent, ReactNode } from 'react';
import { UserAvatar } from '../../../../components/UserAvatar/UserAvatar';

interface VideochatParticipantProps {
  order?: number;
  children: ReactNode;
  avatar?: string;
  nickname?: string;
  videoTrackEnabled?: boolean;
  audioTrackEnabled?: boolean;
  handleSwitchVideo?: () => void;
  handleSwitchAudio?: () => void;
}

export const VideochatParticipant: FunctionComponent<VideochatParticipantProps> = ({
  order,
  children,
  avatar,
  nickname,
  videoTrackEnabled,
  audioTrackEnabled,
  handleSwitchVideo,
  handleSwitchAudio,
}) => {
  const orderSafe = order || 2;
  return (
    <div
      className={`videochat-participant ${orderSafe === 1 ? 'videochat-participant-big' : 'videochat-participant'}`}
      style={{ order: orderSafe }}
    >
      <div className='videochat-overlay videochat-participant-name'>
        {avatar && (
          <UserAvatar
            src={avatar}
            nickname={nickname || ''}
          />
        )}
        {nickname}
      </div>
      {handleSwitchVideo && (
        <button
          className={`videochat-overlay videochat-switch-camera ${videoTrackEnabled ? '' : 'videochat-diasbled'}`}
          onClick={handleSwitchVideo}
        >
          📷
        </button>
      )}
      {handleSwitchAudio && (
        <button
          className={`videochat-overlay videochat-switch-mic ${audioTrackEnabled ? '' : 'videochat-diasbled'}`}
          onClick={handleSwitchAudio}
        >
          🎤
        </button>
      )}
      {!videoTrackEnabled && (
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
      {children}
    </div>
  );
};
