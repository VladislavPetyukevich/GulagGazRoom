import { FunctionComponent, useCallback, useState } from 'react';
import { useParams } from 'react-router-dom';
import Modal from 'react-modal';
import { useApiMethod } from '../../../../hooks/useApiMethod';
import { roomsApiDeclaration } from '../../../../apiDeclarations';
import { Captions } from '../../../../constants';

import './CloseRoom.css';

export const CloseRoom: FunctionComponent = () => {
  const { id } = useParams();
  const [modalOpen, setModalOpen] = useState(false);
  const {
    apiMethodState,
    fetchData,
  } = useApiMethod<unknown>({ noParseResponse: true });
  const {
    process: { loading, error },
  } = apiMethodState;

  const handleOpenModal = useCallback(() => {
    setModalOpen(true);
  }, []);

  const handleCloseModal = useCallback(() => {
    setModalOpen(false);
  }, []);

  const handleCloseRoom = useCallback(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchData(roomsApiDeclaration.close(id));
    setModalOpen(false);
  }, [id, fetchData]);

  if (loading) {
    return (<div>{Captions.CloseRoomLoading}...</div>);
  }

  if (error) {
    return (<div>{Captions.Error}: {error}</div>);
  }

  return (
    <>
      <button
        onClick={handleOpenModal}
      >
        {Captions.CloseRoom}
      </button>
      <Modal
        isOpen={modalOpen}
        contentLabel={Captions.CloseRoom}
        appElement={document.getElementById('root') || undefined}
        className="close-room-modal"
        onRequestClose={handleCloseModal}
      >
        <div className="close-room-modal-header">
          <h3>{Captions.CloseRoomModalTitle}</h3>
          <button onClick={handleCloseModal}>X</button>
        </div>
        <div className="close-room-modal-content">
          <button onClick={handleCloseRoom}>{Captions.Yes}</button>
          <button onClick={handleCloseModal}>{Captions.No}</button>
        </div>
      </Modal>
    </>
  );
};
