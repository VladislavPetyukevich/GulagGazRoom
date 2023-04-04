import React, { ChangeEventHandler, FunctionComponent, MouseEventHandler, useEffect, useRef, useState } from 'react';
import { Question } from '../../types/question';

import './ActiveQuestionSelector.css';

const OpenIcon = () => {
  return (
    <svg height="20" width="20" viewBox="0 0 20 16">
      <path d="M4.516 7.548c0.436-0.446 1.043-0.481 1.576 0l3.908 3.747 3.908-3.747c0.533-0.481 1.141-0.446 1.574 0 0.436 0.445 0.408 1.197 0 1.615-0.406 0.418-4.695 4.502-4.695 4.502-0.217 0.223-0.502 0.335-0.787 0.335s-0.57-0.112-0.789-0.335c0 0-4.287-4.084-4.695-4.502s-0.436-1.17 0-1.615z"></path>
    </svg>
  );
};

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
