import { FunctionComponent, useContext } from 'react';
import { Theme, ThemeContext } from '../../context/ThemeContext';

import './ThemeSwitchMini.css';

const getNextTheme = (themeInUi: Theme) =>
themeInUi === Theme.Light ? Theme.Dark : Theme.Light;

export const ThemeSwitchMini: FunctionComponent = () => {
  const { themeInUi, setTheme } = useContext(ThemeContext);

  const handleSwitch = () => {
    setTheme(getNextTheme(themeInUi));
  }

  return (
    <div
      className='theme-switch-mini'
      onClick={handleSwitch}
    >
      {themeInUi === Theme.Light ? '🌙' : '💡'}
    </div>
  );
};
