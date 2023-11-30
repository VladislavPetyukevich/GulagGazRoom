import { FunctionComponent, useContext } from 'react';
import { Theme, ThemeContext } from '../../context/ThemeContext';

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

  const handleSwitch = () => {
    setTheme(getNextTheme(themeInUi));
  }

  return (
    <div
      className={`theme-switch-mini ${className}`}
      onClick={handleSwitch}
    >
      {themeInUi === Theme.Light ? '🌙' : '💡'}
    </div>
  );
};
