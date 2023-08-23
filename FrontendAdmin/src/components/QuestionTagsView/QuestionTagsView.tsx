import { FunctionComponent, MouseEventHandler } from 'react';
import { Tag } from '../../types/question';

import './QuestionTagsView.css';

interface QuestionTagsViewProps {
  tags: Tag[];
  placeHolder: string;
  onClick?: (tag: Tag) => MouseEventHandler<HTMLElement>;
}

export const QuestionTagsView: FunctionComponent<QuestionTagsViewProps> = ({
  tags,
  placeHolder,
  onClick,
}) => {
  const createItem = (tag: Tag) => {
    if (onClick) {
      return <span className='tag-item' onClick={onClick(tag)} key={tag.id}>{tag.value} ✖</span>;
    }
    return <span className='tag-item' key={tag.id}>{tag.value}</span>
  };

  const getDisplay = () => {
    if (tags.length === 0) {
      return placeHolder;
    }
    return tags.map(createItem);
  };

  return (
    <div className="questionTags">{getDisplay()}</div>
  );
};
