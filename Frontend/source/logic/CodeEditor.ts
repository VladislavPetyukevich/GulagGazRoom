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

const defaultFontSize = 22;
const fontSizeList = [10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48];

export class CodeEditor {
  container: HTMLElement;
  settingsPanelWrapper?: HTMLDivElement;
  languageSelect?: HTMLSelectElement;
  fontSizeSelect?: HTMLSelectElement;
  editor: monaco.editor.IStandaloneCodeEditor;
  onChange?: (value: string) => void;

  private ignoreChange: boolean;

  constructor(container: HTMLElement) {
    this.container = container;
    this.ignoreChange = false;
    this.settingsPanelWrapper = this.createSettingsPanel();
    monaco.languages.typescript.javascriptDefaults.setCompilerOptions({
      noLib: true,
      allowNonTsExtensions: true,
    });
    const languages =
      monaco.languages.getLanguages()
      .filter(language => includedLanguagesId.includes(language.id));
    this.createLanguagesSelect(languages);
    const fontSize = defaultFontSize;
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
    this.fontSizeSelect = document.createElement('select');
    for (let fontSize of fontSizeList) {
      const option = document.createElement('option');
      option.value = String(fontSize);
      option.textContent = String(fontSize);
      this.fontSizeSelect.appendChild(option);
    }
    this.fontSizeSelect.value = String(defaultFontSize);
    this.fontSizeSelect.onchange = this.handleFontSizeChange;
    this.settingsPanelWrapper.appendChild(this.fontSizeSelect);
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
    if (!this.fontSizeSelect) {
      console.error('Change font size error: font size select not found');
      return;
    }
    const fontSize = Number(this.fontSizeSelect.value);
    this.editor.updateOptions({ fontSize });
  };
}
