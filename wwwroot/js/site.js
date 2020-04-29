// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
document.addEventListener('DOMContentLoaded', function() {
    const posts = document.querySelector('.posts');
    const gridOptions = document.querySelectorAll(".post-display-options > *");
    
    function setGridAspect(aspect) {
        gridOptions.forEach(option => {
            option.classList = option.id.endsWith(aspect)
                ? "active" : "";
        });
        posts.classList = "posts posts-aspect-" + aspect;
        window.sessionStorage.setItem("aspect", aspect);
    }

    gridOptions.forEach(option => {
        option.addEventListener("click", e => {
            setGridAspect(option.id.substring("post-display-option-".length));
        });
    });

    let aspect = window.sessionStorage.getItem("aspect") || "middle";
    setGridAspect(aspect);
}, false);