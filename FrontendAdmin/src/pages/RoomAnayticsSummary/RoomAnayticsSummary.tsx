import React, { FunctionComponent, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { HeaderWithLink } from '../../components/HeaderWithLink/HeaderWithLink';
import { pathnames } from '../../constants';
import { Field } from '../../components/FieldsBlock/Field';
import { useApiMethod } from '../../hooks/useApiMethod';
import { roomsApiDeclaration } from '../../apiDeclarations';
import { AnalyticsSummary } from '../../types/analytics';

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
  const [flatQuestions, setFlatQuestions] = useState<FlatQuestion[]>([]);

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
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchData(roomsApiDeclaration.analyticsSummary(id));
  }, [id, fetchData]);

  if (loading) {
    return (
      <div>Loading...</div>
    );
  }

  if (error) {
    return (
      <div>Error: {error}</div>
    );
  }

  return (
    <MainContentWrapper className="room-anaytics-summary">
      <HeaderWithLink
        title="RoomAnayticsSummary"
        linkVisible={true}
        path={pathnames.rooms}
        linkCaption="<"
        linkFloat="left"
      />
      <Field>
        RoomAnayticsSummary
      </Field>
      <Field>
        <div>Summary:</div>
        {data?.reactions.map(reaction => (
          <div key={reaction.id}>{reaction.type}: {reaction.count}</div>
        ))}
      </Field>
      <Field>
        <div>Questions:</div>
        {flatQuestions.map(question => (
          <div key={question.id}>
            <span>{question.value}&emsp;</span>
            {question.reactions.map(reaction => (
              <span key={`${question.id}${reaction.type}`}>
                {reaction.type}: {reaction.count}&emsp;
              </span>
            ))}
          </div>
        ))}
      </Field>
    </MainContentWrapper>
  );
};
