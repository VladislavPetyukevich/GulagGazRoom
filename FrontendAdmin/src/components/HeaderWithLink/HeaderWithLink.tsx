import React, { FunctionComponent } from 'react';
import { Link } from 'react-router-dom';
import { Field } from '../../components/FieldsBlock/Field';

import './HeaderWithLink.css';

interface HeaderWithLinkProps {
  title: string;
  path: string;
  linkCaption: string;
  linkFloat: 'left' | 'right';
}

export const HeaderWithLink: FunctionComponent<HeaderWithLinkProps> = ({
  title,
  path,
  linkCaption,
  linkFloat,
}) => {
  return (
    <Field className="header-with-link">
      <Link to={path}>
        <button className={`button-link float-${linkFloat}`}>{linkCaption}</button>
      </Link>
      <span>{title}</span>
    </Field>
  );
};
