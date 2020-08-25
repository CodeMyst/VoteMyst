/*
    modal.js - Copyright © YilianSource 2020

    Provides functionality to display modal dialog windows and context menus.

    Modal Config:
    - title (string): The title of the modal window.
    - content (string): The text inside the window. HTML content possible.
    - closeable (bool) [true]: Should the window be closeable via an 'x' in the top-right?
    - width (number): The desired width in pixels. Note that the window will never exceed 90% of the viewport width.
    - buttons (array): An array of buttons that should be displayed. These have the following properties:
        - content (string): The text inside the button.
        - style (string) [default]: The name of the style to use (default/ok/cancel).
        - action (function): The action to execute when the button is pressed.
        - close (bool) [true]: Should the window be closed after the button press?

    Context Menu Config:
    - positionX, positionY (number): If not set, the cursor position will be used as the origin.
    - items (array): An array of items that are displayed
        - icon (string): The FA class of the desired icon.
        - content (string): The text in the item.
        - consumed (string): The text to shown once the item is consumed.
        - action (function): The action to execute when the option is used.
        - enabled (bool) [true]: If false, pressing the option won't do anything.
        - style (string): A CSS string for content styling.

    Notification Config:
    - content (string): The content of the notification.
    - duration (number) [4]: The duration that the notification is visible for, before fading out.
    - style (string) [blank]: The name of the style to use (blank/success/error/warning).
*/

function promptModal(config) {
    let modalElement = document.createElement("div");
    modalElement.classList = "modal no-select";

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

function showContextMenu(config) {
    let contextMenu = document.createElement("div");
    contextMenu.classList = "context-menu no-select";
    contextMenu.tabIndex = -1;
    contextMenu.addEventListener("blur", function() {
        contextMenu.remove();
    }, true);

    // Items
    if (config.items) {
        config.items.forEach(item => {
            let contextItem = document.createElement("div");
            let itemEnabled = item.enabled == undefined || item.enabled;
            contextItem.classList.add("item");

            if (itemEnabled) {
                contextItem.addEventListener("click", function() {
                    if (item.action) {
                        item.action();
                    }

                    contextMenu.blur();
                });
            }

            if (item.style) {
                contextItem.classList.add(item.style);
            }
            if (!itemEnabled) {
                contextItem.classList.add("disabled");
            }

            let inner = "";
            if (item.icon) {
                inner += `<i class="fas ${item.icon}"></i>`;
            }
            if (item.content) {
                inner += `<span class="content">${item.content}</span>`;
            }

            contextItem.innerHTML = inner;
            contextMenu.appendChild(contextItem);
        });
    }

    document.body.appendChild(contextMenu);
    contextMenu.focus();
    
    // Positioning
    let rect = contextMenu.getBoundingClientRect();
    contextMenu.style.top = (config.positionY) + "px";
    contextMenu.style.left = config.positionX + "px";

    if (config.positionX + rect.width > window.innerWidth) {
        contextMenu.style.left = (config.positionX - rect.width) + "px";
    }
}

const notificationContainer = document.querySelector("#notifications");

function showNotification(config) {
    let notification = document.createElement("div");
    notification.classList.add("notification");

    let duration = config.duration ?? 4;
    let style = config.style ?? "blank";
    notification.classList.add(style);

    if (config.content) {
        notification.innerHTML = config.content;
    }

    notificationContainer.appendChild(notification);

    window.setTimeout(() => notification.remove(), duration * 1000);
}