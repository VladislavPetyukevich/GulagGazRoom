import React, { Fragment, FunctionComponent, useCallback, useContext, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { HeaderWithLink } from '../../components/HeaderWithLink/HeaderWithLink';
import { Captions, pathnames } from '../../constants';
import { Field } from '../../components/FieldsBlock/Field';
import { useApiMethod } from '../../hooks/useApiMethod';
import { roomsApiDeclaration } from '../../apiDeclarations';
import { AnalyticsQuestionsExpert, AnalyticsSummary } from '../../types/analytics';
import { Room as RoomType } from '../../types/room';
import { SovietMark } from '../../components/SovietMark/SovietMark';
import { ProcessWrapper } from '../../components/ProcessWrapper/ProcessWrapper';
import { RoomReviews } from './components/RoomReviews/RoomReviews';
import { AuthContext } from '../../context/AuthContext';
import { checkAdmin } from '../../utils/checkAdmin';
import { RoomActionModal } from '../Room/components/RoomActionModal/RoomActionModal';

import './RoomAnayticsSummary.css';

export const RoomAnayticsSummary: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const admin = checkAdmin(auth);
  let { id } = useParams();
  const { apiMethodState, fetchData } = useApiMethod<AnalyticsSummary, RoomType['id']>(roomsApiDeclaration.analyticsSummary);
  const { data, process: { loading, error } } = apiMethodState;

  const {
    apiMethodState: roomApiMethodState,
    fetchData: fetchRoom,
  } = useApiMethod<RoomType, RoomType['id']>(roomsApiDeclaration.getById);
  const {
    process: { loading: roomLoading, error: roomError },
    data: room,
  } = roomApiMethodState;

  const {
    apiMethodState: apiRoomCloseMethodState,
    fetchData: fetchRoomClose,
  } = useApiMethod<unknown, RoomType['id']>(roomsApiDeclaration.close);
  const {
    process: { loading: roomCloseLoading, error: roomCloseError },
  } = apiRoomCloseMethodState;

  const displayedReactions = ['Like', 'Dislike'];
  const displayedReactionsView = [Captions.LikeTable, Captions.DislikeTable];
  const [totalLikesDislikes, setTotalLikesDislikes] = useState({ likes: 0, dislikes: 0 });
  const [totalMarkError, setTotalMarkError] = useState('');
  const loaders = [
    {},
    { height: '3.5rem' },
    {},
  ];

  useEffect(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchData(id);
    fetchRoom(id);
  }, [id, fetchData, fetchRoom]);

  useEffect(() => {
    if (!data?.questions) {
      return;
    }
    const getExpertReactionsCount = (expert: AnalyticsQuestionsExpert, reactionType: string) =>
      expert.reactionsSummary.find(reaction => reaction.type === reactionType)?.count || 0;

    const expertReactionsSummary = data.questions.reduce((totalAcc, question) => {
      if (!question.experts) {
        return { ...totalAcc };
      }
      const expertSummary = question.experts.reduce((expertAcc, expert) => ({
        likes: expertAcc.likes + getExpertReactionsCount(expert, 'Like'),
        dislikes: expertAcc.dislikes + getExpertReactionsCount(expert, 'Dislike'),
      }), { likes: 0, dislikes: 0 });
      return {
        likes: totalAcc.likes + expertSummary.likes,
        dislikes: totalAcc.dislikes + expertSummary.dislikes,
      }
    }, { likes: 0, dislikes: 0 });
    if (!expertReactionsSummary.likes || !expertReactionsSummary.dislikes) {
      setTotalMarkError(Captions.FailedToCalculateMark);
      return;
    }
    setTotalLikesDislikes(expertReactionsSummary);
  }, [data]);

  const handleCloseRoom = useCallback(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchRoomClose(id);
  }, [id, fetchRoomClose]);

  return (
    <MainContentWrapper className="room-anaytics-summary">
      <HeaderWithLink
        title={Captions.RoomAnayticsSummary}
        linkVisible={true}
        path={pathnames.rooms}
        linkCaption="<"
        linkFloat="left"
      />
      <ProcessWrapper
        loading={loading || roomLoading}
        error={error || roomError}
        loaders={loaders}
      >
        <>
        {admin && (
            <Field>
              <RoomActionModal
                title={Captions.CloseRoomModalTitle}
                openButtonCaption={Captions.CloseRoom}
                loading={roomCloseLoading}
                error={roomCloseError}
                onAction={handleCloseRoom}
              />
            </Field>
          )}
          <Field>
            <h3>{room?.name}</h3>
          </Field>
          <Field>
            <h3>Чёткость ответа:</h3>
            <div>
              {totalMarkError ? totalMarkError : (<SovietMark {...totalLikesDislikes} />)}
            </div>
          </Field>
          <Field>
            <h3>{Captions.QuestionsSummary}:</h3>
            <table className='anaytics-table'>
              <thead>
                <tr>
                  <th>{Captions.Question}</th>
                  <th></th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {data?.questions.map(question => (
                  <Fragment key={question.id}>
                    <tr>
                      <td className="question-cell">
                        {question.value}
                      </td>
                      {displayedReactions.map((reaction, reactionIndex) => (
                        <td key={reaction}>{displayedReactionsView[reactionIndex]}</td>
                      ))}
                    </tr>
                    {question.experts && question.experts.map(expert => (
                      <tr key={`${question.id}${expert.id}`} className="user-row">
                        <td>{expert.nickname}</td>
                        {displayedReactions.map(displayedReaction => (
                          <td key={`expert-${displayedReaction}`}>
                            {expert.reactionsSummary.find(
                              reactionSummary => reactionSummary.type === displayedReaction
                            )?.count || 0}
                          </td>
                        ))}
                      </tr>
                    ))}
                    {question.viewers && question.viewers.map(viewer => (
                      <tr key={`${question.id}-viewer`} className="user-row">
                        <td>{Captions.Viewers}</td>
                        {displayedReactions.map(displayedReaction => (
                          <td key={`viewer-${displayedReaction}`}>
                            {viewer.reactionsSummary.find(
                              reactionSummary => reactionSummary.type === displayedReaction
                            )?.count || 0}
                          </td>
                        ))}
                      </tr>
                    ))}
                  </Fragment>
                ))}
              </tbody>
            </table>
          </Field>
          <RoomReviews roomId={id || ''} />
        </>
      </ProcessWrapper>
    </MainContentWrapper>
  );
};
