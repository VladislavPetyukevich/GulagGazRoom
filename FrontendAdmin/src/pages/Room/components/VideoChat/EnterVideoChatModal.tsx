import { FunctionComponent, useCallback, useContext, useEffect, useRef, useState } from 'react';
import Modal from 'react-modal';
import { Captions } from '../../../../constants';
import { DeviceSelect } from './DeviceSelect';
import { createAudioAnalyser, frequencyBinCount } from './utils/createAudioAnalyser';
import { getAverageVolume } from './utils/getAverageVolume';
import { SwitchButton } from './SwitchButton';
import { AuthContext } from '../../../../context/AuthContext';
import { UserAvatar } from '../../../../components/UserAvatar/UserAvatar';

import './EnterVideoChatModal.css';

interface Device {
  deviceId?: MediaDeviceInfo['deviceId'];
  enabled: boolean;
}

export interface Devices {
  mic?: Device;
  camera?: Device;
}

interface EnterVideoChatModalProps {
  open: boolean;
  roomName?: string;
  userStream: MediaStream | null;
  onClose: () => void;
  onSelect: (devices: Devices) => void;
}

const micDeviceKind = 'audioinput';

const cameraDeviceKind = 'videoinput';

const updateAnalyserDelay = 1000 / 30;

const getDevices = async () =>
  await navigator.mediaDevices.enumerateDevices();

