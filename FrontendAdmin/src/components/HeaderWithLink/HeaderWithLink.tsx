import React, { FunctionComponent } from 'react';
import { Link } from 'react-router-dom';
import { Field } from '../../components/FieldsBlock/Field';

import './HeaderWithLink.css';

interface HeaderWithLinkProps {
  title: string;
  linkVisible: boolean;
  path: string;
  linkCaption: string;
  linkFloat: 'left' | 'right';
}

export const HeaderWithLink: FunctionComponent<HeaderWithLinkProps> = ({
  title,
  linkVisible,
  path,
  linkCaption,
  linkFloat,
}) => {
  return (
    <Field className="header-with-link">
      {linkVisible && (
        <Link to={path}>
          <button
            className={`button-link float-${linkFloat}`}
            data-cy={`header-link-${path}`}
          >
            {linkCaption}
          </button>
        </Link>
      )}
      <span>{title}</span>
    </Field>
  );
};
