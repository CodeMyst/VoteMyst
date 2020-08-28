const lightbox = document.querySelector("#lightbox");
const lightboxImage = lightbox.querySelector("img");

function initializeLightbox() {
    document.querySelectorAll(".entry .entry-image > img").forEach(source => {
        source.addEventListener("click", e => openLightbox(source));
        source.style = "cursor: pointer";
    });
    
    document.addEventListener("keydown", e => {
        if (e.key === "Escape") {
            closeLightbox();
        }
    })
    lightbox.querySelector(".close").addEventListener("click", e => closeLightbox());
}
function openLightbox(image) {
    lightbox.setAttribute("open", "");
    lightboxImage.src = image.src;
}
function closeLightbox() {
    lightbox.removeAttribute("open");
}

if (window.innerWidth <= 640) {
    console.log("The lightbox is disabled on small screens.");
}
else {
    initializeLightbox();
}