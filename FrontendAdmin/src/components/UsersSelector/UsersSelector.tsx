import React, { ChangeEvent, FunctionComponent, useCallback, useEffect, useState } from 'react';
import { Loader } from '../Loader/Loader';
import { Paginator } from '../Paginator/Paginator';
import { useApiMethod } from '../../hooks/useApiMethod';
import { PaginationUrlParams, usersApiDeclaration } from '../../apiDeclarations';
import { User } from '../../types/user';

import './UsersSelector.css';

const pageSize = 10;
const initialPageNumber = 1;

interface UsersSelectorProps {
  selected: User[];
  onSelect: (user: User) => void;
  onUnselect: (user: User) => void;
}

export const UsersSelector: FunctionComponent<UsersSelectorProps> = ({
  selected,
  onSelect,
  onUnselect,
}) => {
  const [pageNumber, setPageNumber] = useState(initialPageNumber);
  const {
    apiMethodState: usersState,
    fetchData: fetchUsers,
  } = useApiMethod<User[], PaginationUrlParams>(usersApiDeclaration.getPage);
  const { process: { loading, error }, data: users } = usersState;

  useEffect(() => {
    fetchUsers({
      PageNumber: pageNumber,
      PageSize: pageSize,
    });
  }, [fetchUsers, pageNumber]);

  const handleCheckboxChange = useCallback((event: ChangeEvent<HTMLInputElement>) => {
    const { value, checked } = event.target;
    if (!users) {
      console.warn('No users found');
      return;
    }
    const userItem = users.find(
      user => user.id === value
    );
    if (!userItem) {
      throw new Error('User item not found in state');
    }
    if (checked) {
      onSelect(userItem);
    } else {
      onUnselect(userItem);
    }
  }, [users, onSelect, onUnselect]);

  const createUserItem = useCallback((user: User) => (
    <li key={user.id}>
      <label htmlFor={`input-${user.id}`}>{user.nickname}</label>
      <input
        id={`input-${user.id}`}
        type="checkbox"
        value={user.id}
        checked={selected.some(que => que.id === user.id)}
        onChange={handleCheckboxChange}
      />
    </li>
  ), [selected, handleCheckboxChange]);

  const handleNextPage = useCallback(() => {
    setPageNumber(pageNumber + 1);
  }, [pageNumber]);

  const handlePrevPage = useCallback(() => {
    setPageNumber(pageNumber - 1);
  }, [pageNumber]);

  if (error) {
    return (
      <div>Error: {error}</div>
    );
  }
  if (loading || !users) {
    return (
      <>
        {Array.from({ length: pageSize + 1 }, (_, index) => (
          <div key={index}>
            <Loader />
          </div>
        ))}
      </>
    );
  }
  return (
    <>
      <ul className="users-selector">
        {users.map(createUserItem)}
      </ul>
      <Paginator
        pageNumber={pageNumber}
        prevDisabled={pageNumber === initialPageNumber}
        nextDisabled={users.length !== pageSize}
        onPrevClick={handlePrevPage}
        onNextClick={handleNextPage}
      />
    </>
  );
};
