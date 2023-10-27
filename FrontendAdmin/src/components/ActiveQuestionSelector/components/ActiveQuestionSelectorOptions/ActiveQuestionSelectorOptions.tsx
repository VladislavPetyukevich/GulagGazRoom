import { FunctionComponent } from 'react';
import { Question } from '../../../../types/question';
import { Captions } from '../../../../constants';

export interface Option extends Question {
  open: boolean;
}

interface ActiveQuestionSelectorOptionsProps {
  options: Option[];
  onOptionClick: (option: Question) => void;
  onCreateNew: () => void;
}

export const ActiveQuestionSelectorOptions: FunctionComponent<ActiveQuestionSelectorOptionsProps> = ({
  options,
  onOptionClick,
  onCreateNew,
}) => {
  return (
    <div className='activeQuestionSelector-options'>
      {options.length === 0 ? (
        <button onClick={onCreateNew}>{Captions.CreateAndAddRoomQuestion}</button>
      ) : (
        options.map((option) => (
          <div
            onClick={() => onOptionClick(option)}
            key={option.value}
            className={`activeQuestionSelector-item ${!option.open && 'closed'}`}
          >
            {option.value}
          </div>
        ))
      )}
    </div>
  );
};
