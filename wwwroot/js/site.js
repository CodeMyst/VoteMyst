
// Setup the sidebar for mobile devices

const body = document.querySelector("body");
const sidebar = document.querySelector("#sidebar");

sidebar.querySelector(".toggle").addEventListener("click", e => {
    body.classList.toggle('sidebar-open');
    sidebar.toggleAttribute("expanded");
});
container.addEventListener("click", e => {
    body.classList.remove('sidebar-open');
    sidebar.removeAttribute("expanded");
});

// Shows the cookie disclaimer if it hasn't been read yet.
const cookieStorageKey = 'readCookies';
const cookieDisclaimer = document.querySelector(".cookie-disclaimer");
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