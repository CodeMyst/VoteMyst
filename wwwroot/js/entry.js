/*
    entry.js - Copyright © YilianSource 2020
    
    Provides functionality for users to interact with entries,
      and consequentially with the VoteMyst backend.
    
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

function getEntryId(post) {
    return post.getAttribute("entry-id");
}

function castVote(element) {
    const post = element.closest('.post');
    if (!element.hasAttribute(votedAttribute)) {
        fetch(`/vote/cast/${getEntryId(post)}`, buildApiPostBody())
            .then(toJson)
            .then(result => {
                if (result.hasVote) {
                    element.setAttribute(votedAttribute, '');
                }
                if (!result.actionSuccess) {
                    console.log("Error: The post already had a vote.");
                }
            })
            .catch();
    }
}
function removeVote(element) {
    const post = element.closest('.post');
    if (element.hasAttribute(votedAttribute)) {
        fetch(`/vote/remove/${getEntryId(post)}`, buildApiPostBody())
            .then(toJson)
            .then(result => {
                if (!result.hasVote) {
                    element.removeAttribute(votedAttribute);
                }
                if (!result.actionSuccess) {
                    console.log("Error: The post did not have a vote.");
                }
            })
            .catch();
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