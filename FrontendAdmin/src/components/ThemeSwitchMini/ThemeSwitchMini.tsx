import { FunctionComponent, useContext } from 'react';
import { Theme, ThemeContext } from '../../context/ThemeContext';
import { IconNames, IconThemePostfix } from '../../constants';

import './ThemeSwitchMini.css';

interface ThemeSwitchMiniProps {
  className?: string | null;
}

const getNextTheme = (themeInUi: Theme) =>
themeInUi === Theme.Light ? Theme.Dark : Theme.Light;

export const ThemeSwitchMini: FunctionComponent<ThemeSwitchMiniProps> = ({
  className,
}) => {
  const { themeInUi, setTheme } = useContext(ThemeContext);
  const iconPostfix = themeInUi === Theme.Dark ? IconThemePostfix.Dark : IconThemePostfix.Light;
  const iconName = themeInUi === Theme.Light ? IconNames.ThemeSwitchDark : IconNames.ThemeSwitchLight;

  const handleSwitch = () => {
    setTheme(getNextTheme(themeInUi));
  }

  return (
    <div
      className={`theme-switch-mini ${className}`}
      onClick={handleSwitch}
    >
      <ion-icon name={`${iconName}${iconPostfix}`}></ion-icon>
    </div>
  );
};
