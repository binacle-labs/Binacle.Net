const path = require('path');

// One entry, one file. Nothing here is served to anyone, so no chunking and no budget.
module.exports = (env) => {
	const production = (env.build || 'dist') === 'dist';

	return {
		mode: production ? 'production' : 'development',
		entry: {main: './_js/main.ts'},
		output: {
			filename: '[name].js',
			path: path.resolve(__dirname, 'js'),
			// Watch mode shares this directory with a running jekyll: deleting a file it has already
			// listed makes its next File.stat raise ENOENT and kills the serve.
			clean: production,
		},
		resolve: {extensions: ['.ts', '.js']},
		module: {rules: [{test: /\.ts$/, use: 'ts-loader', exclude: /node_modules/}]},
		devtool: production ? false : 'source-map',
	};
};
