// three ships its lines files as ESM only and the commonjs transform cannot load them. They reach the module
// graph because the plugin barrels import the pack view; nothing that imports them here ever constructs one.
export class LineMaterial {
}

export class LineSegments2 {
}

export class LineSegmentsGeometry {
}
