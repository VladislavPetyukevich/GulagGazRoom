import React, { FunctionComponent, useEffect, useState } from 'react';

interface SovietMarkProps {
  likes: number;
  dislikes: number;
}

export const SovietMark: FunctionComponent<SovietMarkProps> = ({
  likes,
  dislikes,
}) => {
  const [markWithComment, setMarkWithComment] = useState('Mark not calculated');
  const [markPostfix, setMarkPostfix] = useState('Mark not calculated');

  useEffect(() => {
    const getMarkWithComment = (mark: number) => {
      const markInt = ~~mark;
      const markParts = mark.toString().split('.');
      const markFirstDecimal = markParts.length < 2 ? 0 : +markParts[1][0];
      if (markFirstDecimal >= 8) {
        return `${markInt + 1} с минусом.`;
      }
      if (markFirstDecimal > 5) {
        return `${markInt} с плюсом.`;
      }
      return `Чисто ${markInt} без плюса и минуса, без крестика и без нолика.`;
    };

    const getMarkPostfix = (mark: number) => {
      if (mark > 4) {
        return 'Ну ты и даёшь, братишка.';
      }
      if (mark >= 3) {
        return 'То густо, то пусто. Продолжай, брат.';
      }
      return 'Надо тренироваться, брат.';
    };

    const totalCount = likes + dislikes;
    const mark = likes / totalCount * 10 / 2;
    const newMarkWithComment = getMarkWithComment(mark);
    const markPostfix = getMarkPostfix(mark);
    setMarkWithComment(newMarkWithComment);
    setMarkPostfix(markPostfix);
  }, [likes, dislikes]);

  return (
    <>
      <div>{markWithComment}</div>
      <div>Чтож сказать. {markPostfix}</div>
    </>
  );
};
