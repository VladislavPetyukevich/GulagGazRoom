import React, { FunctionComponent, MouseEventHandler } from 'react';
import { Field } from '../FieldsBlock/Field';

import './Paginator.css';

interface PaginatorProps {
  pageNumber: number;
  prevDisabled: boolean;
  nextDisabled: boolean;
  onPrevClick: MouseEventHandler<HTMLButtonElement>;
  onNextClick: MouseEventHandler<HTMLButtonElement>;
}

export const Paginator: FunctionComponent<PaginatorProps> = ({
  pageNumber,
  prevDisabled,
  nextDisabled,
  onPrevClick,
  onNextClick,
}) => {
  return (
    <Field className="paginator">
      <button
        disabled={prevDisabled}
        onClick={onPrevClick}
      >
        &#60;
      </button>
      <span>Page: {pageNumber}</span>
      <button
        disabled={nextDisabled}
        onClick={onNextClick}
      >
        &#62;
      </button>
    </Field>
  );
};
