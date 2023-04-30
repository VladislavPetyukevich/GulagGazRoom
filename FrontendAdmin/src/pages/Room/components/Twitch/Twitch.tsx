import { FunctionComponent, memo, useRef } from 'react';
import { TwitchEmbed, TwitchEmbedInstance } from 'react-twitch-embed';

export interface TwitchProps {
  autoplay: boolean;
  channel: string;
}

export const Twitch: FunctionComponent<TwitchProps> = memo(({
  autoplay,
  channel,
}) => {
  const embed = useRef<TwitchEmbedInstance>();

  const handleReady = (e: TwitchEmbedInstance) => {
    embed.current = e;
  };

  return (
    <TwitchEmbed
      autoplay={autoplay}
      channel={channel}
      withChat
      darkMode={true}
      onVideoReady={handleReady}
    />
  );
});
