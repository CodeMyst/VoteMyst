const posts = document.querySelectorAll('.entries');
const gridOptions = document.querySelectorAll(".entry-display-options > *");

function setGridAspect(aspect) {
    gridOptions.forEach(option => {
        option.classList = option.id.endsWith(aspect)
            ? "active" : "";
    });
    posts.forEach(post => post.classList = "entries entry-aspect-" + aspect);
    window.sessionStorage.setItem("aspect", aspect);
}
gridOptions.forEach(option => {
    option.addEventListener("click", e => {
        setGridAspect(option.id.substring("entry-display-option-".length));
    });
});
let aspect = window.sessionStorage.getItem("aspect") || "middle";
setGridAspect(aspect);