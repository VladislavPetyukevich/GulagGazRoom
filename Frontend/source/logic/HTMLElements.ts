interface HTMLElementsProps {
  onControlsEnabled: (enabled: boolean) => void;
  onAudioVolumeUpdate: (volume: number) => void;
}

export class HTMLElements {
  blocker: HTMLElement;
  instructions: HTMLElement;
  authenticationEl: HTMLElement;
  renderContainer: HTMLElement;
  audioVolume: HTMLInputElement;
  audioVolumeValue: HTMLElement;
  editorContainer: HTMLElement;

  constructor(props: HTMLElementsProps) {
    this.blocker = this.getElementByIdOrError('blocker');
    this.instructions = this.getElementByIdOrError('instructions');
    this.authenticationEl = this.getElementByIdOrError('authentication');
    this.renderContainer = this.getElementByIdOrError('render-container');
    this.audioVolume = this.getElementByIdOrError('audio-volume') as HTMLInputElement;
    this.audioVolumeValue = this.getElementByIdOrError('audio-volume-value');
    this.editorContainer = this.getElementByIdOrError('editor-container');

    this.instructions.addEventListener('click', () => {
      this.setBlockerVisibility(false);
      props.onControlsEnabled(true);
    });

    this.audioVolume.addEventListener('input', event => {
      if (!event.target) {
        throw new Error('audioVolume event target not found');
      }
      const value = +(event.target as HTMLInputElement).value;
      this.updateAudioVolumeValue(value);
      props.onAudioVolumeUpdate(value);
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

  getRendererSize() {
    return {
      width: ~~this.renderContainer.offsetWidth,
      height: ~~this.renderContainer.offsetHeight
    };
  }

  setBlockerVisibility(isVisible: boolean) {
    this.blocker.style.visibility = isVisible ? 'visible' : 'hidden';
    this.blocker.style.transition = isVisible ? '1s' : '0s';
  }

  setAudioVolume(value: number) {
    this.audioVolume.value = String(value);
  }

  getAudioVolume() {
    return +this.audioVolume.value;
  }

  displayAuthentication() {
    this.instructions.style.visibility = 'hidden';
    this.authenticationEl.style.visibility = 'visible';
  }
}
