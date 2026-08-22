const path = require('path');
const dest = 'js';

// No splitChunks and no vendors group, unlike the other two sites: this site has no npm dependencies at all,
// so there is nothing to split out. The whole bundle is the two behaviours in _js/main.ts.
module.exports = (env, argv) => {
	const buildType = env.build || 'dist'; // 'dist' or 'watch'
	const production = buildType === 'dist';
	console.log(`Environment Build: ${buildType}`);

	return {
		mode: production ? 'production' : 'development',
		entry: {
			main: './_js/main.ts'
		},
		output: {
			filename: '[name].js',
			path: path.resolve(__dirname, dest),
			// Watch mode shares this directory with a running jekyll: deleting a file it has already
			// listed makes its next File.stat raise ENOENT and kills the serve.
			clean: production,
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
}
