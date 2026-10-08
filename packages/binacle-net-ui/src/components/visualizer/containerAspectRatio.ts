// A container that has not been laid out yet measures 0, and 0 or Infinity makes the camera projection NaN.
export function containerAspectRatio(container: HTMLElement) {
	if (container.offsetWidth < 1 || container.offsetHeight < 1) {
		return 1;
	}
	return container.offsetWidth / container.offsetHeight;
}
