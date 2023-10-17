import { ChangeEventHandler, FunctionComponent } from 'react';
import { Captions } from '../../constants';

import './RoomsSearch.css';

interface RoomsSearchProps {
  searchValue: string;
  participating: boolean;
  closed: boolean;
  onSearchChange: (value: string) => void;
  onParticipatingChange: (value: boolean) => void;
  onClosedChange: (value: boolean) => void;
}

export const RoomsSearch: FunctionComponent<RoomsSearchProps> = ({
  searchValue,
  participating,
  closed,
  onSearchChange,
  onParticipatingChange,
  onClosedChange,
}) => {
  const handleSearchChange: ChangeEventHandler<HTMLInputElement> = (e) => {
    onSearchChange(e.target.value);
  };

  const handleParticipatingChange: ChangeEventHandler<HTMLInputElement> = (e) => {
    onParticipatingChange(e.target.checked);
  };

  const handleClosedChange: ChangeEventHandler<HTMLInputElement> = (e) => {
    onClosedChange(e.target.checked);
  };

  return (
    <div className="rooms-search">
      <input
        type="text"
        className="qustions-search-value"
        placeholder={Captions.SearchByName}
        value={searchValue}
        onChange={handleSearchChange}
      />
      <input
        id="participating-rooms"
        type="checkbox"
        checked={participating}
        onChange={handleParticipatingChange}
      />
      <label htmlFor="participating-rooms">{Captions.ParticipatingRooms}</label>
      <input
        id="closed-rooms"
        type="checkbox"
        checked={closed}
        onChange={handleClosedChange}
      />
      <label htmlFor="closed-rooms">{Captions.ClosedRooms}</label>
    </div>
  );
};
