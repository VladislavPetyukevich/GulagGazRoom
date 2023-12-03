import { FunctionComponent } from 'react';

import './SwitchButton.css';

interface SwitchButtonProps {
  enabled: boolean;
  iconEnabledName: string;
  iconDisabledName: string;
  subCaption?: string;
  onClick: () => void;
}

export const SwitchButton: FunctionComponent<SwitchButtonProps> = ({
  enabled,
  iconDisabledName,
  iconEnabledName,
  subCaption,
  onClick,
}) => {
  const iconName = enabled ? iconEnabledName : iconDisabledName;

  return (
    <div className="switch-button-container">
      <button
        className="switch-button"
        onClick={onClick}
      >
        <ion-icon name={iconName}></ion-icon>
      </button>
      {!!subCaption && (
        <span className="switch-button-subcaption">{subCaption}</span>
      )}
    </div>
  );
};
