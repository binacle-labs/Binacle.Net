import { ThemeSwitcherButtonElement } from "theme-switcher";

document.addEventListener('DOMContentLoaded', function () {
    
    const versionSelects = document.querySelectorAll('[data-versionselect]');
    if(!!versionSelects){
        versionSelects.forEach(versionSelect => {
            versionSelect.addEventListener('change', function (event) {
                const url = versionSelect.dataset.versionselect;
                const selectedVersion = event.target.value;
                if (selectedVersion) {
                    const target = new URL(url + selectedVersion, window.location.origin);
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

    customElements.define('theme-switcher', ThemeSwitcherButtonElement);

});
