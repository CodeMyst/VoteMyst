function reportPost(element) {
    const post = element.closest('.post');
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

                fetch(`/reports/submit`, requestBody)
                    .then(result => {
                        if (result.ok) {
                            post.querySelector(".report").remove();
                            promptModal({
                                title: "Report submitted",
                                content: "Thank you for submitting the report. A staff member has been notified and will look into the matter shortly.",
                                width: 450,
                                buttons: [{
                                    content: "Got it!",
                                    style: "ok"
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
    executeActionForReport(event.target.closest(".report"), "delete");
}
function rejectReport() {
    executeActionForReport(event.target.closest(".report"), "reject");
}

function executeActionForReport(report, action) {
    let reportId = parseInt(report.getAttribute("report-id"));
    let requestBody = buildApiPostBody();
    requestBody.body = reportId;

    fetch('/reports/action/' + action, requestBody)
        .then(result => {
            if (result.ok) {
                report.remove();
            }
        })
        .catch();
}