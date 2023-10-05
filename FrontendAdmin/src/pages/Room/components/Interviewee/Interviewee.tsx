import { FunctionComponent } from 'react';
import { REACT_APP_INTERVIEW_FRONTEND_URL } from '../../../../config';

export interface IntervieweeProps {
  roomId: string;
}

export const Interviewee: FunctionComponent<IntervieweeProps> = ({
  roomId,
}) => {
  return (
    <iframe
      title="interviewee-client-frame"
      className="interviewee-frame"
      src={`${REACT_APP_INTERVIEW_FRONTEND_URL}/?roomId=${roomId}`}
    >
    </iframe>
  )
};
