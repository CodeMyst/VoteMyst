/*
    vm-api.js - Copyright © YilianSource 2020

    Provides functionality to interact with the VoteMyst API from client-side.
    This script also updates visuals accordingly.
*/

// ~~ Constants ~~
const votedAttribute = "voted";
const entryIdAttribute = "entry-id";
const eventIdAttribute = "event-id";


// ~~ Common Methods ~~

// Constructs the body for common API requests.
function buildApiBody(method) {
    return {
        method: method.toUpperCase(),
        headers: {
            'Content-Type': 'application/json'
        }
    }
}
// Constructs the request body for API POST requests.
function buildApiPostBody() {
    return buildApiBody("POST");
}
// Returns the fetch response as a JSON object.
async function jsonOrReject(response) {
    if (response.ok) {
        return response.json();
    }
    else {
        var content = await response.text();
        return Promise.reject(content);
    }
}


// ~~ API Calls ~~

// -- Entries --

function getEntryId(post) {
    return post.getAttribute("id");
}
function castVote(element) {
    const post = element.closest('.post');
    if (!element.hasAttribute(votedAttribute)) {
        fetch(`/api/entry/${getEntryId(post)}/votes/cast`, buildApiPostBody())
            .then(jsonOrReject)
            .then(result => {
                if (result.hasVote) {
                    element.setAttribute(votedAttribute, '');
                }
                if (!result.actionSuccess) {
                    console.log("Error: The post already had a vote.");
                }
            });
    }
}
function removeVote(element) {
    const post = element.closest('.post');
    if (element.hasAttribute(votedAttribute)) {
        fetch(`/api/entry/${getEntryId(post)}/votes/remove`, buildApiPostBody())
            .then(jsonOrReject)
            .then(result => {
                if (!result.hasVote) {
                    element.removeAttribute(votedAttribute);
                }
                if (!result.actionSuccess) {
                    console.warn("The post did not have a vote.");
                }
            });
    }
}
function toggleVote(element) {
    if (element.hasAttribute(votedAttribute)) {
        removeVote(element);
    }
    else {
        castVote(element);
    }
    document.activeElement = null;
}

function showEntryMenu(element, canReport, canDelete) {
    const post = element.closest('.post');
    let rect = element.getBoundingClientRect();

    let items = [{
        content: "Copy Link",
        icon: "fa-link",
        action: async function() {
            let url = location.protocol + '//' + location.host+location.pathname + "#" + post.id;
            await navigator.clipboard.writeText(url);
        } 
    }, {
        content: "Report Post",
        icon: "fa-flag",
        style: "warning",
        enabled: canReport,
        action: () => reportPost(post)
    }];

    if (canDelete) {
        items.push({
            icon: "fa-trash",
            content: "Delete Post",
            style: "warning",
            action: () => deletePostConfirm(post)
        })
    }

    showContextMenu({
        positionX: rect.x + rect.width,
        positionY: rect.y,
        items: items
    });
}

function deletePostConfirm(post) {
    promptModal({
        title: "Delete post",
        content: "Are you sure you want to delete this post?<br><b>This action cannot be undone.</b>",
        width: 450,
        buttons: [{
            content: "Yes",
            style: "ok",
            action: () => deletePost(post),
        }, {
            content: "No",
            style: "cancel"
        }]
    })
}
function deletePost(post) {
    fetch(`/api/entry/${post.id}/delete`, buildApiPostBody())
        .then(result => {
            if (result.ok) {
                post.remove();
            }
        })
        .catch();
}

// -- Events --


// -- Users --


// -- Reports --

function reportPost(post) {
    promptModal({
        title: "Report post",
        content:
              "<textarea id=\"report-reason\" style=\"font: inherit;\" maxlength=\"512\" placeholder=\"Please enter a brief description of why you want to report the post.\"></textarea>"
            + "<br>Note that unreasonable usage of the report system will be punished.",
        width: 450,
        buttons: [{
            content: "Submit",
            style: "ok",
            action: function() {
                let reason = document.querySelector("#report-reason").value;

                let requestBody = buildApiPostBody();
                requestBody.body = JSON.stringify({
                    entryDisplayId: post.getAttribute("id"),
                    reason: reason
                });

                fetch(`/api/reports/submit`, requestBody)
                    .then(result => {
                        if (result.ok) {
                            promptModal({
                                title: "Report submitted",
                                content: "Thank you for submitting the report. A staff member has been notified and will look into the matter shortly.",
                                width: 450,
                                closeable: false,
                                buttons: [{
                                    content: "Got it!",
                                    style: "ok",
                                    action: () => window.location.reload()
                                }]
                            })
                        }
                    })
                    .catch();
            }
        }, {
            content: "Cancel",
            style: "cancel"
        }]
    });
}

function deleteReportedPost() {
    executeActionForReport(event.target.closest(".report"), "approve");
}
function rejectReport() {
    executeActionForReport(event.target.closest(".report"), "reject");
}

function executeActionForReport(report, action) {
    let reportId = parseInt(report.getAttribute("report-id"));
    let requestBody = buildApiPostBody();
    requestBody.body = reportId;

    fetch(`/api/reports/${reportId}/` + action, requestBody)
        .then(result => {
            if (result.ok) {
                report.remove();
            }
        })
        .catch();
}