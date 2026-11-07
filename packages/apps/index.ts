// The two declaration files carry ambient augmentations - the Alpine global, its $logger magic and
// _x_fieldPrefix - and nothing imports them, so without these references a host never adds them to its
// program and every use of the three reports as a missing property.
/// <reference path="./src/types/global.d.ts" />
/// <reference path="./src/types/alpine.d.ts" />

export {everydayItemsPlugin} from "./src/apps/everydayItems/plugin";
export {packingDemoPlugin} from "./src/apps/packingDemo/plugin";
export {protocolDecoderPlugin} from "./src/apps/protocolDecoder/plugin";
