/*
    vote.js - Copyright © YilianSource 2020
    
    Provides functionality for users to vote on entries,
      interacting with the VoteMyst backend.
    
    Methods are designed with user error in mind.
*/

// ~~ Attributes ~~
const votedAttribute = "voted";
const entryIdAttribute = "entry-id";

// Returns the JSON body of the fetch response
function json(response) {
    return response.json();
}

// Constructs the POST body, using the entryId of the given element
function buildPostBody(element) {
    return {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        }, 
        body: JSON.stringify({ 
            entryId: parseInt(element.getAttribute(entryIdAttribute))
        })
    }
}

// Register the voting events
document.querySelectorAll(".post")
    .forEach(post => post.addEventListener("click", e => {
        if (!post.hasAttribute(votedAttribute)) {
            fetch("/vote/cast", buildPostBody(post))
                .then(json)
                .then(json => {
                    if (json.hasVote) {
                        post.setAttribute(votedAttribute, '');
                    }
                    
                    if (!json.actionSuccess) {
                        console.log("Error: The post already had a vote.");
                    }
                })
                .catch();
        }
    }));

// Register the unvote events
document.querySelectorAll(".post-has-voted")
    .forEach(hasVoted => hasVoted.addEventListener("click", e => {
        let post = hasVoted.parentElement;
        if (post.hasAttribute(votedAttribute)) {
            fetch("/vote/remove", buildPostBody(post))
                .then(json)
                .then(json => {
                    if (!json.hasVote) {
                        post.removeAttribute(votedAttribute);
                    }
                    
                    if (!json.actionSuccess) {
                        console.log("Error: The post did not have a vote.");
                    }
                })
                .catch(e => console.log(e));
        }
    }));