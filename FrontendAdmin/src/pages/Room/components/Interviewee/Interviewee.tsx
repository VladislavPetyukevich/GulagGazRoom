import { forwardRef } from 'react';
import { REACT_APP_INTERVIEW_FRONTEND_URL } from '../../../../config';

export interface IntervieweeProps {
  roomId: string;
}

export const Interviewee = forwardRef<HTMLIFrameElement, IntervieweeProps>((
  {
    roomId,
  },
  ref
) => {
  return (
    <iframe
      title="interviewee-client-frame"
      className="interviewee-frame"
      ref={ref}
      src={`${REACT_APP_INTERVIEW_FRONTEND_URL}/?roomId=${roomId}`}
    >
    </iframe>
  )
});
