import { Color } from 'three';
import wallTextureFile from '@/assets/wall.png';
import wallDecal1TextureFile from '@/assets/wallDecal1.png';
import wallDecal2TextureFile from '@/assets/wallDecal2.png';
import wallDecal3TextureFile from '@/assets/wallDecal3.png';
import floorTextureFile from '@/assets/floor.png';
import damage from './assets/damage.mp3';

export const PI_2 = Math.PI / 2;

export const RANDOM_NUMBERS_COUNT = 100;

export const enum ENTITY_TYPE {
  PLAYER = 'PLAYER',
  WALL = 'WALL',
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
};

export const gameSounds = {
  damage,
};
