import React, { FunctionComponent, useCallback, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Field } from '../../components/FieldsBlock/Field';
import { HeaderWithLink } from '../../components/HeaderWithLink/HeaderWithLink';
import { Loader } from '../../components/Loader/Loader';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { Paginator } from '../../components/Paginator/Paginator';
import { pathnames } from '../../constants';
import { Room } from '../../types/room';
import { useRoomsGetApi } from './hooks/useRoomsGetApi';

import './Rooms.css';

const pageSize = 10;
const initialPageNumber = 1;

const createRoomItem = (room: Room) => (
  <li key={room.id}>
    <Field>
      <Link to={`${pathnames.rooms}/${room.id}`}>
        {room.name}
      </Link>
      <div className="room-users">
        {room.users.map(user => user.nickname).join(', ')}
      </div>
    </Field>
  </li>
);

export const Rooms: FunctionComponent = () => {
  const [pageNumber, setPageNumber] = useState(initialPageNumber);
  const {
    roomsState,
    loadRooms,
  } = useRoomsGetApi();
  const { process: { loading, error }, rooms } = roomsState;

  useEffect(() => {
    loadRooms({ pageSize, pageNumber });
  }, [loadRooms, pageNumber]);

  const handleNextPage = useCallback(() => {
    setPageNumber(pageNumber + 1);
  }, [pageNumber]);

  const handlePrevPage = useCallback(() => {
    setPageNumber(pageNumber - 1);
  }, [pageNumber]);

  const renderMainContent = useCallback(() => {
    if (error) {
      return (
        <Field>
          <div>Error: {error}</div>
        </Field>
      );
    }
    if (loading) {
      return (
        Array.from({ length: pageSize + 1 }, (_, index) => (
          <Field key={index}>
            <Loader />
          </Field>
        ))
      );
    }
    return (
      <>
        <ul className="rooms-list">
          {rooms.map(createRoomItem)}
        </ul>
        <Paginator
          pageNumber={pageNumber}
          prevDisabled={pageNumber === initialPageNumber}
          nextDisabled={rooms.length !== pageSize}
          onPrevClick={handlePrevPage}
          onNextClick={handleNextPage}
        />
      </>
    );
  }, [error, loading, pageNumber, rooms, handleNextPage, handlePrevPage]);

  return (
    <MainContentWrapper>
      <HeaderWithLink
        title="Rooms:"
        path={pathnames.roomsCreate}
        linkCaption="+"
        linkFloat="right"
      />
      {renderMainContent()}
    </MainContentWrapper>
  );
};
