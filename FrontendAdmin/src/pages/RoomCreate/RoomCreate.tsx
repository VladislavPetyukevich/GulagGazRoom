import React, { FormEvent, FunctionComponent, useCallback, useState } from 'react';
import { Field } from '../../components/FieldsBlock/Field';
import { HeaderWithLink } from '../../components/HeaderWithLink/HeaderWithLink';
import { Loader } from '../../components/Loader/Loader';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { QuestionsSelector } from '../../components/QuestionsSelector/QuestionsSelector';
import { SubmitField } from '../../components/SubmitField/SubmitField';
import { UsersSelector } from '../../components/UsersSelector/UsersSelector';
import { pathnames } from '../../constants';
import { Question } from '../../types/question';
import { User } from '../../types/user';
import { useRoomsCreateApi } from './hooks/useRoomsCreateApi';

import './RoomCreate.css';

const nameFieldName = 'roomName';

export const RoomCreate: FunctionComponent = () => {
  const { roomState, createRoom } = useRoomsCreateApi();
  const { process: { loading, error }, success } = roomState;
  const [selectedQuestions, setSelectedQuestions] = useState<Question[]>([]);
  const [selectedUsers, setSelectedUsers] = useState<User[]>([]);

  const handleSubmit = useCallback(async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const form = event.target as HTMLFormElement;
    const data = new FormData(form);
    const roomName = data.get(nameFieldName);
    if (!roomName) {
      return;
    }
    if (typeof roomName !== 'string') {
      throw new Error('qestionText field type error');
    }
    createRoom({
      name: roomName,
      questions: selectedQuestions.map(question => question.id),
      users: selectedUsers.map(user => user.id),
    });
  }, [selectedQuestions, selectedUsers, createRoom]);

  const handleQuestionSelect = useCallback((question: Question) => {
    setSelectedQuestions([...selectedQuestions, question]);
  }, [selectedQuestions]);

  const handleQuestionUnSelect = useCallback((question: Question) => {
    const newSelectedQuestions = selectedQuestions.filter(
      ques => ques.id !== question.id
    );
    setSelectedQuestions(newSelectedQuestions);
  }, [selectedQuestions]);

  const handleUserSelect = useCallback((user: User) => {
    setSelectedUsers([...selectedUsers, user]);
  }, [selectedUsers]);

  const handleUserUnSelect = useCallback((user: User) => {
    const newSelectedUsers = selectedUsers.filter(
      usr => usr.id !== user.id
    );
    setSelectedUsers(newSelectedUsers);
  }, [selectedUsers]);

  const renderStatus = useCallback(() => {
    if (error) {
      return (
        <Field>
          <div>Error: {error}</div>
        </Field>
      );
    }
    if (loading) {
      return (
        <Field>
          <Loader />
        </Field>
      );
    }
    if (success) {
      return (
        <Field>
          <div>Room created successfully</div>
        </Field>
      );
    }
    return <></>;
  }, [error, loading, success]);

  return (
    <MainContentWrapper className="question-create">
      <HeaderWithLink
        title="Create room"
        path={pathnames.rooms}
        linkCaption="<"
        linkFloat="left"
      />
      {renderStatus()}
      <form action="" onSubmit={handleSubmit}>
        <Field>
          <label htmlFor="roomName">Name:</label>
          <input id="roomName" name={nameFieldName} type="text" required />
        </Field>
        <Field>
          <div>Questions:</div>
          <div className="items-selected">
            Selected: {selectedQuestions.map(question => question.value).join(', ')}
          </div>
          <QuestionsSelector
            selected={selectedQuestions}
            onSelect={handleQuestionSelect}
            onUnselect={handleQuestionUnSelect}
          />
          <div>Users:</div>
          <div className="items-selected">
            Selected: {selectedUsers.map(user => user.nickname).join(', ')}
          </div>
          <UsersSelector
            selected={selectedUsers}
            onSelect={handleUserSelect}
            onUnselect={handleUserUnSelect}
          />
        </Field>
        <SubmitField caption="Create" />
      </form>
    </MainContentWrapper>
  );
};
