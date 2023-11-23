interface HTMLElementsProps {
  onControlsEnabled: (enabled: boolean) => void;
  onAudioVolumeUpdate: (volume: number) => void;
}

export class HTMLElements {
  authenticationEl: HTMLElement;
  editorContainer: HTMLElement;

  constructor() {
    this.authenticationEl = this.getElementByIdOrError('authentication');
    this.editorContainer = this.getElementByIdOrError('editor-container');
  }

  getElementByIdOrError(id: string) {
    const el = document.getElementById(id);
    if (!el) {
      throw new Error(`Failed to get element with id: ${id}`);
    }
    return el;
  }

  displayAuthentication() {
    this.authenticationEl.style.visibility = 'visible';
  }
}
