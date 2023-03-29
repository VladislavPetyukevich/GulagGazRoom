import { ChangeEvent, FunctionComponent, useCallback, useState } from 'react';
import { Question } from '../../types/question';

import './ActiveQuestionSelector.css';

interface ActiveQuestionSelectorProps {
  questions: Question[];
  openQuestions: Question['id'][];
  selectButtonLabel: string;
  onSelect: (question: Question) => void;
}

export const ActiveQuestionSelector: FunctionComponent<ActiveQuestionSelectorProps> = ({
  questions,
  openQuestions,
  selectButtonLabel,
  onSelect,
}) => {
  const [selectedQuestion, setSelectedQuestion] = useState<Question>(questions[0]);
  const questionsWithStatusSorted = questions
    .map(
      question => ({ ...question, open: openQuestions.includes(question.id) })
    )
    .sort(
      (question1, question2) => {
        if (question1.open === question2.open) {
          return 0;
        }
        return +question2.open - +question1.open;
      }
    );

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
        {questionsWithStatusSorted
          .map(question => (
            <option
              key={question.id}
              value={question.id}
              className={`${question.open ? '' : 'closed'}`}
            >
              {question.value}
            </option>
          ))
        }
      </select>
      <br />
      <button onClick={handleOnSelect}>{selectButtonLabel}</button>
    </div>
  );
};
