import React, { ChangeEventHandler, FunctionComponent, MouseEventHandler, useEffect, useState } from 'react';
import { Tag } from '../../../../types/question';
import { Captions } from '../../../../constants';
import { OpenIcon } from '../../../../components/OpenIcon/OpenIcon';
import { QuestionTagsView } from '../../../../components/QuestionTagsView/QuestionTagsView';

import './QuestionTags.css';

export interface QuestionTagsProps {
  loading: boolean;
  tags: Tag[];
  selectedTags: Tag[];
  placeHolder: string;
  onSelect: (tag: Tag) => void;
  onUnselect: (tag: Tag) => void;
  onSearch: (value: string) => void;
  onCreate?: (tag: Omit<Tag, 'id'>) => void;
}

export const QuestionTags: FunctionComponent<QuestionTagsProps> = ({
  loading,
  tags,
  selectedTags,
  placeHolder,
  onSelect,
  onUnselect,
  onSearch,
  onCreate,
}) => {
  const [showMenu, setShowMenu] = useState(false);
  const [searchValue, setSearchValue] = useState("");
  const [color, setColor] = useState("#000000");

  const checkIsSelected = (tag: Tag) => {
    return !!selectedTags.find(tg => tg.id === tag.id);
  }

  useEffect(() => {
    onSearch(searchValue);
  }, [searchValue, onSearch]);

  const handleInputClick: MouseEventHandler<HTMLDivElement> = () => {
    setShowMenu(!showMenu);
  };

  const handleUnselectClick = (tag: Tag): MouseEventHandler<HTMLElement> => (event) => {
    event.stopPropagation();
    onUnselect(tag);
  }

  const handleSearch: ChangeEventHandler<HTMLInputElement> = (e) => {
    setSearchValue(e.target.value);
  };

  const handleColorChange: ChangeEventHandler<HTMLInputElement> = (e) => {
    setColor(e.target.value);
  };

  const handleCreate: React.MouseEventHandler<HTMLButtonElement> = (event) => {
    event.preventDefault();
    if (!onCreate) {
      return;
    }
    onCreate({
      value: searchValue,
      hexValue: color.replace('#', ''),
    });
    setSearchValue('');
  };

  const options = tags.filter(
    (tag) => !checkIsSelected(tag) && tag.value.toLowerCase().indexOf(searchValue.toLowerCase()) >= 0
  );

  return (
    <div className="questionTagsSelector-container">
      <div onClick={handleInputClick} className="questionTagsSelector-input">
        <QuestionTagsView tags={selectedTags} placeHolder={placeHolder} onClick={handleUnselectClick} />
        <div className="questionTagsSelector-tools">
          <div className="questionTagsSelector-tool">
            <OpenIcon />
          </div>
        </div>
      </div>
      {showMenu && (
        <div className="questionTagsSelector-menu">
          <div className="search-box">
            <input className='tag-value' onChange={handleSearch} value={searchValue} />
            {onCreate && (
              <>
                <input type="color" value={color} onChange={handleColorChange} />
                <button onClick={handleCreate}>{Captions.Create}</button>
              </>
            )}
          </div>
          <div className="questionTagsSelector-items">
          {options.map((option) => (
            <div
              onClick={() => onSelect(option)}
              key={option.id}
              className="questionTagsSelector-item"
              style={{ borderColor: `#${option.hexValue}` }}
            >
              {option.value}
            </div>
          ))}
          </div>
          {options.length === 0 && (
            <div>{Captions.NoTags}</div>
          )}
          {loading && (
            <div>{Captions.TagsLoading}...</div>
          )}
        </div>
      )}
    </div>
  );
};
