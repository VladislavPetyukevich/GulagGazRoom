import React, { ReactNode, FunctionComponent } from 'react';
import { FieldsBlock } from '../../components/FieldsBlock/FieldsBlock';

interface MainContentWrapperProps {
  children: ReactNode
}

export const MainContentWrapper: FunctionComponent<MainContentWrapperProps> =
  ({ children }) => {
    return (
      <FieldsBlock>
        {children}
      </FieldsBlock>
    );
  };
