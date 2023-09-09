import React, { ChangeEventHandler, FunctionComponent, MouseEventHandler, useEffect, useRef, useState } from 'react';
import { Question } from '../../types/question';
import { OpenIcon } from '../OpenIcon/OpenIcon';

import './ActiveQuestionSelector.css';

export interface ActiveQuestionSelectorProps {
  placeHolder: string;
  showClosedQuestions: boolean;
  questions: Question[];
  openQuestions: Array<Question['id']>;
  onSelect: (question: Question) => void;
}

export const ActiveQuestionSelector: FunctionComponent<ActiveQuestionSelectorProps> = ({
  placeHolder,
  showClosedQuestions,
  questions,
  openQuestions,
  onSelect
}) => {
  const [showMenu, setShowMenu] = useState(false);
  const [selectedValue, setSelectedValue] = useState<Question | null>(null);
  const [searchValue, setSearchValue] = useState("");
  const searchRef = useRef<HTMLInputElement>(null);
  const inputRef = useRef<HTMLDivElement>(null);

  const isOpened = (question: Question) => {
    return openQuestions.includes(question.id);
  }

  const questionsFiltered = questions.filter(
    question => showClosedQuestions ? !isOpened(question) : isOpened(question)
  );

  useEffect(() => {
    setSearchValue("");
    if (showMenu && searchRef.current) {
      searchRef.current.focus();
    }
  }, [showMenu]);

  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (!e.target) {
        return;
      }
      if (inputRef.current && !inputRef.current.contains(e.target as any)) {
        setShowMenu(false);
      }
    };

    window.addEventListener('click', handler);
    return () => {
      window.removeEventListener('click', handler);
    };
  });
  const handleInputClick: MouseEventHandler<HTMLDivElement> = () => {
    setShowMenu(!showMenu);
  };

  const getDisplay = () => {
    if (!selectedValue) {
      return placeHolder;
    }
    return selectedValue.value;
  };

  const onItemClick = (option: Question) => {
    setSelectedValue(option);
    onSelect(option);
  };

  const onSearch: ChangeEventHandler<HTMLInputElement> = (e) => {
    setSearchValue(e.target.value);
  };

  const getOptions = () => {
    if (!searchValue) {
      return questionsFiltered;
    }

    return questionsFiltered.filter(
      (question) =>
        question.value.toLowerCase().indexOf(searchValue.toLowerCase()) >= 0
    );
  };

  return (
    <div className="activeQuestionSelector-container">
      <div ref={inputRef} onClick={handleInputClick} className="activeQuestionSelector-input">
        <div className="activeQuestionSelector-selected-value">{getDisplay()}</div>
        <div className="activeQuestionSelector-tools">
          <div className="activeQuestionSelector-tool">
            <OpenIcon />
          </div>
        </div>
      </div>
      {showMenu && (
        <div className="activeQuestionSelector-menu">
          <div className="search-box">
            <input onChange={onSearch} value={searchValue} ref={searchRef} />
          </div>
          {getOptions().map((option) => (
            <div
              onClick={() => onItemClick(option)}
              key={option.value}
              className={`activeQuestionSelector-item ${!isOpened(option) && 'closed'}`}
            >
              {option.value}
            </div>
          ))}
        </div>
      )}
    </div>
  );
};
