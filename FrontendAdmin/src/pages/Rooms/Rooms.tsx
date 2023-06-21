import React, { FunctionComponent, useCallback, useContext, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { PaginationUrlParams, roomsApiDeclaration } from '../../apiDeclarations';
import { Field } from '../../components/FieldsBlock/Field';
import { HeaderWithLink } from '../../components/HeaderWithLink/HeaderWithLink';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { Paginator } from '../../components/Paginator/Paginator';
import { Captions, pathnames } from '../../constants';
import { AuthContext } from '../../context/AuthContext';
import { useApiMethod } from '../../hooks/useApiMethod';
import { Room } from '../../types/room';
import { checkAdmin } from '../../utils/checkAdmin';
import { ProcessWrapper } from '../../components/ProcessWrapper/ProcessWrapper';

import './Rooms.css';

const roomStatusCaption: Record<Room['roomStatus'], string> = {
  New: Captions.RoomStatusNew,
  Active: Captions.RoomStatusActive,
  Review: Captions.RoomStatusReview,
  Close: Captions.RoomStatusClose,
};

const pageSize = 10;
const initialPageNumber = 1;

export const Rooms: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const admin = checkAdmin(auth);
  const [pageNumber, setPageNumber] = useState(initialPageNumber);
  const { apiMethodState, fetchData } = useApiMethod<Room[], PaginationUrlParams>(roomsApiDeclaration.getPage);
  const { process: { loading, error }, data: rooms } = apiMethodState;
  const loaders = Array.from({ length: pageSize }, () => ({ height: '4.46rem' }));
  const roomsSafe = rooms || [];

  useEffect(() => {
    fetchData({
      PageSize: pageSize,
      PageNumber: pageNumber,
    });
  }, [fetchData, pageNumber]);

  const handleNextPage = useCallback(() => {
    setPageNumber(pageNumber + 1);
  }, [pageNumber]);

  const handlePrevPage = useCallback(() => {
    setPageNumber(pageNumber - 1);
  }, [pageNumber]);

  const createRoomItem = useCallback((room: Room) => {
    const roomSummary =
      room.roomStatus === 'Review' ||
      room.roomStatus === 'Close';
    const roomLink = roomSummary ?
      pathnames.roomAnalyticsSummary.replace(':id', room.id) :
      `${pathnames.rooms}/${room.id}`;

    return (
      <li key={room.id}>
        <Field>
          <Link to={roomLink} className='room-link'>
            {`${room.name} (${roomStatusCaption[room.roomStatus]})`}
          </Link>
          <div className="room-users">
            <span>Участники: </span>
            {room.users.map(user => user.nickname).join(', ')}
          </div>
          {admin && (
            <Link to={`${pathnames.roomsParticipants.replace(':id', room.id)}`}>
              {Captions.EditParticipants}
            </Link>
          )}
        </Field>
      </li>
    );
  }, [admin]);

  return (
    <MainContentWrapper>
      <HeaderWithLink
        title={`${Captions.RoomsPageName}:`}
        linkVisible={admin}
        path={pathnames.roomsCreate}
        linkCaption="+"
        linkFloat="right"
      />
      <ProcessWrapper
        loading={loading}
        error={error}
        loaders={loaders}
      >
        <>
          <ul className="rooms-list">
            {roomsSafe.map(createRoomItem)}
          </ul>
          <Paginator
            pageNumber={pageNumber}
            prevDisabled={pageNumber === initialPageNumber}
            nextDisabled={roomsSafe.length !== pageSize}
            onPrevClick={handlePrevPage}
            onNextClick={handleNextPage}
          />
        </>
      </ProcessWrapper>
    </MainContentWrapper>
  );
};
