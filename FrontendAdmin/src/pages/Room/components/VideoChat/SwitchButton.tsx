import { FunctionComponent } from 'react';
import { Captions } from '../../../../constants';

import './SwitchButton.css';

interface SwitchButtonProps {
  enabled: boolean;
  caption: string;
  subCaption?: string;
  onClick: () => void;
}

export const SwitchButton: FunctionComponent<SwitchButtonProps> = ({
  enabled,
  caption,
  subCaption,
  onClick,
}) => {
  return (
    <div className="switch-button-container">
      <button
        className="switch-button"
        onClick={onClick}
      >
        {enabled ? caption : Captions.SwitchOff}
      </button>
      {!!subCaption && (
        <span className="switch-button-subcaption">{subCaption}</span>
      )}
    </div>
  );
};
