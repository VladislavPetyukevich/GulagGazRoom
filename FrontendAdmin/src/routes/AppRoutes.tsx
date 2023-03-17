import React, { FunctionComponent } from 'react';
import { Routes, Route } from 'react-router-dom';
import { pathnames } from '../constants';
import { Home } from '../pages/Home/Home';
import { Rooms } from '../pages/Rooms/Rooms';
import { Questions } from '../pages/Questions/Questions';
import { QuestionCreate } from '../pages/QuestionCreate/QuestionCreate';
import { NotFound } from '../pages/NotFound/NotFound';
import { RoomCreate } from '../pages/RoomCreate/RoomCreate';
import { Room } from '../pages/Room/Room';
import { Session } from '../pages/Session/Session';

export const AppRoutes: FunctionComponent = () => (
  <Routes>
    <Route path={pathnames.home} element={<Home />} />
    <Route path={pathnames.roomsCreate} element={<RoomCreate />} />
    <Route path={pathnames.room} element={<Room />} />
    <Route path={pathnames.rooms} element={<Rooms />} />
    <Route path={pathnames.questionsCreate} element={<QuestionCreate />} />
    <Route path={pathnames.questions} element={<Questions />} />
    <Route path={pathnames.session} element={<Session />} />
    <Route path="*" element={<NotFound />} />
  </Routes>
);
