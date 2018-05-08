import * as webpack from 'webpack'
import * as path from 'path';
import DtsBundlePlugin from 'webpack-dts-bundle';

const config: webpack.Configuration = {
    entry: {
        'index': './src/index.ts'
    },
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: '[name].js',
        libraryTarget: 'umd',
        library: '@kephas/core',
        umdNamedDefine: true
    },
    mode: "production",
    devtool: 'source-map',
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
                exclude: [
                    /node_modules/,
                    /webpack/
                ]
            }
        ]
    },
    plugins: [
        new DtsBundlePlugin({
            name: '@kephas/core',
            main: path.resolve(__dirname, './src/index.d.ts'),
            out: path.resolve(__dirname, './dist/index.d.ts'),
            verbose: true
        })
    ]
};

export default config;