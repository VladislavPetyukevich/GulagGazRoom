export class Speech {
  lang: string;
  speechSynthesisUtterance: SpeechSynthesisUtterance;

  constructor() {
    if (!window.speechSynthesis) {
      throw new Error('SpeechSynthesis not supported');
    }
    this.lang = 'ru-RU';
    this.speechSynthesisUtterance = new SpeechSynthesisUtterance();
    this.speechSynthesisUtterance.rate = 1.5;
    this.speechSynthesisUtterance.pitch = 2.0;
    setTimeout(() => {
      const voice = this.findVoice();
      if (!voice) {
        throw new Error(`Voice with lang ${this.lang} not found`);
      }
      this.speechSynthesisUtterance.voice = voice;
    }, 1000);
  }

  setVolume(volume: number) {
    this.speechSynthesisUtterance.volume = volume;
  }

  findVoice() {
    return speechSynthesis.getVoices().find(voice => voice.lang === this.lang);
  }

  speak(text: string) {
    this.speechSynthesisUtterance.text = text;
    window.speechSynthesis.speak(this.speechSynthesisUtterance);
  }
}
