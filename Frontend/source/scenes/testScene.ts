import {
  PointLight,
  Vector2,
  Vector3,
  Fog,
  AmbientLight,
  Audio,
} from 'three';
import { FBXLoader } from 'three/examples/jsm/loaders/FBXLoader';
import { BasicSceneProps, BasicScene } from '@/core/Scene';
import { PLAYER } from '@/constants';
import { Player } from '@/Entities/Player/Player';
import { Gas } from '@/Entities/Gas/Gas';
import { TV } from '@/Entities/TV/TV';
import { Room, RoomSpawner } from './Spawner/RoomSpawner';
import { CellCoordinates } from './CellCoordinates';
import { randomNumbers } from '@/RandomNumbers';
import HomePakTV from '@/assets/HomePakTV.fbx';
import { TimeoutsManager } from '@/TimeoutsManager';
import { EntitiesPool } from './Spawner/EntitiesPool';
import { PlayerAction, PlayerActionListener, PlayerActionName, playerActions } from '@/PlayerActions';
import { Stats } from './Stats';
import { audioStore } from '@/core/loaders';

interface LightEffect {
  colorHex: number;
  intensity: number;
  duration: number;
}

type LightEffectName = 'flick';

export interface TestSceneProps extends BasicSceneProps {
  preload: {
    question?: string;
    likes: number;
    dislikes: number;
  },
  onFinish: Function;
}

type TimeoutNames =
  'lightFlick';

export class TestScene extends BasicScene {
  pointLight: PointLight;
  ambientLight: AmbientLight;
  ambientLightColor: number;
  ambientLightIntensity: number;
  player: Player;
  cellCoordinates: CellCoordinates;
  currentRoom: Room;
  roomSpawner: RoomSpawner;
  timeoutsManager: TimeoutsManager<TimeoutNames>;
  onFinish: Function;
  gasCenter: Vector3;
  gasParticlesPool: EntitiesPool;
  actions: Record<PlayerActionName, PlayerActionListener['listener']>;
  gasEnabled: boolean;
  lightEffects: Record<LightEffectName, LightEffect>;
  buzzSound: Audio;
  lightFlickAudios: Audio[];
  likeAudio: Audio;
  dislikeAudio: Audio;
  gasAudios: Audio[];
  gasAudioIndex: number;
  stats: Stats;
  tvMain?: TV;
  tvChat?: TV;
  tvStats?: TV;
  tvStatsAnimationInProgress: boolean;

