import { FunctionComponent, useRef, KeyboardEvent, useEffect } from 'react';
import { Transcript } from '../../../../types/transcript';
import { Captions } from '../../../../constants';
import { stringToColor } from './utils/stringToColor';

import './MessagesChat.css';

interface MessagesChatProps {
  transcripts: Transcript[];
  onMessageSubmit: (message: string) => void;
}

export const MessagesChat: FunctionComponent<MessagesChatProps> = ({
  transcripts,
  onMessageSubmit,
}) => {
  const messageInputRef = useRef<HTMLInputElement>(null);
  const videochatTranscriptsRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const chatEl = videochatTranscriptsRef.current;
    if (!chatEl) {
      return;
    }
    const scrollHeight = chatEl.scrollHeight;
    const height = chatEl.clientHeight;
    const maxScrollTop = scrollHeight - height;
    chatEl.scrollTop = maxScrollTop > 0 ? maxScrollTop : 0;
  }, [transcripts]);

  const handleChatMessageSubmit = () => {
    if (!messageInputRef.current) {
      console.error('message input ref not found');
      return;
    }
    const messageValue = messageInputRef.current.value;
    if (!messageValue) {
      return;
    }
    onMessageSubmit(messageValue.trim());
    messageInputRef.current.value = '';
  };

  const handleInputKeyDown = (event: KeyboardEvent<HTMLInputElement>) => {
    if (event.key === 'Enter') {
      handleChatMessageSubmit();
    }
  };

  return (
    <div className='messages-chat'>
      <div className='videochat-transcripts' ref={videochatTranscriptsRef}>
        {transcripts.map(transcript => (
          <div key={transcript.frontendId}>
            <span>
              {!transcript.fromChat && `${Captions.Recognized} `}
            </span>
            <span
              style={{ color: stringToColor(transcript.userNickname) }}
            >
              {transcript.userNickname}
            </span>
            {': '}
            {transcript.value}
          </div>
        ))}
      </div>
      <div className='message-input-box'>
        <div className='message-input-wrapper'>
          <input
            type='text'
            placeholder={Captions.ChatMessagePlaceholder}
            ref={messageInputRef}
            onKeyDown={handleInputKeyDown}
          />
        </div>
        <div>
          <button onClick={handleChatMessageSubmit}>{Captions.SendToChat}</button>
        </div>
      </div>
    </div>
  );
};
