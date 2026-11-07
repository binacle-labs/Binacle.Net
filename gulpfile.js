const {src, dest, task} = require('gulp');


const ASSETS = {
	images: {
		src: `assets/**/*.{svg,png,jpg,gif,ico}`,
		options: {encoding: false}
	},
	js: {
		src: `assets/**/*.js`
	},
	css: {
		src: `assets/**/*.css`
	},
	fonts: {
		src: `assets/**/*.woff2`,
		options: {encoding: false}
	},
	// The vendored libraries are Apache and MIT, and both require their notice to travel with the copy. These
	// have no extension, so none of the globs above would pick them up.
	licenses: {
		src: `assets/**/{LICENSE,NOTICE}`
	}
};
// What each target does not get. Everything else in assets/ goes everywhere, so only the weight differs.
//
// swagger-ui, 6.2 MB: only sites/docs/_layouts/swagger.html reads it. The API serves its own from Swashbuckle.
// beercss and material-dynamic-colors: docs and demo build on the design system instead.
// docs, demo and www take the favicons, the mark and the link preview from media/, and nothing else there.
// The two android icons go to the three sites, which name them in a web app manifest. The UI module has none.
const UNUSED_MEDIA = [
	'assets/media/docker/**',
	'assets/media/github/**',
	'assets/media/logo/binacle-logo-16x16.svg',
	'assets/media/logo/binacle-logo-{16x16a,24x24a,32x32a,48x48a,128x128a,256x256a}.png'
];
const IGNORE = {
	docs: ['assets/lib/beercss/**', 'assets/lib/material-dynamic-colors/**', ...UNUSED_MEDIA],
	demo: ['assets/lib/**', ...UNUSED_MEDIA],
	uimodule: ['assets/lib/swagger-ui/**', 'assets/android-chrome-*.png'],
	www: ['assets/lib/**', ...UNUSED_MEDIA]
};

// ----------------- Begin Functions  ----------------- //
// Main Functions
function copyAssets(destinationDir, ignore = []) {
	let tasks = [];
	console.log(`Assets -> ${destinationDir}`);
	Object.keys(ASSETS).forEach(key => {
		const section = {...ASSETS[key], options: {...(ASSETS[key].options || {}), ignore}};
		console.log(`Assets/${key} -> ${destinationDir}`);
		tasks.push(new Promise((resolve) => {
			src(section.src, section.options)
				.pipe(dest(destinationDir))
				.on('end', () =>{
					console.log(`Assets/${key} -> ${destinationDir}: OK`);
					resolve();
				});
		}));
	});
	return Promise.all(tasks).then(() => {
		console.log(`Assets -> ${destinationDir}: OK`);
	});
}

// A font URL resolves from the built CSS, so a site carries its own copy, with the licences the fonts require.
function copyFonts(destinationDir) {
	return new Promise((resolve) => {
		src('packages/design-system/fonts/*', {encoding: false})
			.pipe(dest(destinationDir))
			.on('end', resolve);
	});
}

// ----------------- End Functions  ----------------- //

// Tasks
task('copy-assets-to-demo', async function(){
	return Promise.all([copyAssets('sites/demo', IGNORE.demo), copyFonts('sites/demo/fonts')]);
});

task('copy-assets-to-docs', async function(){
	return Promise.all([copyAssets('sites/docs', IGNORE.docs), copyFonts('sites/docs/fonts')]);
});

task('copy-assets-to-www', async function(){
	return Promise.all([copyAssets('sites/www', IGNORE.www), copyFonts('sites/www/fonts')]);
});

task('copy-assets-to-uimodule', async function(){
	return copyAssets('api/src/Binacle.Net.UIModule/wwwroot', IGNORE.uimodule);
});
