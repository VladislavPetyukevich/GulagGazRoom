import React, { Fragment, FunctionComponent, useEffect, useState } from 'react';
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

import './RoomAnayticsSummary.css';

export const RoomAnayticsSummary: FunctionComponent = () => {
  let { id } = useParams();
  const { apiMethodState, fetchData } = useApiMethod<AnalyticsSummary>();
  const { data, process: { loading, error } } = apiMethodState;

  const {
    apiMethodState: roomApiMethodState,
    fetchData: fetchRoom,
  } = useApiMethod<RoomType>();
  const {
    process: { loading: roomLoading, error: roomError },
    data: room,
  } = roomApiMethodState;

  const displayedReactions = ['Like', 'Dislike'];
  const displayedReactionsView = [Captions.LikeTable, Captions.DislikeTable];
  const [totalLikesDislikes, setTotalLikesDislikes] = useState({ likes: 0, dislikes: 0 });
  const [totalMarkError, setTotalMarkError] = useState('');

  useEffect(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchData(roomsApiDeclaration.analyticsSummary(id));
    fetchRoom(roomsApiDeclaration.getById(id));
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

  if (loading || roomLoading) {
    return (
      <div>Loading...</div>
    );
  }

  if (error || roomError) {
    return (
      <div>Error: {error}</div>
    );
  }

  return (
    <MainContentWrapper className="room-anaytics-summary">
      <HeaderWithLink
        title={Captions.RoomAnayticsSummary}
        linkVisible={true}
        path={pathnames.rooms}
        linkCaption="<"
        linkFloat="left"
      />
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
        <table>
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
    </MainContentWrapper>
  );
};
