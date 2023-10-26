import { Reaction } from '../../../types/reaction';

interface AseAdditionalReactionsParams {
  reactions: Reaction[];
  eventTypeAdditionalNames: Record<string, string[]>;
}

export const useAdditionalReactions = ({
  reactions,
  eventTypeAdditionalNames,
}: AseAdditionalReactionsParams): Reaction[] => {
  const additionalReactions: Reaction[] = [];

  reactions.forEach(reaction => {
    const additionalReactionNames = eventTypeAdditionalNames[reaction.type.name];
    if (!additionalReactionNames) {
      return;
    }
    additionalReactionNames.forEach(additionalReactionName => {
      additionalReactions.push({
        ...reaction,
        type: {
          ...reaction.type,
          name: additionalReactionName,
        },
      });
    });
  });

  return additionalReactions;
};
