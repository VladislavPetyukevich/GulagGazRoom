import * as monaco from 'monaco-editor';

export class CodeEditor {
  container: HTMLElement
  editor: monaco.editor.IStandaloneCodeEditor;
  onChange?: (value: string) => void;

  private ignoreChange: boolean;

  constructor(container: HTMLElement) {
    this.container = container;
    this.ignoreChange = false;
    this.hide();
    monaco.languages.typescript.javascriptDefaults.setCompilerOptions({
      noLib: true,
      allowNonTsExtensions: true,
    });
    this.editor = monaco.editor.create(container, {
      fontSize: 22,
      theme: 'vs-dark',
      language: 'javascript',
      automaticLayout: true,
      minimap: { enabled: false },
    });
    this.editor.onDidChangeModelContent(() => {
      if (this.ignoreChange) {
        this.ignoreChange = false;
        return;
      }
      if (this.onChange) {
        this.onChange(this.editor.getValue());
      }
    });
  }

  show() {
    this.container.style.visibility = 'visible';
  }

  hide() {
    this.container.style.visibility = 'hidden';
  }

  setValue(value: string) {
    this.ignoreChange = true;
    const cursorPosition = this.editor.getPosition();
    this.editor.setValue(value);
    if (cursorPosition) {
      this.editor.setPosition(cursorPosition);
    }
  }

  setReadonly(value: boolean) {
    this.editor.updateOptions({ readOnly: value });
  }
}
