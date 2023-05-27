interface HTMLElementsProps {
  onControlsEnabled: (enabled: boolean) => void;
  onAudioVolumeUpdate: (volume: number) => void;
  onFovUpdate: (fov: number) => void;
}

export class HTMLElements {
  blocker: HTMLElement;
  instructions: HTMLElement;
  authenticationEl: HTMLElement;
  renderContainer: HTMLElement;
  settings: HTMLElement;
  audioVolume: HTMLInputElement;
  audioVolumeValue: HTMLElement;
  fov: HTMLInputElement;
  fovValue: HTMLElement;

  constructor(props: HTMLElementsProps) {
    this.blocker = this.getElementByIdOrError('blocker');
    this.instructions = this.getElementByIdOrError('instructions');
    this.authenticationEl = this.getElementByIdOrError('authentication');
    this.renderContainer = this.getElementByIdOrError('render-container');
    this.settings = this.getElementByIdOrError('settings');
    this.audioVolume = this.getElementByIdOrError('audio-volume') as HTMLInputElement;
    this.audioVolumeValue = this.getElementByIdOrError('audio-volume-value');
    this.fov = this.getElementByIdOrError('fov') as HTMLInputElement;
    this.fovValue = this.getElementByIdOrError('fov-value');

    this.instructions.addEventListener('click', () => {
      this.setBlockerVisibility(false);
      props.onControlsEnabled(true);
    });

    this.settings.addEventListener('click', event => event.stopPropagation());

    this.audioVolume.addEventListener('input', event => {
      if (!event.target) {
        throw new Error('audioVolume event target not found');
      }
      const value = +(event.target as HTMLInputElement).value;
      this.updateAudioVolumeValue(value);
      props.onAudioVolumeUpdate(value);
    });

    this.fov.addEventListener('input', event => {
      if (!event.target) {
        throw new Error('fov event target not found');
      }
      const value = +(event.target as HTMLInputElement).value;
      this.updateFovValue(value);
      props.onFovUpdate(value);
    });
  }

  getElementByIdOrError(id: string) {
    const el = document.getElementById(id);
    if (!el) {
      throw new Error(`Failed to get element with id: ${id}`);
    }
    return el;
  }

  updateAudioVolumeValue(value: number) {
    const valueNode = document.createTextNode(value.toFixed(1));
    this.audioVolumeValue.innerHTML = '';
    this.audioVolumeValue.appendChild(valueNode);
  }

  updateFovValue(value: number) {
    const valueNode = document.createTextNode(value + '');
    this.fovValue.innerHTML = '';
    this.fovValue.appendChild(valueNode);
  }

  getRendererSize() {
    return {
      width: ~~this.renderContainer.offsetWidth,
      height: ~~this.renderContainer.offsetHeight
    };
  }

  setBlockerVisibility(isVisible: boolean) {
    this.blocker.style.opacity = isVisible ? '1' : '0';
    this.blocker.style.transition = isVisible ? '1s' : '0s';
  }

  setAudioVolume(value: number) {
    this.audioVolume.value = String(value);
  }

  setFov(value: number) {
    this.fov.value = String(value);
  }

  getAudioVolume() {
    return +this.audioVolume.value;
  }

  getFov() {
    return +this.fov.value;
  }

  displayAuthentication() {
    this.instructions.style.visibility = 'hidden';
    this.authenticationEl.style.visibility = 'visible';
  }
}
