import * as monaco from 'monaco-editor';

export class CodeEditor {
  container: HTMLElement
  editor: monaco.editor.IStandaloneCodeEditor;

  constructor(container: HTMLElement) {
    this.container = container;
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
  }

  show() {
    this.container.style.visibility = 'visible';
  }

  hide() {
    this.container.style.visibility = 'hidden';
  }
}
