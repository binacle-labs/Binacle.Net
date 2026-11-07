const path = require('node:path');

module.exports = (env) => {
	const production = (env.build || 'dist') === 'dist';

	return {
		mode: production ? 'production' : 'development',
		entry: {
			main: './_js/main.js',
			packing_demo: './_js/packing_demo.js',
			protocol_decoder: './_js/protocol_decoder.js'
		},
		output: {
			filename: '[name].js',
			path: path.resolve(__dirname, 'wwwroot/js'),
			// Not in watch mode: a running Kestrel has already listed these files.
			clean: production,
		},
		resolve: {
			extensions: ['.ts', '.js', '.json'],
		},
		module: {
			rules: [
				{
					test: /\.ts$/,
					// The package's commonjs would load three twice: our require takes its commonjs build, and its
					// own add-ons import the module one.
					use: {
						loader: 'ts-loader',
						options: {compilerOptions: {module: 'esnext', moduleResolution: 'node'}},
					},
					exclude: /node_modules/,
				},
			],
		},
		optimization: {
			minimize: production,
			runtimeChunk: 'single',
			splitChunks: {
				chunks: 'all',
				automaticNameDelimiter: '-',
				minSize: 0,
				cacheGroups: {
					// three ships as 3 pre-bundled modules, 566 KiB minified, no tree shaking.
					three: {
						test: /[\\/]node_modules[\\/]three[\\/]/,
						name: 'three',
						chunks: 'all',
						enforce: true,
						priority: 30,
					},
					vendors: {
						test: /[\\/]node_modules[\\/]/,
						name: 'vendors',
						chunks: 'all',
						enforce: true,
						priority: 10,
					},
					// The API client and the 3D view ride in this chunk rather than their own. Every chunk file
					// is listed by hand in _AppletScripts.cshtml, so a new one is dead weight until a script tag
					// names it.
					binacleNetUi: {
						test: /[\\/]packages[\\/](apps|api-client|design-system[\\/]components[\\/]pack-view)[\\/]/,
						// A URL the pages and the smoke tests ask for, so it keeps the package's old name.
						name: 'binacle-net-ui',
						chunks: 'all',
						enforce: true,
						priority: 20,
					},
					binacleViPaq: {
						test: /[\\/]vipaq[\\/]packages[\\/]vipaq[\\/]/,
						name: 'binacle-vipaq',
						chunks: 'all',
						enforce: true,
						priority: 20,
					},
				},
			},
		},
		performance: {
			hints: production ? 'warning' : false,
			maxAssetSize: 300 * 1024,
			maxEntrypointSize: 300 * 1024,
			// three is unavoidable on the demo pages. Budget everything else.
			assetFilter: (asset) => asset.endsWith('.js') && !asset.startsWith('three'),
		},
		// A warm cache reports success on source that fails a cold build, type errors included. Delete
		// node_modules/.cache/webpack before trusting a clean run.
		cache: {type: 'filesystem'},
		devtool: production ? false : 'source-map',
	};
};
