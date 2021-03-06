﻿
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
})

// Constructs the body for common POST requests
function buildApiPostBody() {
    return {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        }
    }
}