import { FunctionComponent, useCallback, useContext, useEffect, useState } from 'react';
import { useParams, Navigate } from 'react-router-dom';
import useWebSocket from 'react-use-websocket';
import toast from 'react-hot-toast';
import { roomsApiDeclaration } from '../../apiDeclarations';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { REACT_APP_INTERVIEW_FRONTEND_URL, REACT_APP_WS_URL } from '../../config';
import { Captions, pathnames } from '../../constants';
import { AuthContext } from '../../context/AuthContext';
import { useApiMethod } from '../../hooks/useApiMethod';
import { useCommunist } from '../../hooks/useCommunist';
import { RoomState, Room as RoomType } from '../../types/room';
import { checkAdmin } from '../../utils/checkAdmin';
import { RoomActionModal } from './components/RoomActionModal/RoomActionModal';
import { Twitch } from './components/Twitch/Twitch';
import { Interviewee } from './components/Interviewee/Interviewee';
import { Reactions } from './components/Reactions/Reactions';
import { ActiveQuestion } from './components/ActiveQuestion/ActiveQuestion';
import { ProcessWrapper } from '../../components/ProcessWrapper/ProcessWrapper';

import './Room.css';

export const Room: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const admin = checkAdmin(auth);
  const { getCommunist } = useCommunist();
  const communist = getCommunist();
  let { id } = useParams();
  const socketUrl = `${REACT_APP_WS_URL}/ws?Authorization=${communist}&roomId=${id}`;
  const { lastMessage } = useWebSocket(socketUrl);
  const [roomInReview, setRoomInReview] = useState(false);
  const [reactionsVisible, setReactionsVisible] = useState(false);

  const { apiMethodState, fetchData } = useApiMethod<RoomType, RoomType['id']>(roomsApiDeclaration.getById);
  const { process: { loading, error }, data: room } = apiMethodState;

  const { apiMethodState: apiRoomStateMethodState, fetchData: fetchRoomState } = useApiMethod<RoomState, RoomType['id']>(roomsApiDeclaration.getState);
  const { data: roomState } = apiRoomStateMethodState;

  const {
    apiMethodState: apiRoomStartReviewMethodState,
    fetchData: fetchRoomStartReview,
  } = useApiMethod<unknown, RoomType['id']>(roomsApiDeclaration.startReview);
  const {
    process: { loading: roomStartReviewLoading, error: roomStartReviewError },
  } = apiRoomStartReviewMethodState;

  useEffect(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchData(id);
    fetchRoomState(id);
  }, [id, fetchData, fetchRoomState]);

  useEffect(() => {
    if (!room) {
      return;
    }
    if (room.roomStatus !== 'New') {
      setReactionsVisible(true);
    }
  }, [room]);

  useEffect(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    if (!lastMessage || !auth?.nickname) {
      return;
    }
    try {
      const parsedData = JSON.parse(lastMessage?.data);
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
        default:
          break;
      }
    } catch { }
  }, [id, auth, lastMessage]);

  const handleCopyRoomLink = useCallback(() => {
    navigator.clipboard.writeText(
      `${REACT_APP_INTERVIEW_FRONTEND_URL}/?roomId=${id}`
    );
  }, [id]);

  const handleStartReviewRoom = useCallback(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchRoomStartReview(id);
  }, [id, fetchRoomStartReview]);

  const loaders = [
    {},
    {},
    { height: '5.25rem' },
    { height: '600px' },
    { height: '800px' }
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
            {admin && (
              <button
                className="copy-link-button"
                onClick={handleCopyRoomLink}
              >
                {Captions.CopyRoomLink}
              </button>
            )}
          </Field>
          {admin && (
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
          <Field className="reactions-field">
            {admin && (
              <ActiveQuestion
                room={room}
                placeHolder={roomState?.activeQuestion?.value || null}
                lastWebSocketMessage={lastMessage}
              />
            )}
            {reactionsVisible && (
              <Reactions
                admin={admin}
                room={room}
              />
            )}
            {!reactionsVisible && (
              <div>{Captions.WaitingRoom}</div>
            )}
          </Field>
          <Field className="twitch-embed-field">
            <Twitch
              channel={room?.twitchChannel || ''}
              autoplay={!admin}
            />
          </Field>
          <Field className={`interviewee-frame-wrapper ${admin ? 'admin' : ''}`}>
            <Interviewee
              roomId={room?.id || ''}
              fov={110}
              muted={!admin}
            />
          </Field>
        </>
      </ProcessWrapper>
    </MainContentWrapper>
  );
};
