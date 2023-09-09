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
import { RoomParticipants } from '../pages/RoomParticipants/RoomParticipants';
import { ProtectedRoute } from './ProtectedRoute';
import { UserAuth } from '../types/user';
import { Terms } from '../pages/Terms/Terms';
import { RoomAnayticsSummary } from '../pages/RoomAnayticsSummary/RoomAnayticsSummary';

interface AppRoutesProps {
  user: UserAuth | null;
}

export const AppRoutes: FunctionComponent<AppRoutesProps> = ({
  user,
}) => {
  const authenticated = !!user;
  return (
    <Routes>
      <Route path={pathnames.home} element={<Home />} />
      <Route path={pathnames.terms} element={<Terms />} />
      <Route path={pathnames.roomsCreate}
        element={
          <ProtectedRoute allowed={authenticated}>
            <RoomCreate />
          </ProtectedRoute>
        }
      />
      <Route path={pathnames.roomsParticipants}
        element={
          <ProtectedRoute allowed={authenticated}>
            <RoomParticipants />
          </ProtectedRoute>
        }
      />
      <Route path={pathnames.roomAnalyticsSummary}
        element={
          <ProtectedRoute allowed={authenticated}>
            <RoomAnayticsSummary />
          </ProtectedRoute>
        }
      />
      <Route path={pathnames.room}
        element={
          <ProtectedRoute allowed={authenticated}>
            <Room />
          </ProtectedRoute>
        }
      />
      <Route path={pathnames.rooms}
        element={
          <ProtectedRoute allowed={authenticated}>
            <Rooms />
          </ProtectedRoute>
        }
      />
      <Route path={pathnames.questionsCreate}
        element={
          <ProtectedRoute allowed={authenticated}>
            <QuestionCreate edit={false} />
          </ProtectedRoute>
        }
      />
      <Route path={pathnames.questionsEdit}
        element={
          <ProtectedRoute allowed={authenticated}>
            <QuestionCreate edit={true} />
          </ProtectedRoute>
        }
      />
      <Route path={pathnames.questions}
        element={
          <ProtectedRoute allowed={authenticated}>
            <Questions />
          </ProtectedRoute>
        }
      />
      <Route path={pathnames.session}
        element={
          <ProtectedRoute allowed={authenticated}>
            <Session />
          </ProtectedRoute>
        }
      />
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
};
