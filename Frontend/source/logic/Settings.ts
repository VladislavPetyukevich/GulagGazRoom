interface SettingsData {
  audioVolume: number;
  fov: number;
}

export class Settings {
  localStorageKey: string;

  constructor() {
    this.localStorageKey = 'settings';
  }

  save(data: SettingsData) {
    localStorage.setItem(this.localStorageKey, JSON.stringify(data));
  }

  load(): SettingsData {
    return JSON.parse(localStorage.getItem(this.localStorageKey) || '');
  }
}
