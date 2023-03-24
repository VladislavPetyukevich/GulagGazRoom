import { ChangeEvent, FunctionComponent, useCallback, useState } from 'react';
import { Question } from '../../types/question';

import './ActiveQuestionSelector.css';

interface ActiveQuestionSelectorProps {
  questions: Question[];
  selectButtonLabel: string;
  onSelect: (question: Question) => void;
}

export const ActiveQuestionSelector: FunctionComponent<ActiveQuestionSelectorProps> = ({
  questions,
  selectButtonLabel,
  onSelect,
}) => {
  const [selectedQuestion, setSelectedQuestion] = useState<Question>(questions[0]);

  const handleSelectChange = useCallback((event: ChangeEvent<HTMLSelectElement>) => {
    const targetValue = event.target.value;
    const newSelectedQuestion = questions.find(
      question => question.id === targetValue
    );
    if (!newSelectedQuestion) {
      console.error(`Cannot select question ${targetValue}`);
      return;
    }
    setSelectedQuestion(newSelectedQuestion)
  }, [questions]);

  const handleOnSelect = useCallback(() => {
    onSelect(selectedQuestion);
  }, [selectedQuestion, onSelect]);

  return (
    <div className="activeQuestionSelector">
      <select
        size={8}
        onChange={handleSelectChange}
      >
        {questions.map(question => (
          <option
            key={question.id}
            value={question.id}
          >
            {question.value}
          </option>
        ))}
      </select>
      <br/>
      <button onClick={handleOnSelect}>{selectButtonLabel}</button>
    </div>
  );
};
