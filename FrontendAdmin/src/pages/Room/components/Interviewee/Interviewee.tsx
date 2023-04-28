import { FunctionComponent } from 'react';
import { REACT_APP_INTERVIEW_FRONTEND_URL } from '../../../../config';

export interface IntervieweeProps {
  roomId: string;
  fov: number;
  muted: boolean;
}

export const Interviewee: FunctionComponent<IntervieweeProps> = ({
  roomId,
  fov,
  muted,
}) => {
  return (
    <iframe
      title="interviewee-client-frame"
      className="interviewee-frame"
      src={`${REACT_APP_INTERVIEW_FRONTEND_URL}/?roomId=${roomId}&${muted ? 'muted=1' : ''}&fov=${fov}`}
    >
    </iframe>
  )
};