export const EnterVideoChatModal: FunctionComponent<EnterVideoChatModalProps> = ({
  open,
  roomName,
  userStream,
  onClose,
  onSelect,
}) => {
  const auth = useContext(AuthContext);
  const [joiningScreen, setJoiningScreen] = useState(true);
  const [micDevices, setMicDevices] = useState<MediaDeviceInfo[]>([]);
  const [micId, setMicId] = useState<MediaDeviceInfo['deviceId']>();
  const [cameraDevices, setCameraDevices] = useState<MediaDeviceInfo[]>([]);
  const [cameraId, setCameraId] = useState<MediaDeviceInfo['deviceId']>();
  const [micVolume, setMicVolume] = useState(0);
  const [micEnabled, setMicEnabled] = useState(true);
  const [cameraEnabled, setCameraEnabled] = useState(true);
  const [settingsEnabled, setSettingsEnabled] = useState(false);
  const userVideo = useRef<HTMLVideoElement>(null);
  const requestRef = useRef<number>();
  const updateAnalyserTimeout = useRef(0);
  const audioAnalyser = useRef<AnalyserNode | null>(null);

  useEffect(() => {
    if (!userStream) {
      return;
    }
    try {
      audioAnalyser.current = createAudioAnalyser(userStream);
    } catch {
      console.warn('Failed to create audioAnalyser');
    }
    if (userVideo.current) {
      userVideo.current.srcObject = userStream;
    }
  }, [userStream]);

  useEffect(() => {
    if (!micId && !cameraId) {
      return;
    }
    onSelect({
      mic: {
        deviceId: micId,
        enabled: micEnabled,
      },
      camera: {
        deviceId: cameraId,
        enabled: cameraEnabled,
      },
    });
  }, [micId, micEnabled, cameraId, cameraEnabled, onSelect]);

  useEffect(() => {
    const frequencyData = new Uint8Array(frequencyBinCount);
    let prevTime = performance.now();
    const updateAudioAnalyser = () => {
      const time = performance.now();
      const delta = time - prevTime;
      if (updateAnalyserTimeout.current > 0) {
        updateAnalyserTimeout.current -= delta;
        prevTime = time;
        requestRef.current = requestAnimationFrame(updateAudioAnalyser);
        return;
      }
      const analyser = audioAnalyser.current;
      if (!analyser) {
        requestRef.current = requestAnimationFrame(updateAudioAnalyser);
        return;
      }
      analyser.getByteFrequencyData(frequencyData);
      const averageVolume = getAverageVolume(frequencyData);
      setMicVolume(averageVolume);

      prevTime = time;
      updateAnalyserTimeout.current = updateAnalyserDelay;
      requestRef.current = requestAnimationFrame(updateAudioAnalyser);
    };
    requestRef.current = requestAnimationFrame(updateAudioAnalyser);

    return () => {
      if (requestRef.current) {
        cancelAnimationFrame(requestRef.current);
      }
    };

  }, []);

  const handleUseMic = async () => {
    const newMicDevices = (await getDevices()).filter(device => device.kind === micDeviceKind);
    setMicDevices(newMicDevices);
  };

  const handleUseCamera = async () => {
    const newCameraDevices = (await getDevices()).filter(device => device.kind === cameraDeviceKind);
    setCameraDevices(newCameraDevices);
  };

  const handleUseAll = () => {
    handleUseMic();
    handleUseCamera();
    setJoiningScreen(false);
  };

  const handleSelectMic = useCallback((deviceId: MediaDeviceInfo['deviceId']) => {
    setMicId(deviceId);
  }, []);

  const handleSelectCamera = useCallback((deviceId: MediaDeviceInfo['deviceId']) => {
    setCameraId(deviceId);
  }, []);

  const handleSwitchMic = () => {
    setMicEnabled(!micEnabled);
  };

  const handleSwitchCamera = () => {
    setCameraEnabled(!cameraEnabled);
  };

  const handleSwitchSettings = () => {
    setSettingsEnabled(!settingsEnabled);
  };

  return (
    <Modal
      isOpen={open}
      contentLabel={Captions.CloseRoom}
      appElement={document.getElementById('root') || undefined}
      className="action-modal enter-videochat-modal"
      style={{
        overlay: {
          backgroundColor: 'rgba(0, 0, 0, 1.0)',
          zIndex: 2,
        },
      }}
    >
      <div className="action-modal-header">
        <h3>{Captions.JoiningRoom} {roomName}</h3>
      </div>
      {joiningScreen ? (
        <div className="enter-videochat-info">
          {!!(auth?.nickname && auth?.avatar) && (
            <UserAvatar
              nickname={auth.nickname}
              src={auth.avatar}
            />
          )}
          <p>{Captions.JoinAs}: {auth?.nickname}</p>
          <button onClick={handleUseAll}>{Captions.SetupDevices}</button>
        </div>
      ) : (
        <div>
          <div className="enter-videochat-row">
            <div className="enter-videochat-column">
              <div className={settingsEnabled ? 'enter-videochat-content-container-mini' : 'enter-videochat-content-container'} >
                <video
                  ref={userVideo}
                  muted
                  autoPlay
                  playsInline
                >
                  Video not supported
                </video>
                <div className="enter-videochat-row switch-row">
                  <SwitchButton
                    enabled={micEnabled}
                    caption={Captions.MicrophoneIcon}
                    onClick={handleSwitchMic}
                  />
                  <SwitchButton
                    enabled={cameraEnabled}
                    caption={Captions.CameraIcon}
                    onClick={handleSwitchCamera}
                  />
                  <SwitchButton
                    enabled={!settingsEnabled}
                    caption={Captions.SettingsIcon}
                    onClick={handleSwitchSettings}
                  />
                </div>
                <progress max="50" value={micVolume}>{Captions.Microphone}: {micVolume}</progress>
              </div>

              <div className="enter-videochat-content-container" style={{ display: settingsEnabled ? 'block' : 'none' }}>
                <div >
                  <div>{Captions.Microphone}</div>
                  <DeviceSelect
                    devices={micDevices}
                    onSelect={handleSelectMic}
                  />
                  <div>{Captions.Camera}</div>
                  <DeviceSelect
                    devices={cameraDevices}
                    onSelect={handleSelectCamera}
                  />
                </div>
              </div>

            </div>
          </div>
          <button className="active" onClick={onClose}>{Captions.Join}</button>
        </div>
      )}
    </Modal>
  );
};
