import React, { Fragment, FunctionComponent, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { HeaderWithLink } from '../../components/HeaderWithLink/HeaderWithLink';
import { Captions, pathnames } from '../../constants';
import { Field } from '../../components/FieldsBlock/Field';
import { useApiMethod } from '../../hooks/useApiMethod';
import { roomsApiDeclaration } from '../../apiDeclarations';
import { AnalyticsSummary } from '../../types/analytics';
import { Room as RoomType } from '../../types/room';
import { SovietMark } from '../../components/SovietMark/SovietMark';

import './RoomAnayticsSummary.css';

export const RoomAnayticsSummary: FunctionComponent = () => {
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

  const displayedReactions = ['Like', 'Dislike'];
  const displayedReactionsView = [Captions.LikeTable, Captions.DislikeTable];
  const [totalLikesDislikes, setTotalLikesDislikes] = useState({ likes: 0, dislikes: 0 });
  const [totalMarkError, setTotalMarkError] = useState('');

  useEffect(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchData(id);
    fetchRoom(id);
  }, [id, fetchData, fetchRoom]);

  useEffect(() => {
    if (!data?.reactions) {
      return;
    }
    const getReactionByType = (reactionType: string) => data.reactions.find(reaction => reaction.type === reactionType);
    const likeReaction = getReactionByType('Like');
    const dislikeReaction = getReactionByType('Dislike');
    if (!likeReaction || !dislikeReaction) {
      setTotalMarkError(Captions.FailedToCalculateMark);
      return;
    }
    setTotalLikesDislikes({
      likes: likeReaction.count,
      dislikes: likeReaction.count,
    });
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
                {question.users && question.users.map(user => (
                  <tr key={user.id} className="user-row">
                    <td>{user.nickname}</td>
                    {displayedReactions.map(displayedReaction => (
                      <td key={displayedReaction}>
                        {user.reactionsSummary.find(
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
