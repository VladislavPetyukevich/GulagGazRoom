import {
  ReinhardToneMapping,
  WebGLRenderer,
  BasicShadowMap,
} from 'three';
import { BasicScene } from './core/Scene';
import { TestScene } from './scenes/testScene';
import { LoadingScene } from './scenes/loadingScene';
import { ImageDisplayer, imageDisplayer } from './ImageDisplayer';
import { ShaderPass } from './Postprocessing/ShaderPass';
import { RenderPass } from './Postprocessing/RenderPass';
import { EffectComposer } from './Postprocessing/EffectComposer';
import { ColorCorrectionShader } from './Postprocessing/Shaders/ColorCorrectionShader';
import { ColorPaletteShader } from './Postprocessing/Shaders/ColorPalette';
import { texturesStore, audioStore } from '@/core/loaders';
import { ImageScaler } from '@/ImageScaler';
import { gameTextures, gameSounds } from './constants';
import { playerActions, PlayerActionName } from '@/PlayerActions';
import { globalSettings } from '@/GlobalSettings';

const SceneClass = TestScene;

export default class ThreeShooter {
  gameProps: any;
  currScene: BasicScene;
  loadedScene?: BasicScene;
  imageDisplayer: ImageDisplayer;
  prevTime: number;
  enabled: boolean;
  loaded: boolean;
  pixelRatio: number;
  renderer: WebGLRenderer;
  composer: EffectComposer;
  effectColorCorrection?: ShaderPass;
  effectColorPalette?: ShaderPass;

  constructor(props: any) {
    this.gameProps = props;
    this.currScene = new LoadingScene(props);
    this.imageDisplayer = imageDisplayer;
    this.prevTime = performance.now();
    this.enabled = false;
    this.loaded = false;
    this.loadTextures(props);

    this.pixelRatio = 1;
    this.renderer = new WebGLRenderer({
      powerPreference: 'high-performance',
    });
    this.renderer.setPixelRatio(this.pixelRatio);
    this.renderer.autoClear = false;
    this.renderer.shadowMap.enabled = true;
    this.renderer.shadowMap.type = BasicShadowMap;
    this.renderer.physicallyCorrectLights = true;
    this.renderer.gammaInput = true;
    this.renderer.gammaOutput = true;
    this.renderer.toneMapping = ReinhardToneMapping;
    this.renderer.toneMappingExposure = Math.pow(0.68, 5.0);

    this.composer = new EffectComposer(this.renderer);
    this.composer.addPass(new RenderPass(this.currScene.scene, this.currScene.camera));

    props.renderContainer.appendChild(this.renderer.domElement);
    this.update();
    document.addEventListener('pointerlockchange', () => {
      this.enabled = document.pointerLockElement === props.renderContainer;
      this.prevTime = performance.now();
    });
    this.handleResize(props.renderWidth, props.renderHeight);
  }

  handleResize = (width: number, height: number) => {
    this.currScene.camera.aspect = width / height;
    this.currScene.camera.updateProjectionMatrix();
    this.renderer.setSize(width, height);
    this.composer.setSize(width, height);
  };

  onPlayerActionStart(actionName: PlayerActionName, payload?: string) {
    playerActions.startAction(actionName, payload);
  }

  loadTextures(gameProps: any) {
    const imageScaler = new ImageScaler(8);
    const onLoad = () => {
      const soundsProgress = (<LoadingScene>this.currScene).soundsProgress;
      const texturesProgress = (<LoadingScene>this.currScene).texturesProgress;
      if ((soundsProgress !== 100) || (texturesProgress !== 100)) {
        return;
      }

      const pointerlockHandler = () => {
        const isRenderContainer = document.pointerLockElement === gameProps.renderContainer;
        if (!isRenderContainer) {
          return;
        }
        this.loaded = true;
        if (this.loadedScene) {
          this.changeScene(this.loadedScene);
        }
        document.removeEventListener('pointerlockchange', pointerlockHandler);
      };
      this.loadScene(SceneClass, gameProps);
      document.addEventListener('pointerlockchange', pointerlockHandler);
      gameProps.onLoad();
    };

    const onTexturesProgress = (progress: number) => {
      (<LoadingScene>this.currScene).onTexturesProgress(progress);
    };

    const onSoundsProgress = (progress: number) => {
      (<LoadingScene>this.currScene).onSoundsProgress(progress);
    };

    const onImagesScale = (imagesInfo: { [name: string]: string }) => {
      texturesStore.loadTextures(imagesInfo, onLoad, onTexturesProgress);
    };

    const onImagesScaleProgress = (progress: number) => {
      (<LoadingScene>this.currScene).onImagesScaleProgress(progress);
    };
    imageScaler.addToIgnore('damageEffect');
    imageScaler.scaleImages(gameTextures, onImagesScale, onImagesScaleProgress);
    audioStore.loadSounds(gameSounds, onLoad, onSoundsProgress);
  }

  onSceneFinish = () => {
    setTimeout(
      () => {
        this.loadScene(
          SceneClass,
          { ...this.gameProps, onFinish: this.onSceneFinish },
          () => this.loadedScene && this.changeScene(this.loadedScene)
        );
      },
      0
    );
  }

  loadScene(constructor: typeof BasicScene, gameProps: any, onLoaded?: Function) {
    this.loadedScene = new constructor({
      ...gameProps, onFinish: this.onSceneFinish
    });
    if (onLoaded) {
      onLoaded();
    }
  }

  changeScene(scene: BasicScene) {
    this.currScene.entitiesContainer.onDestroy();
    this.currScene = scene;
    this.composer = new EffectComposer(this.renderer);
    this.composer.addPass(new RenderPass(this.currScene.scene, this.currScene.camera));

    this.effectColorCorrection = new ShaderPass(ColorCorrectionShader);
    this.composer.addPass(this.effectColorCorrection);
    this.effectColorPalette = new ShaderPass(ColorPaletteShader);
    this.composer.addPass(this.effectColorPalette);
  }

  updateAudioVolume = (value: number) => {
    globalSettings.setSetting('audioVolume', value);
  };

  updateFov = (value: number) => {
    globalSettings.setSetting('fov', value);
  };

  update = () => {
    if (this.enabled || !this.loaded) {
      const time = performance.now();
      const delta = (time - this.prevTime) / 1000;
      if (delta < 1) {
        this.renderer.clear();
        this.currScene.update(delta);
        this.composer.render(delta);
        this.renderer.clearDepth();
        this.renderer.render(this.imageDisplayer.scene, this.imageDisplayer.camera);
      } else {
        console.warn('Performance issues. Skip frame');
      }
      this.prevTime = time;
    }
    requestAnimationFrame(this.update);
  }
}
