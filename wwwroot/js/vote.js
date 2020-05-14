/*
    vote.js - Copyright © YilianSource 2020
    
    Provides functionality for users to vote on entries,
      interacting with the VoteMyst backend.
    
    Methods are designed with user error in mind.
*/

// ~~ Attributes ~~
const votedAttribute = "voted";
const entryIdAttribute = "entry-id";
const eventIdAttribute = "event-id";

// Returns the JSON body of the fetch response
function toJson(response) {
    return response.json();
}

function getEntryId(post) {
    return parseInt(post.getAttribute(entryIdAttribute));
}
function getEventId() {
    return parseInt(document.querySelector(".posts").getAttribute(eventIdAttribute));
}

// Constructs the POST body, using the entryId of the given element
function buildPostBody(post) {
    return {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        }
    }
}
function buildUrlParameters(post) {
    return getEventId() + "/" + getEntryId(post);
}

function castVote(element) {
    const post = element;
    if (!post.hasAttribute(votedAttribute)) {
        fetch(`/vote/cast/${buildUrlParameters(post)}`, buildPostBody(post))
            .then(toJson)
            .then(result => {
                if (result.hasVote) {
                    post.setAttribute(votedAttribute, '');
                }
                if (!result.actionSuccess) {
                    console.log("Error: The post already had a vote.");
                }
            })
            .catch();
    }
}
function removeVote(element) {
    const post = element.parentElement;
    if (post.hasAttribute(votedAttribute)) {
        fetch(`/vote/remove/${buildUrlParameters(post)}`, buildPostBody(post))
            .then(toJson)
            .then(result => {
                if (!result.hasVote) {
                    post.removeAttribute(votedAttribute);
                }
                if (!result.actionSuccess) {
                    console.log("Error: The post did not have a vote.");
                }
            })
            .catch();
    }
}