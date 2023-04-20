import React, { FunctionComponent, useEffect, useState } from 'react';
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

interface FlatQuestionReaction {
  type: string;
  count: number;
}

interface FlatQuestion {
  id: string;
  value: string;
  status: string;
  reactions: FlatQuestionReaction[];
}

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

  const [flatQuestions, setFlatQuestions] = useState<FlatQuestion[]>([]);
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
    const newFlatQuestions = data.questions.reduce((acc: FlatQuestion[], question) => {
      const questionReactions = new Map<string, number>();
      question.users.forEach(questionUser => {
        questionUser.reactionsSummary.forEach(userReactionsSummary => {
          const prevCount = questionReactions.get(userReactionsSummary.type) || 0;
          const newCount = prevCount + userReactionsSummary.count;
          questionReactions.set(userReactionsSummary.type, newCount);
        });
      });
      const reactions = Array.from(questionReactions.entries()).map(
        questionReaction => ({ type: questionReaction[0], count: questionReaction[1] })
      );
      return [...acc, { ...question, reactions }];
    }, []);
    setFlatQuestions(newFlatQuestions);
  }, [data]);

  useEffect(() => {
    if (!data?.reactions) {
      return;
    }
    const getReactionByType = (reactionType: string) => data.reactions.find(reaction => reaction.type === reactionType);
    const likeReaction = getReactionByType('Like');
    const dislikeReaction = getReactionByType('Dislike');
    if (!likeReaction || !dislikeReaction) {
      setTotalMarkError('Failed to calculate total mark');
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
        title="Официальный отчет заседания"
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
              {displayedReactions.map((reaction, reactionIndex) => (
                <th key={reaction}>{displayedReactionsView[reactionIndex]}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {flatQuestions.map(question => (
              <tr key={question.id}>
                <td>{question.value}</td>
                {displayedReactions.map(displayedReaction =>
                  question.reactions
                    .filter(reaction => reaction.type === displayedReaction)
                    .map(reaction => (
                      <td key={`${question.id}${reaction.type}`}>
                        {reaction.count}
                      </td>
                    )))}
              </tr>
            ))}
          </tbody>
        </table>
      </Field>
    </MainContentWrapper>
  );
};
