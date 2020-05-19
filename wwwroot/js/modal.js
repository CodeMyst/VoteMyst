/*
    modal.js - Copyright © YilianSource 2020

    Provides functionality to display modal dialog windows.

    Options:
    - title (string): The title of the modal window.
    - content (string): The text inside the window. HTML content possible.
    - closeable (bool) [true]: Should the window be closeable via an 'x' in the top-right?
    - width (number): The desired width in pixels. Note that the window will never exceed 90% of the viewport width.
    - buttons (array): An array of buttons that should be displayed. These have
                       the following properties:
        - content (string): The text inside the button.
        - style (string) [default]: The name of the style to use (default/ok/cancel).
        - action (function): The action to execute when the button is pressed.
        - close (bool) [true]: Should the window be closed after the button press?
*/

function promptModal(config) {
    let modalElement = document.createElement("div");
    modalElement.classList.add("modal");

    if (config.title) {
        let titleElement = document.createElement("h1");
        titleElement.classList.add("title");
        titleElement.innerText = config.title;
        modalElement.appendChild(titleElement);
    }
    if (config.content) {
        let contentElement = document.createElement("p");
        contentElement.classList.add("content");
        contentElement.innerHTML = config.content;
        modalElement.appendChild(contentElement);
    }
    if (config.closeable == undefined || config.closeable) {
        let closeElement = document.createElement("div");
        closeElement.classList.add("close");
        closeElement.innerHTML = "&times;";
        closeElement.onclick = e => closeModal(modalElement);
        modalElement.appendChild(closeElement);
    }
    if (config.width) {
        modalElement.style.width = config.width + "px";
    }
    if (config.buttons) {
        let buttonContainer = document.createElement("div");
        buttonContainer.classList.add("buttons");

        config.buttons.forEach(button => {
            let buttonElement = document.createElement("div");
            buttonElement.classList.add("modal-button");

            if (button.content) {
                buttonElement.innerText = button.content;
            }
            if (button.style) {
                buttonElement.classList.add(button.style);
            }
            else {
                buttonElement.classList.add("default");
            }
            if (button.action) {
                buttonElement.addEventListener("click", button.action);
            }
            if (button.close == undefined || button.close) {
                buttonElement.addEventListener("click", e => closeModal(modalElement));
            }

            buttonContainer.appendChild(buttonElement);
        });

        modalElement.appendChild(buttonContainer);
    }

    let modalContainer = document.createElement("div");
    modalContainer.classList.add("modal-container");
    modalContainer.appendChild(modalElement);
    document.body.appendChild(modalContainer);
}
function closeModal(modal) {
    modal.parentNode.remove();
}
function closeAllModals() {
    document.querySelectorAll(".modal").forEach(modal => closeModal(modal));
}