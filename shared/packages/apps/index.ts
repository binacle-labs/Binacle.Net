// The two declaration files add the Alpine global, its $logger magic and _x_fieldPrefix. Nothing imports
// them, so without these references a host never loads them and each of those reports as a missing property.
/// <reference path="./src/types/global.d.ts" />
/// <reference path="./src/types/alpine.d.ts" />

export {everydayItemsPlugin} from "./src/everydayItems/plugin";
export {packingDemoPlugin} from "./src/packingDemo/plugin";
export {protocolDecoderPlugin} from "./src/protocolDecoder/plugin";
