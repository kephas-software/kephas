import * as webpack from 'webpack'
import * as path from 'path';

const config: webpack.Configuration = {
    entry: {
        'index': './src/index.ts',
        'index.min': './src/index.ts',
    },
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: '[name].js',
        libraryTarget: 'umd',
        library: '@kephas/core',
        umdNamedDefine: true
    },
    devtool: 'inline-source-map',
    resolve: {
        extensions: ['.ts', '.js']
    },
    optimization: {
        minimize: false,
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader', // 'awesome-typescript-loader',
                exclude: /node_modules/                
            }
        ]
    }
};

export default config;