  constructor(props: TestSceneProps) {
    super(props);
    const timeoutValues = {
      lightFlick: randomNumbers.getRandomInRange(1, 2),
    };
    this.timeoutsManager = new TimeoutsManager(timeoutValues);

    this.onFinish = props.onFinish;
    this.player = this.entitiesContainer.add(
      new Player({
        camera: this.camera,
        position: new Vector3(0, PLAYER.BODY_HEIGHT, 0),
        container: this.entitiesContainer,
        audioListener: this.audioListener
      })
    ) as Player;
    this.player.setOnDeathCallback(() => {
      this.ambientLight.color.setHex(0xFF0000);
      setTimeout(() => this.finish(), 400);
    });
    this.cellCoordinates = new CellCoordinates({
      size: 3,
    });
    const roomSizeScale = 2;
    this.roomSpawner = new RoomSpawner({
      scene: this,
      player: this.player,
      entitiesContainer: this.entitiesContainer,
      cellCoordinates: this.cellCoordinates,
      roomSize: new Vector2(10 * roomSizeScale, 10 * roomSizeScale),
      doorWidthHalf: 1,
    });
    this.currentRoom = this.roomSpawner.createRoom(new Vector2(0, 0));

    this.camera.rotation.y = 0;

    // lights
    this.ambientLightColor = 0x404040;
    this.ambientLightIntensity = 70;
    this.ambientLight = new AmbientLight(
      this.ambientLightColor,
      this.ambientLightIntensity
    );
    this.scene.add(this.ambientLight);
    const pointLightColor = 0xFFFFFF;
    const pointLightIntensity = 110;
    const pointLightDistance = 130;
    this.pointLight = new PointLight(
      pointLightColor,
      pointLightIntensity,
      pointLightDistance
    );
    this.scene.add(this.pointLight);

    this.scene.fog = new Fog(0x202020, 0.15, 150);

    this.stats = new Stats();
    this.stats.setCount('like', props.preload.likes);
    this.stats.setCount('dislike', props.preload.dislikes);

    const loader = new FBXLoader();
    loader.load(HomePakTV, (object) => {
      this.tvMain = this.entitiesContainer.add(new TV({
        model: object,
        position: new Vector3(30, 0.8, 44),
        rotationY: 0,
        screenSpinSpeed: 0.2,
        screenSpinAxis: 'y',
      })) as TV;
      this.scene.add(object);
      this.tvMain.printText(props.preload.question || 'Здесь будет вопрос');
    });

    loader.load(HomePakTV, (object) => {
      this.tvChat = this.entitiesContainer.add(new TV({
        model: object,
        position: new Vector3(24, 0.8, 45),
        rotationY: 0.436332,
        screenSpinSpeed: -2.0,
        screenSpinAxis: 'y',
      })) as TV;
      this.scene.add(object);
      this.tvChat.printText(
        'izede:\nЗа этим стоит лаборатория'
      );
    });

    loader.load(HomePakTV, (object) => {
      this.tvStats = this.entitiesContainer.add(new TV({
        model: object,
        position: new Vector3(24, 3.07, 45),
        rotationY: 0.436332,
        screenSpinSpeed: -0.1,
        screenSpinAxis: 'x',
      })) as TV;
      this.scene.add(object);
      this.updateStatsTv();
    });
    this.tvStatsAnimationInProgress = false;

    this.player.mesh.position.set(31.0, 1.5, 52.0);
    this.camera.rotation.set(0.0, 0.21, 0.0);

    this.roomSpawner.spawnWall(
      new Vector2(30, 45),
      new Vector2(this.roomSpawner.cellCoordinates.size * 15, this.roomSpawner.cellCoordinates.size),
      true
    );

    this.gasCenter = new Vector3(30.55, -2.0, 50.0);
    const gasParticlesCount = 160;
    this.gasParticlesPool = new EntitiesPool(this.createGasParticle, gasParticlesCount);

    this.buzzSound = new Audio(this.audioListener);
    const buzzSoundBuffer = audioStore.getSound('buzz');
    this.buzzSound.setBuffer(buzzSoundBuffer);
    this.buzzSound.setLoop(true);
    this.buzzSound.setVolume(0.4);

    this.lightEffects = {
      flick: {
        colorHex: 0xFFFFFF,
        intensity: 70.0,
        duration: 100,
      },
    };
    this.lightFlickAudios = [];
    for (let i = 1; i <= 4; i++) {
      this.lightFlickAudios.push(
        this.createAudio(`lightFlick${i}`)
      );
    }

    this.gasAudioIndex = 0;
    this.gasAudios = [];
    for (let i = 1; i <= 2; i++) {
      const gasAudio = this.createAudio(`gazBeat${i}`);
      gasAudio.setLoop(true);
      this.gasAudios.push(gasAudio);
    }

    this.likeAudio = new Audio(this.audioListener);
    this.likeAudio.setBuffer(audioStore.getSound('like'));
    this.likeAudio.setVolume(0.2);

    this.dislikeAudio = new Audio(this.audioListener);
    this.dislikeAudio.setBuffer(audioStore.getSound('dislike'));
    this.dislikeAudio.setVolume(0.3);

    this.gasEnabled = false;
    this.actions = {
      gasEnable: this.onGasEnable,
      gasDisable: this.onGasDisable,
      newQuestion: this.onQuestion,
      like: this.onLike,
      dislike: this.onDislike,
      chatMessage: this.onChatMessage,
    };
    this.addActionListeners();
  }

  getInitialPlayerPositon() {
    const roomCenterCell = this.getCenterPosition(this.currentRoom.cellPosition, this.roomSpawner.roomSize);
    const positionShift = 8;
    return new Vector2(
      roomCenterCell.x * this.cellCoordinates.size,
      (roomCenterCell.y + positionShift) * this.cellCoordinates.size
    );
  }

  resumeSounds() {
    this.buzzSound.play();
    if (
      this.gasEnabled &&
      !this.gasAudios[this.gasAudioIndex].isPlaying
    ) {
      this.gasAudios[this.gasAudioIndex].play();
    }
  }

  stopSounds() {
    this.buzzSound.stop();
    if (
      this.gasEnabled &&
      this.gasAudios[this.gasAudioIndex].isPlaying
    ) {
      this.gasAudios[this.gasAudioIndex].pause();
    }
  }

  setNextGasAudioIndex() {
    this.gasAudioIndex++;
    if (this.gasAudioIndex === this.gasAudios.length) {
      this.gasAudioIndex = 0;
    }
  }

  createAudio(soundName: string) {
    const audio = new Audio(this.audioListener);
    const audioBuffer = audioStore.getSound(soundName);
    audio.setBuffer(audioBuffer);
    audio.setVolume(0.5);
    return audio;
  }

  getCenterPosition(position: Vector2, size: Vector2) {
    return new Vector2(
      position.x + size.x / 2,
      position.y + size.y / 2
    );
  }

