import {
  Audio,
  AudioListener,
} from 'three';
import { audioStore } from '@/core/loaders';
import { audioSlices } from '@/constants';

type SliceName =
  'like' |
  'dislike' |
  'lightFlick1' |
  'lightFlick2' |
  'lightFlick3' |
  'lightFlick4';

interface SliceInfo {
  bufferName: string;
  start: number;
  end: number;
  volume: number;
}

export interface AudioSlicesProps {
  audioListener: AudioListener;
}

export class AudioSlices {
  props: AudioSlicesProps;
  slicesInfo: Record<SliceName, SliceInfo>;
  slices: Record<SliceName, Audio>;

  constructor(props: AudioSlicesProps) {
    this.props = props;
    this.slicesInfo = audioSlices;

    this.slices = {
      like: this.createAudio('like'),
      dislike: this.createAudio('dislike'),
      lightFlick1: this.createAudio('lightFlick1'),
      lightFlick2: this.createAudio('lightFlick2'),
      lightFlick3: this.createAudio('lightFlick3'),
      lightFlick4: this.createAudio('lightFlick4'),
    };
  }

  getAudio(sliceName: SliceName) {
    return this.slices[sliceName];
  }

  createAudio(voxName: SliceName) {
    const audio = new Audio(this.props.audioListener);
    const sliceInfo = this.slicesInfo[voxName];
    audio.setBuffer(audioStore.getSound(sliceInfo.bufferName));
    audio.offset = sliceInfo.start;
    audio.duration = sliceInfo.end - sliceInfo.start;
    audio.setVolume(sliceInfo.volume);
    return audio;
  }
}
