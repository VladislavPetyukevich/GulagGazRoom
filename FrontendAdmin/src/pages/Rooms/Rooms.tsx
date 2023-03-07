import React, { FunctionComponent } from 'react';
import { Field } from '../../components/FieldsBlock/Field';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';

export const Rooms: FunctionComponent = () => {
  console.log(Array.from({ length: 11 }, (_, index) => `Room ${index}`));
  return (
    <MainContentWrapper>
      <Field>
        <div>Rooms</div>
      </Field>
    </MainContentWrapper>
  );
};
