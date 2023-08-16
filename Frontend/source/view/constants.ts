import { Color } from 'three';
import wallTextureFile from '@/assets/wall.png';
import wallDecal1TextureFile from '@/assets/wallDecal1.png';
import wallDecal2TextureFile from '@/assets/wallDecal2.png';
import wallDecal3TextureFile from '@/assets/wallDecal3.png';
import floorTextureFile from '@/assets/floor.png';
import gasTextureFile from '@/assets/smoke.png';
import damage from './assets/damage.mp3';
import buzz from './assets/buzz.mp3';
import lightFlicks from './assets/lightFlicks.mp3';
import gazBeat1 from './assets/gazBeat1.mp3';
import gazBeat2 from './assets/gazBeat2.mp3';
import gulagVox from './assets/gulagVox.mp3';

export const DEFAULT_FOV = 105;

export const PI_2 = Math.PI / 2;

export const RANDOM_NUMBERS_COUNT = 500;

export const enum ENTITY_TYPE {
  PLAYER = 'PLAYER',
  WALL = 'WALL',
  GAS = 'GAS',
  TV = 'TV',
};

const createColor = (r: number, g: number, b: number) => {
  return new Color(r / 255, g / 255, b / 255);
};

const DarkColor = createColor(0, 0, 0);

const lerpColor = (colorOriginal: Color, color: Color, factor: number) => {
  const newColor = colorOriginal.clone();
  return newColor.lerp(color, factor);
};

export const darker = (color: Color, factor: number) => {
  return lerpColor(color, DarkColor, factor);
};

const wallDarkerFactor = 0.915;
export const WALL_COLORS = {
  Neutral: darker(createColor(180, 180, 180), wallDarkerFactor),
};

export const WALL = {
  SIZE: 3
};

export const PLAYER = {
  HP: 10,
  BODY_WIDTH: 1.5,
  BODY_HEIGHT: 1.5,
  BODY_DEPTH: 1.5,
};

export const gameTextures = {
  wallTextureFile,
  wallDecal1TextureFile,
  wallDecal2TextureFile,
  wallDecal3TextureFile,
  floorTextureFile,
  gasTextureFile,
};

export const gameSounds = {
  damage,
  buzz,
  lightFlicks,
  gazBeat1,
  gazBeat2,
  gulagVox,
};

const voxVolume = 0.15;

export const audioSlices = {
  like1: {
    bufferName: 'gulagVox',
    start: 0.0, end: 1.65, volume: voxVolume,
  },
  like2: {
    bufferName: 'gulagVox',
    start: 17.21, end: 18.42, volume: voxVolume,
  },
  dislike1: {
    bufferName: 'gulagVox',
    start: 7.77, end: 8.59, volume: voxVolume,
  },
  dislike2: {
    bufferName: 'gulagVox',
    start: 5.08, end: 6.76, volume: voxVolume,
  },
  dislike3: {
    bufferName: 'gulagVox',
    start: 1.66, end: 3.43, volume: voxVolume,
  },
  dislike4: {
    bufferName: 'gulagVox',
    start: 3.57, end: 5.03, volume: voxVolume,
  },
  dislike5: {
    bufferName: 'gulagVox',
    start: 6.83, end: 7.67, volume: voxVolume,
  },
  dislike6: {
    bufferName: 'gulagVox',
    start: 8.74, end: 9.67, volume: voxVolume,
  },
  dislike7: {
    bufferName: 'gulagVox',
    start: 9.80, end: 13.97, volume: voxVolume,
  },
  dislike8: {
    bufferName: 'gulagVox',
    start: 13.97, end: 15.36, volume: voxVolume,
  },
  dislike9: {
    bufferName: 'gulagVox',
    start: 15.36, end: 17.18, volume: voxVolume,
  },
  dislike10: {
    bufferName: 'gulagVox',
    start: 18.57, end: 20.27, volume: voxVolume,
  },
  dislike11: {
    bufferName: 'gulagVox',
    start: 20.30, end: 22.31, volume: voxVolume,
  },
  lightFlick1: {
    bufferName: 'lightFlicks',
    start: 0.05, end: 0.39, volume: 0.5,
  },
  lightFlick2: {
    bufferName: 'lightFlicks',
    start: 0.39, end: 0.6, volume: 0.5,
  },
  lightFlick3: {
    bufferName: 'lightFlicks',
    start: 0.6, end: 0.91, volume: 0.5,
  },
  lightFlick4: {
    bufferName: 'lightFlicks',
    start: 0.91, end: 1.22, volume: 0.5,
  },
};
