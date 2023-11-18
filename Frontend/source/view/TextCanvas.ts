import { Sprite, CanvasTexture, SpriteMaterial } from 'three';

interface TextCanvasProps {
  size: ImageSize;
  textAlign?: TextCanvas['textAlign'];
}

interface ImageSize {
  width: number;
  height: number;
}

export class TextCanvas {
  fontSize: number;
  textAlign: CanvasRenderingContext2D['textAlign'];
  sprite: Sprite;
  texture: CanvasTexture;
  canvas: HTMLCanvasElement;
  context: CanvasRenderingContext2D;
  textX: number;
  maxLineLength: number;
  size: ImageSize;
  padding: number;

  constructor(props: TextCanvasProps) {
    this.fontSize = 32;
    this.maxLineLength = 28;
    this.size = props.size;
    this.padding = 160;
    this.textAlign = props.textAlign || 'left';
    this.canvas = this.initCanvas(props.size);
    this.context = this.initContext();
    this.texture = new CanvasTexture(this.canvas);
    const material = new SpriteMaterial({ map: this.texture });
    this.sprite = new Sprite(material);
    this.textX = this.getTextX();
  }

  clear() {
    this.context.clearRect(0, 0, this.canvas.width, this.canvas.height);
  }

  printAll(text: string) {
    const lines = text.split('\n').map((textPart) => this.wordWrap(textPart));
    const flatLines = lines.reduce((acc, curVal) => {
      return acc.concat(curVal)
    }, []);
    flatLines.forEach(
      (line, lineIndex) => this.print(line, lineIndex + 1)
    );
  }

  wordWrap(text: string) {
    const words = text.split(' ');
    const lines = [];
    let currentLine = '';
  
    words.forEach((word) => {
      if ((currentLine + word).length <= this.maxLineLength) {
        currentLine += word + ' ';
      } else {
        if (currentLine !== '') {
          lines.push(currentLine.trim());
        }
        currentLine = word + ' ';
      }
    });

    if (currentLine.length > 0) {
      lines.push(currentLine.trim());
    }

    return lines;
  }

  print(text: string, line: number) {
    this.context.fillText(`${text}`, this.textX, this.fontSize * line, this.size.width - this.padding);
    this.sprite.material.map && (this.sprite.material.map.needsUpdate = true);
  }

  initCanvas(size: ImageSize) {
    const canvas = document.createElement('canvas');
    canvas.setAttribute('width', `${size.width}`);
    canvas.setAttribute('height', `${size.height}`);
    return canvas;
  }

  initContext() {
    const context = this.canvas.getContext('2d');
    if (!context) {
      throw new Error('TextCanvas: Canvas context not found');
    }
    context.font = `${this.fontSize}px sans-serif`;
    context.fillStyle = 'white';
    context.textAlign = this.textAlign;
    return context;
  }

  getTextX() {
    if (this.textAlign === 'center') {
      return Math.round(this.canvas.width / 2);
    }
    return 0;
  }
}
