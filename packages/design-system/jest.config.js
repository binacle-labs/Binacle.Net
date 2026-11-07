const { createDefaultPreset } = require("ts-jest");

const tsJestTransformCfg = createDefaultPreset().transform;

/** @type {import("jest").Config} **/
module.exports = {
  // The folder name, not the npm name: `--selectProjects <name>` runs this package on its own, and the
  // test runner names its report files after it, so a "/" would make a folder.
  displayName: "design-system",
  testEnvironment: "jsdom",
  transform: {
    ...tsJestTransformCfg,
  },
  testMatch: ["**/tests/**/*.test.ts"],
};
