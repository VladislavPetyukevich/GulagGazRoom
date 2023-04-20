import React, { FunctionComponent, useEffect, useState } from 'react';
import { Captions } from '../../constants';

import './SovietMark.css';

interface SovietMarkProps {
  likes: number;
  dislikes: number;
}

export const SovietMark: FunctionComponent<SovietMarkProps> = ({
  likes,
  dislikes,
}) => {
  const [markWithComment, setMarkWithComment] = useState<string>(Captions.MarkNotCalculated);
  const [markPostfix, setMarkPostfix] = useState<string>(Captions.MarkNotCalculated);

  useEffect(() => {
    const getMarkWithComment = (mark: number) => {
      const markInt = ~~mark;
      const markParts = mark.toString().split('.');
      const markFirstDecimal = markParts.length < 2 ? 0 : +markParts[1][0];
      if (markFirstDecimal >= 8) {
        return `${markInt + 1} ${Captions.MarkWithMinus}.`;
      }
      if (markFirstDecimal > 5) {
        return `${markInt} ${Captions.MarkWithPlus}.`;
      }
      return `${Captions.MarkAveragePrefix} ${markInt} ${Captions.MarkAverage}.`;
    };

    const getMarkPostfix = (mark: number) => {
      if (mark > 4) {
        return Captions.MarkPostfixCool;
      }
      if (mark >= 3) {
        return Captions.MarkPostfixAverage;
      }
      return Captions.MarkPostfixBad;
    };

    const totalCount = likes + dislikes;
    const mark = likes / totalCount * 10 / 2;
    const newMarkWithComment = getMarkWithComment(mark);
    const markPostfix = getMarkPostfix(mark);
    setMarkWithComment(newMarkWithComment);
    setMarkPostfix(markPostfix);
  }, [likes, dislikes]);

  return (
    <div className="soviet-mark">
      <div>{markWithComment}</div>
      <div className="soviet-mark-postfix">{Captions.MarkSmmary}. {markPostfix}</div>
    </div>
  );
};
