import React, { FunctionComponent } from 'react';
import { Routes, Route } from 'react-router-dom';
import { pathnames } from '../constants';
import { Home } from '../pages/Home/Home';
import { Rooms } from '../pages/Rooms/Rooms';
import { Questions } from '../pages/Questions/Questions';
import { QuestionCreate } from '../pages/QuestionCreate/QuestionCreate';
import { NotFound } from '../pages/NotFound/NotFound';

export const AppRoutes: FunctionComponent = () => (
  <Routes>
    <Route path={pathnames.home} element={<Home />} />
    <Route path={pathnames.rooms} element={<Rooms />} />
    <Route path={pathnames.questionsCreate} element={<QuestionCreate />} />
    <Route path={pathnames.questions} element={<Questions />} />
    <Route path="*" element={<NotFound />} />
  </Routes>
);
