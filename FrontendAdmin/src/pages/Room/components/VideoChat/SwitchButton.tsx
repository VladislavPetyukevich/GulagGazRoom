import { FunctionComponent } from 'react';

import './SwitchButton.css';

interface SwitchButtonProps {
  enabled: boolean;
  iconEnabledName: string;
  iconDisabledName: string;
  disabledColor?: boolean;
  subCaption?: string;
  onClick: () => void;
}

export const SwitchButton: FunctionComponent<SwitchButtonProps> = ({
  enabled,
  iconDisabledName,
  iconEnabledName,
  disabledColor,
  subCaption,
  onClick,
}) => {
  const iconName = enabled ? iconEnabledName : iconDisabledName;

  return (
    <div className="switch-button-container">
      <button
        className={`switch-button ${(!enabled && disabledColor) ? 'switch-button-disabled' : ''}`}
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
