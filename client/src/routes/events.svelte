<script lang="ts">
    import { EventType, getEventsListing } from "$lib/api/event";
    import type { Event } from "$lib/api/event";
    import { session } from "$app/stores";
    import { getUserById } from "$lib/api/user";

    const eventsPromise = getEventsListing();

    const now = new Date();

    const groupEvents = (events: Event[]): Event[][] => {
        const res: Event[][] = [];

        // init 3 arrays, upcoming, in progress, finished, host
        res.push([]);
        res.push([]);
        res.push([]);
        res.push([]);

        for (const event of events) {
            const revealDate = new Date(event.revealDate);
            const votingEndDate = new Date(event.votingEndDate);
            const submissionStartDate = new Date(event.submissionStartDate);

            // host
            if ($session.user && event.hostIds.includes($session.user._id)) {
                res[3].push(event);
                // finished
            } else if (now > votingEndDate) {
                res[2].push(event);
                // in progress
            } else if (now > submissionStartDate) {
                res[1].push(event);
                // upcoming
            } else if (now > revealDate) {
                res[0].push(event);
            }
        }

        return res;
    };
</script>

<h2>Events</h2>

<p>This is a list of all hosted events.</p>

{#await eventsPromise then events}
    {#each groupEvents(events) as group, i}
        {#if group.length > 0}
            {#if i === 0}
                <h3>Upcoming</h3>
            {:else if i === 1}
                <h3>In progress</h3>
            {:else if i === 2}
                <h3>Finished</h3>
            {:else if i === 3}
                <h3>Hosted by you</h3>
            {/if}

            {#each group as event}
                <div class="event">
                    <h5 class="flex center sm-row space-between">
                        <a class="title no-dec" href="/events/{event.vanityUrl}">
                            {event.title}
                        </a>

                        <div class="flex center row">
                            {#if event.type === EventType.ART}
                                <span class="type art">Art</span>
                            {:else if event.type === EventType.CODING}
                                <span class="type coding">Coding</span>
                            {:else if event.type === EventType.STORY}
                                <span class="type story">Story</span>
                            {:else if event.type === EventType.GAMEJAM}
                                <span class="type jam">Game Jam</span>
                            {/if}

                            {#if $session.user && event.hostIds.includes($session.user._id)}
                                <span class="host">HOST</span>
                            {/if}
                        </div>
                    </h5>

                    {#await getUserById(event.hostIds[0]) then host}
                        <p class="hosts">
                            Hosted by: <a href="/users/{host?.username}">{host?.username}</a>
                        </p>
                    {/await}

                    {#if now > new Date(event.votingEndDate)}
                        <p>Event ended on: {new Date(event.votingEndDate).toDateString()}</p>
                    {:else if now > new Date(event.submissionEndDate)}
                        <p>Voting open until: {new Date(event.submissionEndDate).toDateString()}</p>
                    {:else if now > new Date(event.submissionStartDate)}
                        <p>
                            Submissions open until:
                            {new Date(event.submissionEndDate).toDateString()}
                        </p>
                    {:else if now > new Date(event.revealDate)}
                        <p>Starts on: {new Date(event.submissionStartDate).toDateString()}</p>
                    {:else if now < new Date(event.revealDate)}
                        <p>Reveals on: {new Date(event.revealDate).toDateString()}</p>
                    {/if}

                    <p class="short-desc">{event.shortDescription}</p>
                </div>
            {/each}
        {/if}
    {/each}
{/await}

<style lang="scss">
    h3 {
        margin-top: 2rem;
    }

    .event {
        background-color: var(--color-bg-1);
        border-radius: var(--border-radius);
        border-bottom: 3px solid var(--color-bg-2);
        padding: 1rem;
        margin-bottom: 1rem;

        h5 {
            font-size: var(--fs-medium);
            margin: 0;

            .title {
                margin-bottom: 0.5rem;
            }

            .type,
            .host {
                border-radius: var(--border-radius);
                padding: 0.1rem 2rem;
                color: var(--color-bg);
                display: inline-block;
                font-size: var(--fs-small);
                font-weight: normal;
                margin-bottom: 0.5rem;
            }

            .type.art {
                background-color: var(--color-green);
            }

            .host {
                background-color: var(--color-red);
                margin-left: 0.5rem;
            }
        }

        .hosts {
            font-size: var(--fs-small);
            margin-bottom: 1rem;
        }

        p {
            margin: 0;
            margin-bottom: 0.5rem;

            &.short-desc {
                margin-top: 1rem;
            }
        }
    }
</style>
