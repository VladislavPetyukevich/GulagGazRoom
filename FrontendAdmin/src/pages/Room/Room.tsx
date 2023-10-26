import { FunctionComponent, useCallback, useContext, useEffect, useState } from 'react';
import { useParams, Navigate } from 'react-router-dom';
import useWebSocket from 'react-use-websocket';
import toast from 'react-hot-toast';
import {
  GetRoomParticipantParams,
  GetRoomQuestionsBody,
  roomQuestionApiDeclaration,
  roomsApiDeclaration,
} from '../../apiDeclarations';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { REACT_APP_WS_URL } from '../../config';
import { Captions, pathnames } from '../../constants';
import { AuthContext } from '../../context/AuthContext';
import { useApiMethod } from '../../hooks/useApiMethod';
import { useCommunist } from '../../hooks/useCommunist';
import { RoomParticipant, Room as RoomType } from '../../types/room';
import { RoomActionModal } from './components/RoomActionModal/RoomActionModal';
import { Reactions } from './components/Reactions/Reactions';
import { ActiveQuestion } from './components/ActiveQuestion/ActiveQuestion';
import { ProcessWrapper } from '../../components/ProcessWrapper/ProcessWrapper';
import { Question } from '../../types/question';
import { VideoChat } from './components/VideoChat/VideoChat';

import './Room.css';

const getWsStatusMessage = (readyState: number) => {
  switch (readyState) {
    case 0: return 'WS CONNECTING';
    case 2: return 'WS CLOSING';
    case 3: return 'WS CLOSED';
    default: return null;
  }
};

