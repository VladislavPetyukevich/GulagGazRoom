import React, { ReactNode, FunctionComponent } from 'react';
import { FieldsBlock } from '../../components/FieldsBlock/FieldsBlock';

interface MainContentWrapperProps {
  className?: string;
  children: ReactNode;
}

export const MainContentWrapper: FunctionComponent<MainContentWrapperProps> =
  ({ className, children }) => {
    return (
      <FieldsBlock className={className}>
        {children}
      </FieldsBlock>
    );
  };
