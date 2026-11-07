const { createDefaultPreset } = require("ts-jest");

const tsJestTransformCfg = createDefaultPreset().transform;

/** @type {import("jest").Config} **/
module.exports = {
  // The npm name without its "/": `--selectProjects <name>` runs this package on its own, and the test
  // runner names its report files after it, so a "/" would make a folder.
  displayName: "binacle-api-client",
  testEnvironment: "node",
  transform: {
    ...tsJestTransformCfg,
  },
  testMatch: ["**/tests/**/*.test.ts"],
};
