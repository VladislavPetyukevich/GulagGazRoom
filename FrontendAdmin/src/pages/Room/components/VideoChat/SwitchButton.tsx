import { FunctionComponent, useContext } from 'react';
import { Theme, ThemeContext } from '../../../../context/ThemeContext';
import { IconThemePostfix } from '../../../../constants';

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
  const { themeInUi } = useContext(ThemeContext);
  const iconPostfix = themeInUi === Theme.Dark ? IconThemePostfix.Dark : IconThemePostfix.Light;
  const iconName = enabled ? iconEnabledName : iconDisabledName;

  return (
    <div className="switch-button-container">
      <button
        className={`switch-button ${(!enabled && disabledColor) ? 'switch-button-disabled' : ''}`}
        onClick={onClick}
      >
        <ion-icon name={`${iconName}${iconPostfix}`}></ion-icon>
      </button>
      {!!subCaption && (
        <span className="switch-button-subcaption">{subCaption}</span>
      )}
    </div>
  );
};
