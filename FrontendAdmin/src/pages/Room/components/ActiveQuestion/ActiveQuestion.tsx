import { FunctionComponent, MouseEventHandler, useCallback, useEffect, useState } from 'react';
import { Captions } from '../../../../constants';
import { ActiveQuestionSelector } from '../../../../components/ActiveQuestionSelector/ActiveQuestionSelector';
import { Room } from '../../../../types/room';
import { useApiMethod } from '../../../../hooks/useApiMethod';
import { Question } from '../../../../types/question';
import { ChangeActiveQuestionBody, GetRoomQuestionsBody, roomQuestionApiDeclaration } from '../../../../apiDeclarations';

export interface ActiveQuestionProps {
  room: Room | null;
  placeHolder: string | null;
  lastWebSocketMessage: MessageEvent<any> | null;
}

export const ActiveQuestion: FunctionComponent<ActiveQuestionProps> = ({
  room,
  placeHolder,
  lastWebSocketMessage,
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

  useEffect(() => {
    if (!room?.id) {
      return;
    }
    try {
      const parsedData = JSON.parse(lastWebSocketMessage?.data);
      if (parsedData.Type !== 'ChangeRoomQuestionState') {
        return;
      }
      if (parsedData.Value.NewState !== 'Active') {
        return;
      }
      getRoomOpenQuestions({
        RoomId: room.id,
        State: 'Open',
      });
    } catch { }
  }, [room, lastWebSocketMessage, getRoomOpenQuestions]);

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
    <div>
      <span>{Captions.ShowClosedQuestions}</span>
      <input type="checkbox" onClick={handleShowClosedQuestions} />
      <ActiveQuestionSelector
        showClosedQuestions={showClosedQuestions}
        questions={room?.questions || []}
        openQuestions={openRoomQuestions || []}
        placeHolder={placeHolder || Captions.SelectActiveQuestion}
        onSelect={handleQuestionSelect}
      />
      {loadingRoomActiveQuestion && <div>{Captions.SendingActiveQuestion}...</div>}
      {errorRoomActiveQuestion && <div>{Captions.ErrorSendingActiveQuestion}...</div>}
    </div>
  );
};
