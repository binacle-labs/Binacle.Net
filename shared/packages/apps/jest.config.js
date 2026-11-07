const { createDefaultPreset } = require("ts-jest");

const tsJestTransformCfg = createDefaultPreset().transform;

/** @type {import("jest").Config} **/
module.exports = {
  // The npm package name, so `--selectProjects <name>` can run this package on its own through the
  // root config, and so coverage and test reports are filed under a name that means something.
  displayName: "binacle-apps",
  // The components read document and window even where the logic under test does not.
  testEnvironment: "jsdom",
  moduleNameMapper: {
    "^three/examples/jsm/controls/OrbitControls(\\.js)?$": "<rootDir>/tests/stubs/orbitControls.ts",
    "^three/examples/jsm/lines/(LineMaterial|LineSegments2|LineSegmentsGeometry)(\\.js)?$": "<rootDir>/tests/stubs/lines.ts",
  },
  transform: {
    ...tsJestTransformCfg,
  },
  testMatch: ["**/tests/**/*.test.ts"],
};
