import { FunctionComponent } from 'react';
import { ThemedIcon } from '../ThemedIcon/ThemedIcon';
import { IconNames } from '../../../../constants';

import './SwitchButton.css';

interface SwitchButtonProps {
  enabled: boolean;
  iconEnabledName: IconNames;
  iconDisabledName: IconNames;
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
        <ThemedIcon name={iconName} />
      </button>
      {!!subCaption && (
        <span className="switch-button-subcaption">{subCaption}</span>
      )}
    </div>
  );
};
