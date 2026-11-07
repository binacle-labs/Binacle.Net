const { createDefaultPreset } = require("ts-jest");

const tsJestTransformCfg = createDefaultPreset().transform;

/** @type {import("jest").Config} **/
module.exports = {
  // No binacle: the package is generic. `--selectProjects <name>` runs it on its own, and the test runner
  // names its report files after it.
  displayName: "cookies",
  testEnvironment: "jsdom",
  // https, not jsdom's default http: Cookies.set defaults to secure, and jsdom hides a secure cookie
  // from a document on an insecure origin, so every round trip would read back nothing.
  testEnvironmentOptions: {
    url: "https://localhost/",
  },
  transform: {
    ...tsJestTransformCfg,
  },
  testMatch: ["**/tests/**/*.test.ts"],
};
