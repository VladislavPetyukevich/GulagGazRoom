import { FunctionComponent } from 'react';

interface SwitchButtonProps {
  enabled: boolean;
  caption: string;
  onClick: () => void;
}

export const SwitchButton: FunctionComponent<SwitchButtonProps> = ({
  enabled,
  caption,
  onClick,
}) => {
  return (
    <button
      className="videochat-caption"
      onClick={onClick}
    >
      {caption} {enabled ? '✔' : '❌'}
    </button>
  );
};
