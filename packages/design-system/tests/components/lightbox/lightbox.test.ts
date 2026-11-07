import {initLightbox} from "../../../components/lightbox/lightbox";

const showModal = jest.fn();
const close = jest.fn();

function element(selector: string): HTMLElement {
	return document.querySelector<HTMLElement>(selector) as HTMLElement;
}

beforeEach(() => {
	showModal.mockReset();
	close.mockReset();
	document.body.innerHTML = `
		<button data-lightbox="box"></button>
		<dialog id="box">
			<div data-pack-still><img alt=""></div>
			<button data-close></button>
			<p></p>
		</dialog>`;
	// jsdom has the element but not these two.
	Object.assign(element("#box"), {showModal, close});
	initLightbox();
});

test("the still opens the dialog it names", () => {
	element("[data-lightbox]").click();

	expect(showModal).toHaveBeenCalledTimes(1);
});

test("a click on the backdrop closes it", () => {
	element("#box").click();

	expect(close).toHaveBeenCalledTimes(1);
});

test("a click on the picture closes it", () => {
	element("[data-pack-still] img").click();

	expect(close).toHaveBeenCalledTimes(1);
});

test("the close button closes it", () => {
	element("[data-close]").click();

	expect(close).toHaveBeenCalledTimes(1);
});

test("a click on anything else leaves it open", () => {
	element("#box p").click();

	expect(close).not.toHaveBeenCalled();
});

test("a still that names no dialog does nothing", () => {
	document.body.innerHTML = '<button data-lightbox="missing"></button>';

	expect(() => initLightbox()).not.toThrow();
});
