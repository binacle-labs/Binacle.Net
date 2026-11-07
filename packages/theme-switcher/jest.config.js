const { createDefaultPreset } = require("ts-jest");

const tsJestTransformCfg = createDefaultPreset().transform;

/** @type {import("jest").Config} **/
module.exports = {
  // No binacle: the package is generic. `--selectProjects <name>` runs it on its own, and the test runner
  // names its report files after it.
  displayName: "theme-switcher",
  testEnvironment: "jsdom",
  // https, not jsdom's default http: cookies are secure by default, and jsdom hides a secure
  // cookie from a document on an insecure origin, so the theme would never read back.
  testEnvironmentOptions: {
    url: "https://localhost/",
  },
  transform: {
    ...tsJestTransformCfg,
  },
  testMatch: ["**/tests/**/*.test.ts"],
};
