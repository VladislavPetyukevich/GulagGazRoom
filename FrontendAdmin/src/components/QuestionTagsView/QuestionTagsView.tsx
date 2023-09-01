import { FunctionComponent, MouseEventHandler } from 'react';
import { Tag } from '../../types/tag';

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
    const style = { borderColor: `#${tag.hexValue}` };
    if (onClick) {
      return <span className='tag-item' style={style} onClick={onClick(tag)} key={tag.id}>{tag.value} ✖</span>;
    }
    return <span className='tag-item' style={style} key={tag.id}>{tag.value}</span>
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
