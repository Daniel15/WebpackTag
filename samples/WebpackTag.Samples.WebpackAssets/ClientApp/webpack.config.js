const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const saveAssets = require('assets-webpack-plugin');

const IS_DEV = process.env.NODE_ENV !== 'production';

module.exports = {
  context: __dirname,
  entry: {
    first: './src/first.js',
    second: './src/second.js',
  },
  output: {
    path: __dirname + '/dist/',
    filename: IS_DEV ? '[name].js?[contenthash]' : '[name]-[contenthash].min.js',
  },
  module: {
    rules: [
      {
        test: /\.css$/,
        use: [
          {
            loader: MiniCssExtractPlugin.loader,
          },
          {
            loader: 'css-loader',
            options: {
              sourceMap: true,
            },
          },
        ],
      },
    ],
  },
  optimization: {
    splitChunks: {
      chunks: 'all',
    },
  },
  plugins: [
    new saveAssets({
      entrypoints: true,
      useCompilerPath: true,
    }), 
    new MiniCssExtractPlugin({
      filename: IS_DEV ? '[name].css?[contenthash]' : '[name]-[contenthash].css',
    }),
   ],
}