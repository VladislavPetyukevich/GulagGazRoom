import React, { FormEvent, FunctionComponent, useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import { CreateRoomBody, roomsApiDeclaration } from '../../apiDeclarations';
import { Field } from '../../components/FieldsBlock/Field';
import { HeaderWithLink } from '../../components/HeaderWithLink/HeaderWithLink';
import { Loader } from '../../components/Loader/Loader';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { QuestionsSelector } from '../../components/QuestionsSelector/QuestionsSelector';
import { SubmitField } from '../../components/SubmitField/SubmitField';
import { UsersSelector } from '../../components/UsersSelector/UsersSelector';
import { Captions, pathnames, toastSuccessOptions } from '../../constants';
import { useApiMethod } from '../../hooks/useApiMethod';
import { Question } from '../../types/question';
import { User } from '../../types/user';

import './RoomCreate.css';

const nameFieldName = 'roomName';
const twitchChannelFieldName = 'roomTwitchChannel';

export const RoomCreate: FunctionComponent = () => {
  const navigate = useNavigate();
  const { apiMethodState, fetchData } = useApiMethod<string, CreateRoomBody>(roomsApiDeclaration.create);
  const { process: { loading, error }, data: createdRoomId } = apiMethodState;
  const [selectedQuestions, setSelectedQuestions] = useState<Question[]>([]);
  const [selectedExperts, setSelectedExperts] = useState<User[]>([]);
  const [selectedExaminees, setSelectedExaminees] = useState<User[]>([]);

  useEffect(() => {
    if (!createdRoomId) {
      return;
    }
    toast.success(Captions.RoomCreated, toastSuccessOptions);
    navigate(pathnames.rooms);
  }, [createdRoomId, navigate]);

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
    const roomTwitchChannel = data.get(twitchChannelFieldName);
    if (!roomTwitchChannel) {
      return;
    }
    if (typeof roomTwitchChannel !== 'string') {
      throw new Error('roomTwitchChannel field type error');
    }
    fetchData({
      name: roomName,
      twitchChannel: roomTwitchChannel,
      questions: selectedQuestions.map(question => question.id),
      experts: selectedExperts.map(user => user.id),
      examinees: selectedExaminees.map(user => user.id),
    });
  }, [selectedQuestions, selectedExperts, selectedExaminees, fetchData]);

  const handleQuestionSelect = useCallback((question: Question) => {
    setSelectedQuestions([...selectedQuestions, question]);
  }, [selectedQuestions]);

  const handleQuestionUnSelect = useCallback((question: Question) => {
    const newSelectedQuestions = selectedQuestions.filter(
      ques => ques.id !== question.id
    );
    setSelectedQuestions(newSelectedQuestions);
  }, [selectedQuestions]);

  const handleExpertSelect = useCallback((user: User) => {
    setSelectedExperts([...selectedExperts, user]);
  }, [selectedExperts]);

  const handleExpertUnSelect = useCallback((user: User) => {
    const newSelectedUsers = selectedExperts.filter(
      usr => usr.id !== user.id
    );
    setSelectedExperts(newSelectedUsers);
  }, [selectedExperts]);

  const handleExamineeSelect = useCallback((user: User) => {
    setSelectedExaminees([...selectedExaminees, user]);
  }, [selectedExaminees]);

  const handleExamineeUnSelect = useCallback((user: User) => {
    const newSelectedUsers = selectedExaminees.filter(
      usr => usr.id !== user.id
    );
    setSelectedExaminees(newSelectedUsers);
  }, [selectedExaminees]);

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
    return <></>;
  }, [error, loading]);

  return (
    <MainContentWrapper className="question-create">
      <HeaderWithLink
        title="Create room"
        linkVisible={true}
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
          <label htmlFor="twitchChannel">Twitch channel name:</label>
          <input id="twitchChannel" name={twitchChannelFieldName} type="text" required />
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
            Experts: {selectedExperts.map(user => user.nickname).join(', ')}
          </div>
          <UsersSelector
            uniqueKey='Experts'
            selected={selectedExperts}
            onSelect={handleExpertSelect}
            onUnselect={handleExpertUnSelect}
          />
          <div className="items-selected">
            Examinees: {selectedExaminees.map(user => user.nickname).join(', ')}
          </div>
           <UsersSelector
            uniqueKey='Examinees'
            selected={selectedExaminees}
            onSelect={handleExamineeSelect}
            onUnselect={handleExamineeUnSelect}
          />
        </Field>
        <SubmitField caption="Create" />
      </form>
    </MainContentWrapper>
  );
};