  playAudio(audio: Audio) {
    if (audio.isPlaying) {
      audio.stop();
    }
    audio.play();
  }

  createLightEffect(effect: LightEffect) {
    this.ambientLight.color.setHex(effect.colorHex);
    this.ambientLight.intensity = effect.intensity;
    setTimeout(() => {
      this.ambientLight.color.setHex(this.ambientLightColor);
      this.ambientLight.intensity = this.ambientLightIntensity;
    }, effect.duration);
  }

  playRandomLightFlickSound() {
    const flickAudioIndex = randomNumbers.getRandomInRange(0, this.lightFlickAudios.length - 1);
    const flickAudio = this.lightFlickAudios[flickAudioIndex];
    this.playAudio(flickAudio);
  }

  lightFlick() {
    this.createLightEffect(this.lightEffects.flick);
    this.playRandomLightFlickSound();
  }

  update(delta: number) {
    super.update(delta);
    this.pointLight.position.copy(this.player.mesh.position);
    this.timeoutsManager.updateTimeOut('lightFlick', delta);
    if (this.timeoutsManager.checkIsTimeOutExpired('lightFlick')) {
      this.lightFlick();
      if (randomNumbers.getRandom() > 0.5) {
        this.timeoutsManager.initialTimeOuts.lightFlick = randomNumbers.getRandomInRange(5, 30);
      } else {
        this.timeoutsManager.initialTimeOuts.lightFlick = randomNumbers.getRandomInRange(5, 10);
      }
      this.timeoutsManager.updateExpiredTimeOut('lightFlick');
    }
  }

  onGasEnable = () => {
    if (this.gasEnabled) {
      return;
    }
    this.setNextGasAudioIndex();
    this.playAudio(this.gasAudios[this.gasAudioIndex]);
    this.disableEnableGas(true);
  }

  onGasDisable = () => {
    if (!this.gasEnabled) {
      return;
    }
    this.gasAudios[this.gasAudioIndex].stop();
    this.disableEnableGas(false);
  }

  disableEnableGas(isEnable: boolean) {
    this.gasEnabled = isEnable;
    this.gasParticlesPool.entities.forEach(
      gasParticle => (gasParticle as Gas).disableEnableGas(isEnable)
    );
  }

  onQuestion = (action: PlayerAction) => {
    this.tvMain?.printText(action.payload);
  }

  startTvStatsAnimation(actionSymbol: string) {
    if (this.tvStatsAnimationInProgress) {
      return;
    }

    this.tvStatsAnimationInProgress = true;
    const variant1 = `${actionSymbol} ${actionSymbol}\n`;
    const variant2 = ` ${actionSymbol} \n`;
    let line1 = '';
    let line2 = '';
    for (let i = 5; i--;) {
      line1 += (i % 2 === 0) ? variant1 : variant2;
      line2 += (i % 2 === 0) ? variant2 : variant1;
    }
    this.tvStats?.printText(line1);
    setTimeout(() => {
      this.tvStats?.printText(line2);
    }, 300);
    setTimeout(() => {
      this.updateStatsTv();
      this.tvStatsAnimationInProgress = false;
    }, 600);
  }

  onLike = () => {
    this.playAudio(this.likeAudio);
    this.stats.increaseCount('like');
    this.startTvStatsAnimation('👍');
  }

  onDislike = () => {
    this.playAudio(this.dislikeAudio);
    this.stats.increaseCount('dislike');
    this.startTvStatsAnimation('👎');
  }

  onChatMessage = (action: PlayerAction) => {
    this.tvChat?.printText(action.payload);
  };

  updateStatsTv() {
    this.tvStats?.printText(
      this.stats.toString()
    );
  }

  createGasParticle = () => {
    const gasPosition = this.gasCenter.clone();
    gasPosition.add(new Vector3(
      randomNumbers.getRandomFloatInRange(-2, 1),
      randomNumbers.getRandomFloatInRange(-5, 0),
      randomNumbers.getRandomFloatInRange(-1, 0.5),
    ));
    const gas = new Gas({
      position: gasPosition,
      player: this.player,
      maxY: 5,
    });
    return this.entitiesContainer.add(gas);
  };

  finish() {
    this.removeActionListeners();
    this.onFinish();
  }

  addActionListeners() {
    Object.keys(this.actions).forEach(actionName =>
      playerActions.addActionListener(
        actionName as PlayerActionName,
        this.actions[actionName as PlayerActionName]
      )
    );
  }

  removeActionListeners() {
    Object.keys(this.actions).forEach(actionName =>
      playerActions.removeActionListener(
        actionName as PlayerActionName,
        this.actions[actionName as PlayerActionName]
      )
    );
  }
}
