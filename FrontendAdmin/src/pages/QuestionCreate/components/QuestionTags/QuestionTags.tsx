import React, { ChangeEventHandler, FunctionComponent, MouseEventHandler, useEffect, useRef, useState } from 'react';
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
  onCreate?: (value: string) => void;
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
  const searchRef = useRef<HTMLInputElement>(null);
  const inputRef = useRef<HTMLDivElement>(null);

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

  const handleCreate: React.MouseEventHandler<HTMLButtonElement> = (event) => {
    event.preventDefault();
    if (!onCreate) {
      return;
    }
    onCreate(searchValue);
    setSearchValue('');
  };

  const options = tags.filter(
    (tag) => !checkIsSelected(tag) && tag.value.toLowerCase().indexOf(searchValue.toLowerCase()) >= 0
  );

  return (
    <div className="questionTagsSelector-container">
      <div ref={inputRef} onClick={handleInputClick} className="questionTagsSelector-input">
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
            <input onChange={handleSearch} value={searchValue} ref={searchRef} />
            {onCreate && (
              <button onClick={handleCreate}>{Captions.Create}</button>
            )}
          </div>
          {options.map((option) => (
            <div
              onClick={() => onSelect(option)}
              key={option.id}
              className="questionTagsSelector-item"
            >
              {option.value}
            </div>
          ))}
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
