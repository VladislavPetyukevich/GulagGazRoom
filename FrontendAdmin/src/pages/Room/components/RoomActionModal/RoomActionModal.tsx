import { FunctionComponent, useCallback, useState } from 'react';
import Modal from 'react-modal';
import { Captions } from '../../../../constants';

import './RoomActionModal.css';

interface RoomActionModalProps {
  title: string;
  openButtonCaption: string;
  loading: boolean;
  error: string | null;
  onAction: () => void;
}

export const RoomActionModal: FunctionComponent<RoomActionModalProps> = ({
  title,
  openButtonCaption,
  loading,
  error,
  onAction,
}) => {
  const [modalOpen, setModalOpen] = useState(false);

  const handleOpenModal = useCallback(() => {
    setModalOpen(true);
  }, []);

  const handleCloseModal = useCallback(() => {
    setModalOpen(false);
  }, []);

  const onCallAction = useCallback(() => {
    handleCloseModal();
    onAction();
  }, [handleCloseModal, onAction]);

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
        {openButtonCaption}
      </button>
      <Modal
        isOpen={modalOpen}
        contentLabel={Captions.CloseRoom}
        appElement={document.getElementById('root') || undefined}
        className="close-room-modal"
        onRequestClose={handleCloseModal}
      >
        <div className="close-room-modal-header">
          <h3>{title}</h3>
          <button onClick={handleCloseModal}>X</button>
        </div>
        <div className="close-room-modal-content">
          <button onClick={onCallAction}>{Captions.Yes}</button>
          <button onClick={handleCloseModal}>{Captions.No}</button>
        </div>
      </Modal>
    </>
  );
};
