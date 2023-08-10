import {
  Audio,
  AudioListener,
} from 'three';
import { audioStore } from '@/core/loaders';
import { audioSlices } from '@/constants';

export type SliceName =
  'like1' |
  'like2' |
  'dislike1' |
  'dislike2' |
  'dislike3' |
  'dislike4' |
  'dislike5' |
  'dislike6' |
  'dislike7' |
  'dislike8' |
  'dislike9' |
  'dislike10' |
  'dislike11' |
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
      like1: this.createAudio('like1'),
      like2: this.createAudio('like2'),
      dislike1: this.createAudio('dislike1'),
      dislike2: this.createAudio('dislike2'),
      dislike3: this.createAudio('dislike3'),
      dislike4: this.createAudio('dislike4'),
      dislike5: this.createAudio('dislike5'),
      dislike6: this.createAudio('dislike6'),
      dislike7: this.createAudio('dislike7'),
      dislike8: this.createAudio('dislike8'),
      dislike9: this.createAudio('dislike9'),
      dislike10: this.createAudio('dislike10'),
      dislike11: this.createAudio('dislike11'),
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
