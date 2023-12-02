import { FunctionComponent, MouseEventHandler, useCallback, useEffect, useState } from 'react';
import { Captions } from '../../../../constants';
import { ActiveQuestionSelector } from '../../../../components/ActiveQuestionSelector/ActiveQuestionSelector';
import { Room } from '../../../../types/room';
import { useApiMethod } from '../../../../hooks/useApiMethod';
import { Question } from '../../../../types/question';
import { ChangeActiveQuestionBody, GetRoomQuestionsBody, roomQuestionApiDeclaration } from '../../../../apiDeclarations';

import './ActiveQuestion.css';

export interface ActiveQuestionProps {
  room: Room | null;
  initialQuestionText?: string;
}

export const ActiveQuestion: FunctionComponent<ActiveQuestionProps> = ({
  room,
  initialQuestionText,
}) => {
  const [showClosedQuestions, setShowClosedQuestions] = useState(false);

  const {
    apiMethodState: apiOpenRoomQuestions,
    fetchData: getRoomOpenQuestions,
  } = useApiMethod<Array<Question['id']>, GetRoomQuestionsBody>(roomQuestionApiDeclaration.getRoomQuestions);
  const {
    data: openRoomQuestions,
  } = apiOpenRoomQuestions;

  const {
    apiMethodState: apiSendActiveQuestionState,
    fetchData: sendRoomActiveQuestion,
  } = useApiMethod<unknown, ChangeActiveQuestionBody>(roomQuestionApiDeclaration.changeActiveQuestion);
  const {
    process: { loading: loadingRoomActiveQuestion, error: errorRoomActiveQuestion },
  } = apiSendActiveQuestionState;

  useEffect(() => {
    if (!room?.id) {
      return;
    }
    getRoomOpenQuestions({
      RoomId: room.id,
      State: 'Open',
    })
  }, [room, getRoomOpenQuestions]);

  const handleShowClosedQuestions: MouseEventHandler<HTMLInputElement> = useCallback((e) => {
    setShowClosedQuestions(e.currentTarget.checked);
  }, []);

  const handleQuestionSelect = useCallback((question: Question) => {
    if (!room) {
      throw new Error('Error sending reaction. Room not found.');
    }
    sendRoomActiveQuestion({
      roomId: room.id,
      questionId: question.id,
    });
  }, [room, sendRoomActiveQuestion]);

  return (
    <div className='active-question-container'>
      <ActiveQuestionSelector
        showClosedQuestions={showClosedQuestions}
        questions={room?.questions || []}
        openQuestions={openRoomQuestions || []}
        initialQuestionText={initialQuestionText}
        placeHolder={Captions.SelectActiveQuestion}
        onSelect={handleQuestionSelect}
        onShowClosedQuestions={handleShowClosedQuestions}
      />
      {loadingRoomActiveQuestion && <div>{Captions.SendingActiveQuestion}...</div>}
      {errorRoomActiveQuestion && <div>{Captions.ErrorSendingActiveQuestion}...</div>}
    </div>
  );
};
