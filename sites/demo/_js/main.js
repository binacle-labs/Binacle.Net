import { ThemeSwitcherButtonElement, optionsFromDocument } from "theme-switcher";

document.addEventListener('DOMContentLoaded', function () {
    ThemeSwitcherButtonElement.configure(optionsFromDocument());
    customElements.define('theme-switcher', ThemeSwitcherButtonElement);
});

