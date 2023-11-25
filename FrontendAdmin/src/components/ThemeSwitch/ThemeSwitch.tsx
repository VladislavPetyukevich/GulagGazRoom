import { FunctionComponent, useContext } from 'react';
import { Theme, ThemeContext } from '../../context/ThemeContext';
import { Captions } from '../../constants';

import './ThemeSwitch.css';

const themeLocalization: Record<Theme, string> = {
  [Theme.System]: Captions.ThemeSystem,
  [Theme.Light]: Captions.ThemeLight,
  [Theme.Dark]: Captions.ThemeDark,
};

export const ThemeSwitch: FunctionComponent = () => {
  const { themeInSetting, setTheme } = useContext(ThemeContext);

  return (
    <div className='theme-switch'>
      <div>{Captions.Theme}:</div>
      {Object.entries(Theme).map(([_, themeValue]) => (
        <div key={themeValue}>
          <input
            type="checkbox"
            id={themeValue}
            checked={themeInSetting === themeValue}
            onChange={() => setTheme(themeValue)}
          />
          <label htmlFor={themeValue}>{themeLocalization[themeValue]}</label>
        </div>
      ))}
    </div>
  )
};
