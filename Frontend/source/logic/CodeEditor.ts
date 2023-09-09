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
  settingsPanelWrapper?: HTMLDivElement;
  languageSelect?: HTMLSelectElement;
  fontSizeInput?: HTMLInputElement;
  editor: monaco.editor.IStandaloneCodeEditor;
  onChange?: (value: string) => void;

  private ignoreChange: boolean;

  constructor(container: HTMLElement) {
    this.container = container;
    this.ignoreChange = false;
    this.hide();

    this.settingsPanelWrapper = this.createSettingsPanel();
    monaco.languages.typescript.javascriptDefaults.setCompilerOptions({
      noLib: true,
      allowNonTsExtensions: true,
    });
    const languages =
      monaco.languages.getLanguages()
      .filter(language => includedLanguagesId.includes(language.id));
    this.createLanguagesSelect(languages);
    const fontSize = 22;
    this.createFontSizeInput(fontSize);
    this.container.appendChild(this.settingsPanelWrapper);

    this.editor = monaco.editor.create(container, {
      fontSize: fontSize,
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

  switchVisibility() {
    const isVisible = this.container.style.visibility === 'visible';
    if (isVisible) {
      this.hide();
    } else {
      this.show()
    }
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

  createSettingsPanel() {
    const settingsPanelWrapper = document.createElement('div');
    settingsPanelWrapper.classList.add('settings-panel-wrapper');
    return settingsPanelWrapper;
  }

  createLanguagesSelect(languages: monaco.languages.ILanguageExtensionPoint[]) {
    if (!this.settingsPanelWrapper) {
      return;
    }
    this.settingsPanelWrapper.appendChild(document.createTextNode(`${Captions.Language}:`));
    this.languageSelect = document.createElement('select');
    this.languageSelect.onchange = this.handleLanguageChange;
    for (let language of languages) {
      const option = document.createElement('option');
      option.textContent = this.getLanguageDisplayName(language);
      option.value = language.id;
      this.languageSelect.appendChild(option);
    }
    this.settingsPanelWrapper.appendChild(this.languageSelect);
  }

  createFontSizeInput(fontSize: number) {
    if (!this.settingsPanelWrapper) {
      return;
    }
    this.settingsPanelWrapper.appendChild(document.createTextNode(` ${Captions.FontSize}:`));
    this.fontSizeInput = document.createElement('input');
    this.fontSizeInput.type = 'number';
    this.fontSizeInput.min = '6';
    this.fontSizeInput.max = '36';
    this.fontSizeInput.value = String(fontSize);
    this.fontSizeInput.onchange = this.handleFontSizeChange;
    this.settingsPanelWrapper.appendChild(this.fontSizeInput);
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

  handleFontSizeChange = () => {
    if (!this.fontSizeInput) {
      console.error('Change font size error: font size input not found');
      return;
    }
    const fontSize = Number(this.fontSizeInput.value);
    this.editor.updateOptions({ fontSize });
  };
}
