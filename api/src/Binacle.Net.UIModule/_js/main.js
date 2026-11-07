import { ThemeSwitcherButtonElement, optionsFromDocument } from "@binacle/theme-switcher";

document.addEventListener('DOMContentLoaded', function () {
    ThemeSwitcherButtonElement.configure(optionsFromDocument());
    customElements.define('theme-switcher', ThemeSwitcherButtonElement);
});
