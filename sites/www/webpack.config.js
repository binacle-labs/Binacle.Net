const path = require('path');
const dest = 'js';

// No splitChunks and no vendors group, unlike the other two sites. One workspace dependency, the design system,
// is not worth a vendors chunk.
module.exports = (env, argv) => {
	const buildType = env.build || 'dist'; // 'dist' or 'watch'
	const production = buildType === 'dist';
	console.log(`Environment Build: ${buildType}`);


	// A second config, as on the other two sites: the head script ships alone and without a source map.
	const themeInit = {
		mode: production ? 'production' : 'development',
		entry: {'theme-init': './_js/theme-init.ts'},
		output: {filename: '[name].js', path: path.resolve(__dirname, dest), clean: false},
		resolve: {extensions: ['.ts', '.js', '.json']},
		module: {rules: [{test: /\.ts$/, exclude: /node_modules/, use: 'ts-loader'}]},
		optimization: {minimize: production, splitChunks: false, runtimeChunk: false},
		devtool: false,
	};

	const main = {
		mode: production ? 'production' : 'development',
		entry: {
			main: './_js/main.ts'
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
		optimization: {
			minimize: production,
		},
		cache: {
			type: 'filesystem',
		},
		devtool: production ? false : 'source-map',
	};

	return [main, themeInit];
}
