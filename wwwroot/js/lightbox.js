const lightbox = document.querySelector("#lightbox");
const lightboxImage = lightbox.querySelector("img");

document.querySelectorAll(".post .post-image > img").forEach(source => {
    source.addEventListener("click", e => openLightbox(source));
    source.style = "cursor: pointer";
});

document.addEventListener("keydown", e => {
    if (e.key === "Escape") {
        closeLightbox();
    }
})
lightbox.querySelector(".close").addEventListener("click", e => closeLightbox());

function openLightbox(image) {
    lightbox.setAttribute("open", "");
    lightboxImage.src = image.src;
}
function closeLightbox() {
    lightbox.removeAttribute("open");
}