export const Room: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const { getCommunist } = useCommunist();
  const communist = getCommunist();
  let { id } = useParams();
  const socketUrl = `${REACT_APP_WS_URL}/ws?Authorization=${communist}&roomId=${id}`;
  const { lastMessage, readyState, sendMessage } = useWebSocket(socketUrl);
  const [roomInReview, setRoomInReview] = useState(false);
  const [reactionsVisible, setReactionsVisible] = useState(false);
  const [currentQuestion, setCurrentQuestion] = useState<Question>();

  const { apiMethodState, fetchData } = useApiMethod<RoomType, RoomType['id']>(roomsApiDeclaration.getById);
  const { process: { loading, error }, data: room } = apiMethodState;

  const {
    apiMethodState: apiRoomStartReviewMethodState,
    fetchData: fetchRoomStartReview,
  } = useApiMethod<unknown, RoomType['id']>(roomsApiDeclaration.startReview);
  const {
    process: { loading: roomStartReviewLoading, error: roomStartReviewError },
  } = apiRoomStartReviewMethodState;

  const {
    apiMethodState: apiOpenRoomQuestions,
    fetchData: getRoomOpenQuestions,
  } = useApiMethod<Array<Question['id']>, GetRoomQuestionsBody>(roomQuestionApiDeclaration.getRoomQuestions);
  const {
    data: openRoomQuestions,
  } = apiOpenRoomQuestions;

  const {
    apiMethodState: apiRoomParticipantState,
    fetchData: getRoomParticipant,
  } = useApiMethod<RoomParticipant, GetRoomParticipantParams>(roomsApiDeclaration.getParticipant);
  const {
    data: roomParticipant,
  } = apiRoomParticipantState;

  const currentUserExpert = roomParticipant?.userType === 'Expert';
  const currentUserExaminee = roomParticipant?.userType === 'Examinee';
  const viewerMode = !(currentUserExpert || currentUserExaminee);

  useEffect(() => {
    if (!auth?.id) {
      return;
    }
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchData(id);
    getRoomOpenQuestions({
      RoomId: id,
      State: 'Active',
    });
    getRoomParticipant({
      RoomId: id,
      UserId: auth.id,
    });
  }, [id, auth?.id, fetchData, getRoomOpenQuestions, getRoomParticipant]);

  useEffect(() => {
    if (!room) {
      return;
    }
    if (room.roomStatus !== 'New') {
      setReactionsVisible(true);
    }
  }, [room]);

  useEffect(() => {
    if (!room || !openRoomQuestions || !openRoomQuestions[0]) {
      return;
    }
    const openQuestion = room.questions.find(roomQ => roomQ.id === openRoomQuestions[0])
    if (!openQuestion) {
      return;
    }
    setCurrentQuestion(openQuestion);
  }, [room, openRoomQuestions]);

  useEffect(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    if (!lastMessage || !auth?.nickname) {
      return;
    }
    try {
      const parsedData = JSON.parse(lastMessage?.data);
      console.log('parsedData: ', parsedData);
      switch (parsedData?.Type) {
        case 'ChatMessage':
          const message = parsedData?.Value?.Message;
          const nickname = parsedData?.Value?.Nickname;
          if (typeof message !== 'string') {
            return;
          }
          if (message.includes(auth.nickname)) {
            toast(`${nickname}: ${message}`, { icon: '💬' });
          }
          break;
        case 'ChangeRoomStatus':
          const newStatus: RoomType['roomStatus'] = 'New';
          const reviewStatus: RoomType['roomStatus'] = 'Review';
          if (parsedData?.Value === reviewStatus) {
            setRoomInReview(true);
          }
          if (parsedData?.Value !== newStatus) {
            setReactionsVisible(true);
          }
          break;
        case 'ChangeRoomQuestionState':
          if (parsedData.Value.NewState !== 'Active') {
            break;
          }
          getRoomOpenQuestions({
            RoomId: id,
            State: 'Open',
          });
          break;
        default:
          break;
      }
    } catch { }
  }, [id, auth, lastMessage, getRoomOpenQuestions]);

  const handleCopyRoomLink = () =>
    navigator.clipboard.writeText(window.location.href);

  const handleStartReviewRoom = useCallback(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchRoomStartReview(id);
  }, [id, fetchRoomStartReview]);

  const loaders = [
    {},
    {},
    { height: '890px' }
  ];

  if (roomInReview && id) {
    return <Navigate to={pathnames.roomAnalyticsSummary.replace(':id', id)} replace />;
  }

  return (
    <MainContentWrapper className="room-page">
      <ProcessWrapper
        loading={loading}
        loadingPrefix={Captions.LoadingRoom}
        loaders={loaders}
        error={error}
        errorPrefix={Captions.ErrorLoadingRoom}
      >
        <>
          <Field className='room-title'>
            <h2>{Captions.Room}: {room?.name}</h2>
            {!viewerMode && (
              <button
                className="copy-link-button"
                onClick={handleCopyRoomLink}
              >
                {Captions.CopyRoomLink}
              </button>
            )}
          </Field>
          {!viewerMode && (
            <Field>
              <RoomActionModal
                title={Captions.StartReviewRoomModalTitle}
                openButtonCaption={Captions.StartReviewRoom}
                loading={roomStartReviewLoading}
                error={roomStartReviewError}
                onAction={handleStartReviewRoom}
              />
            </Field>
          )}
          <Field>
            <div className="reactions-field">
              {viewerMode && (
                <Field>
                  <div>{Captions.ActiveQuestion}: {currentQuestion?.value}</div>
                </Field>
              )}
              {!viewerMode && (
                <ActiveQuestion
                  room={room}
                  placeHolder={currentQuestion?.value}
                />
              )}
              {reactionsVisible && (
                <Reactions
                  room={room}
                  roles={auth?.roles || []}
                  participantType={roomParticipant?.userType || null}
                />
              )}
              {!reactionsVisible && (
                <div>{Captions.WaitingRoom}</div>
              )}
            </div>
            {getWsStatusMessage(readyState) || (
              <VideoChat
                roomId={room?.id || ''}
                viewerMode={viewerMode}
                lastWsMessage={lastMessage}
                onSendWsMessage={sendMessage}
              />
            )}
          </Field>
        </>
      </ProcessWrapper>
    </MainContentWrapper>
  );
};
