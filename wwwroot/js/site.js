// Setup the sidebar for mobile devices
const body = document.querySelector("body");
const sidebar = document.querySelector("#sidebar");

if (sidebar && body) {
    sidebar.querySelector(".toggle").addEventListener("click", e => {
        body.classList.toggle('sidebar-open');
        sidebar.toggleAttribute("expanded");
    });
    container.addEventListener("click", e => {
        body.classList.remove('sidebar-open');
        sidebar.removeAttribute("expanded");
    });
}



// Shows the cookie disclaimer if it hasn't been read yet.
const cookieStorageKey = 'readCookies';
const cookieDisclaimer = document.querySelector(".cookie-disclaimer");

if (cookieDisclaimer) {
    if (localStorage.getItem(cookieStorageKey)) {
        cookieDisclaimer.remove();
    }
    else {
        cookieDisclaimer.classList.remove("hidden");
    }
    
    function confirmCookies() {
        // Confirms that the user read the cookie disclaimer.
        localStorage.setItem(cookieStorageKey, true);
        cookieDisclaimer.remove();
    }
}



// Update the footer to be sticky at the bottom if the page is large enough
const footer = document.querySelector("footer");
const fixedClass = "fixed";

if (footer) {
    function updateFooter() {
        let documentHeight = document.documentElement.offsetHeight;
        let windowHeight = window.innerHeight;
        
        let isFixed = footer.classList.contains(fixedClass);
        
        if (isFixed) {
            windowHeight -= 40 + 3 + 24 * 2;
        }
        
        let needsFixed = documentHeight < windowHeight;
        
        if (!isFixed && needsFixed) {
            footer.classList.add(fixedClass);
        }
        if (isFixed && !needsFixed) {
            footer.classList.remove(fixedClass);
        }
    }
    window.addEventListener("resize", updateFooter);
    let documentObserver = new MutationObserver(updateFooter);
    documentObserver.observe(document.body, { childList: true, subtree: true });
    updateFooter();
}