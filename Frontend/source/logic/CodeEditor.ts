import * as monaco from 'monaco-editor';
import { Captions } from './constants';

const includedLanguagesId = [
  'plaintext',
  'c',
  'cpp',
  'csharp',
  'css',
  'go',
  'html',
  'java',
  'javascript',
  'kotlin',
  'mysql',
  'php',
  'python',
  'ruby',
  'rust',
  'sql',
  'swift',
  'typescript',
  'xml',
  'yaml',
];

export class CodeEditor {
  container: HTMLElement;
  languageSelect?: HTMLSelectElement;
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
    const languages =
      monaco.languages.getLanguages()
      .filter(language => includedLanguagesId.includes(language.id));

    this.createLanguagesSelect(languages);
    this.editor = monaco.editor.create(container, {
      fontSize: 22,
      theme: 'vs-dark',
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

  createLanguagesSelect(languages: monaco.languages.ILanguageExtensionPoint[]) {
    const languageSelectWrapper = document.createElement('div');
    languageSelectWrapper.classList.add('language-select-wrapper');
    languageSelectWrapper.appendChild(document.createTextNode(`${Captions.Language}:`));

    this.languageSelect = document.createElement('select');
    this.languageSelect.classList.add('language-select');
    this.languageSelect.onchange = this.handleLanguageChange;
    for (let language of languages) {
      const option = document.createElement('option');
      option.textContent = this.getLanguageDisplayName(language);
      option.value = language.id;
      this.languageSelect.appendChild(option);
    }
    languageSelectWrapper.appendChild(this.languageSelect);
    this.container.appendChild(languageSelectWrapper);
  }

  getLanguageDisplayName(language: monaco.languages.ILanguageExtensionPoint) {
    const alias = language.aliases && language.aliases[0];
    if (alias) {
      return alias;
    }
    return language.id;
  }

  handleLanguageChange = () => {
    if (!this.languageSelect) {
      console.error('Change language error: language select not found');
      return;
    }
    const languageId = this.languageSelect.value;
    const model = this.editor.getModel();
    if (!model) {
      console.error('Change language error: editor model not found');
      return;
    }
    monaco.editor.setModelLanguage(model, languageId);
  };
}
