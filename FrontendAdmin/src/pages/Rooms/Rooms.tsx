import React, { FunctionComponent, useCallback, useContext, useEffect, useState } from 'react';
import { Tooltip } from 'react-tooltip'
import { Link } from 'react-router-dom';
import { GetRoomPageParams, roomsApiDeclaration } from '../../apiDeclarations';
import { Field } from '../../components/FieldsBlock/Field';
import { HeaderWithLink } from '../../components/HeaderWithLink/HeaderWithLink';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { Paginator } from '../../components/Paginator/Paginator';
import { Captions, pathnames } from '../../constants';
import { AuthContext } from '../../context/AuthContext';
import { useApiMethod } from '../../hooks/useApiMethod';
import { Room, RoomStatus } from '../../types/room';
import { checkAdmin } from '../../utils/checkAdmin';
import { ProcessWrapper } from '../../components/ProcessWrapper/ProcessWrapper';
import { TagsView } from '../../components/TagsView/TagsView';
import { RoomsSearch } from '../../components/RoomsSearch/RoomsSearch';

import './Rooms.css';

const userTooltipId = 'user-tooltip';

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
  const { apiMethodState, fetchData } = useApiMethod<Room[], GetRoomPageParams>(roomsApiDeclaration.getPage);
  const { process: { loading, error }, data: rooms } = apiMethodState;
  const [searchValue, setSearchValue] = useState('');
  const [participating, setParticipating] = useState(false);
  const [closed, setClosed] = useState(false);
  const loaders = Array.from({ length: pageSize }, () => ({ height: '4.46rem' }));
  const roomsSafe = rooms || [];

  useEffect(() => {
    const participants = (auth?.id && participating) ? [auth?.id] : [];
    const statuses: RoomStatus[] = closed ? ['Close'] : ['New', 'Active', 'Review'];
    fetchData({
      PageSize: pageSize,
      PageNumber: pageNumber,
      Name: searchValue,
      Participants: participants,
      Statuses: statuses,
    });
  }, [pageNumber, searchValue, auth?.id, participating, closed, fetchData]);

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
            {room.name}
          </Link>
          <div>
            {roomStatusCaption[room.roomStatus]}
          </div>
          <div className="room-users">
            <span>Участники ({room.users.length}):&nbsp;</span>
            {room.users.map((user, index, usersArr) => (
              <span key={user.id}>
                {user.avatar ? (
                  <img
                    src={user.avatar}
                    className="room-users-avatar"
                    alt={`${user.nickname} avatar`}
                    data-tooltip-id={userTooltipId}
                    data-tooltip-content={user.nickname}
                  />
                ) : (
                  <span>{user.nickname}</span>
                )}
                {index < usersArr.length - 1 ? (<>,&nbsp;</>) : ''}
              </span>
            ))}
          </div>
          <div className="room-tags">
            <TagsView
              placeHolder={Captions.NoTags}
              tags={room.tags}
            />
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
      <Tooltip id={userTooltipId} />
      <Field>
        <RoomsSearch
          searchValue={searchValue}
          participating={participating}
          closed={closed}
          onSearchChange={setSearchValue}
          onParticipatingChange={setParticipating}
          onClosedChange={setClosed}
        />
      </Field>
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
