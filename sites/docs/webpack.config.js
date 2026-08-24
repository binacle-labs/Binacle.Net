const path = require('path');
const dest = 'js';

module.exports = (env, argv) => {
	const buildType = env.build || 'dist'; // 'dist' or 'watch'
	const production = buildType === 'dist';
	console.log(`Environment Build: ${buildType}`);


	// A second config, not a second entry: the head script has to be one self-contained file, and the main
	// config splits every entry into a runtime chunk and a vendors chunk.
	const themeInit = {
		mode: production ? 'production' : 'development',
		entry: {'theme-init': './_js/theme-init.ts'},
		output: {filename: '[name].js', path: path.resolve(__dirname, dest), clean: false},
		resolve: {extensions: ['.ts', '.js', '.json']},
		// esnext here only: tsconfig emits commonjs, and webpack cannot tree-shake or concatenate that -
		// it costs this bundle its module registry, on a script that blocks the first paint.
		module: {rules: [{test: /\.ts$/, exclude: /node_modules/, use: {
			loader: 'ts-loader',
			options: {compilerOptions: {module: 'esnext', moduleResolution: 'node'}},
		}}]},
		optimization: {minimize: production, splitChunks: false, runtimeChunk: false},
		devtool: false,
	};

	const main = {
		mode: production ? 'production' : 'development',
		entry: {
			main: './_js/main.js'
		},
		output: {
			filename: '[name].js',
			path: path.resolve(__dirname, dest),
			// Watch mode shares this directory with a running jekyll: deleting a file it has already
			// listed makes its next File.stat raise ENOENT and kills the serve.
			// The two configs run in parallel into one directory, so cleaning has to spare the other's file.
			clean: production ? {keep: /^theme-init\.js$/} : false,
		},
		resolve: {
			extensions: ['.ts', '.js', '.json']
		},
		module: {
			rules: [
				{
					test: /\.ts$/,
					use: 'ts-loader',
					exclude: /node_modules/,
				},
			],
		},
		plugins: [
		],
		optimization: {
			minimize: production ? true : false,
			splitChunks: {
				chunks: 'all',
				automaticNameDelimiter: '-',
				minSize: 20000,
				cacheGroups: {
					vendors: {
						test: /[\\/]node_modules[\\/]/,
						name: 'vendors',
						chunks: 'all',
						enforce: true,
						priority: 10,
					}
				},
			},
		},
		cache: {
			type: 'filesystem',
		},
		devtool: production ? false : 'source-map',
	};

	return [main, themeInit];
}
