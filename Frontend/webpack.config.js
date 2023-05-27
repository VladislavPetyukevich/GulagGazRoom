const path = require('path');
const webpack = require('webpack');
const TerserPlugin = require("terser-webpack-plugin");
const CopyPlugin = require("copy-webpack-plugin");
const HtmlWebpackPlugin = require('html-webpack-plugin');

const PATHS = {
  root: path.join(__dirname),
  source: path.join(__dirname, 'source'),
  public: path.join(__dirname, 'public'),
  build: path.join(__dirname, 'build'),
};

const createCommonConfig = (envVariables = {}) => {
  return {
    entry: {
      main: PATHS.source + '/index.ts'
    },
    output: {
      path: PATHS.build,
      filename: '[name].[contenthash].js',
      clean: true,
      library: {
        type: 'umd',
        name: 'ThreeShooter',
      },
    },
    resolve: {
      alias: {
        '@': path.resolve(__dirname, 'source/view'),
      },
      extensions: ['.js', '.ts'],
    },
    devtool: 'source-map',
    module: {
      rules: [
        { enforce: "pre", test: /\.js$/, use: ["source-map-loader"] },
        {
          test: /\.(js|ts)$/,
          exclude: /node_modules/,
          use: 'ts-loader'
        },
        {
          test: /\.(gif|png|jpe?g|svg|mp3)$/i,
          use: [
            'file-loader',
            {
              loader: 'image-webpack-loader',
              options: {
                bypassOnDebug: true,
                disable: true,
              },
            },
          ],
        },
        {
          test: /\.(json|dae|fbx)$/,
          use: [
            {
              loader: 'url-loader',
              options: {
                limit: 8192
              }
            }
          ]
        },
        {
          test: /\.(glsl)$/i,
          use: 'raw-loader'
        }
      ]
    },
    plugins: [
      new HtmlWebpackPlugin({
        hash: true,
        inject: false,
        template: PATHS.root + '/index.html',
        filename: './index.html',
      }),
      new webpack.DefinePlugin(envVariables),
    ],
  };
};

const developmentConfig = {
  mode: 'development',
  devServer: {
    static: {
      directory: PATHS.public,
    },
    port: 8080,
    hot: true,
    proxy: {
      '/api': {
        target: 'http://localhost:5043',
        pathRewrite: { '^/api': '' },
      },
    },
  },
  optimization: {
    moduleIds: 'named',
  },
  plugins: [
    new webpack.NoEmitOnErrorsPlugin(),
  ],
};

const productionConfig = {
  mode: 'production',
  optimization: {
    minimize: true,
    minimizer: [new TerserPlugin({
      terserOptions: {
        ecma: 5,
      },
    })],
  },
  plugins: [
    new CopyPlugin({
      patterns: [
        { from: PATHS.public, to: PATHS.build },
      ],
    }),
  ],
}

const createEnvVariables = (obj = {}) => ({
  'process.env.REACT_APP_BACKEND_URL': JSON.stringify(obj['REACT_APP_BACKEND_URL']),
  'process.env.REACT_APP_WS_URL': JSON.stringify(obj['REACT_APP_WS_URL']),
});

const createConfig = (envVariables = {}, extension = {}) => {
  const commonConfig = createCommonConfig(envVariables);
  return {
    ...commonConfig,
    ...extension,
    plugins: [
      ...commonConfig.plugins,
      ...extension.plugins,
    ],
  };
};

module.exports = function (env) {
  if (env.development) {
    const dotenvDev = require('dotenv').config({ path: './.env.development' }).parsed;
    return createConfig(
      createEnvVariables(dotenvDev),
      developmentConfig
    );
  }
  if (!env.development) {
    return createConfig(
      createEnvVariables(process.env),
      productionConfig
    );
  }
};
