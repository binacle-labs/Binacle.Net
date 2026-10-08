import { ThemeSwitcherButtonElement, optionsFromDocument } from "theme-switcher";

document.addEventListener('DOMContentLoaded', function () {
    
    const versionSelects = document.querySelectorAll('[data-versionselect]');
    if(!!versionSelects){
        versionSelects.forEach(versionSelect => {
            versionSelect.addEventListener('change', function (event) {
                // The option value is the url of this page in the chosen version, written by the build.
                const selected = event.target.value;
                if (selected) {
                    const target = new URL(selected, window.location.origin);
                    if (target.origin === window.location.origin) {
                        window.location.href = target.href;
                    }
                }
            });
        });
    }
    
    const activeSpans = document.querySelectorAll('span[data-active]');
    if (!!activeSpans){
        activeSpans.forEach(activeSpan => {
            let parent = activeSpan.closest('details');
            let count = 0;
            while (parent && count < 10) { // Limit to 10 levels to prevent infinite loop
                parent.open = true;
                parent = parent.parentElement.closest('details');
                count++;
            }
        });    
    }


	const closeButtons = document.querySelectorAll('button.close-btn');
	if(!!closeButtons) {
		closeButtons.forEach(button => {
			button.addEventListener('click', function () {
				const dialog = button.closest('dialog');
				if(!!dialog){
					dialog.close();
					dialog.classList.remove('active');
					const overlay = dialog.previousElementSibling;
					if(!!overlay){
						overlay.classList.remove('active');
					}
				}

			});
		});

	}

    ThemeSwitcherButtonElement.configure(optionsFromDocument());
    customElements.define('theme-switcher', ThemeSwitcherButtonElement);

});
