import { FunctionComponent, ReactNode } from 'react';

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
          <img
            src={avatar}
            className='videochat-participant-avatar'
            alt={`${nickname} avatar`}
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
      {children}
    </div>
  );
};
