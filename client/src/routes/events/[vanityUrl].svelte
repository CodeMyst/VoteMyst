<script lang="ts" context="module">
    import type { UserSession } from "src/hooks";
    import { createArtSubmission, EventType, getEvent, hasSubmitted, type ArtEntry, type Event } from "$lib/api/event";
    import { getUserById, type User } from "$lib/api/user";

    export const load = async ({
        session,
        params
    }: {
        session: UserSession | Record<string, never>;
        params: { vanityUrl: string };
    }) => {
        const event = await getEvent(params.vanityUrl, session.user ? session.token : undefined);

        if (event) {
            const host = await getUserById(event.hostIds[0]);

            const submitted = session.user
                ? await hasSubmitted(event.vanityUrl, session.token)
                : false;

            return {
                props: { event, host, submitted }
            };
        }

        return {
            status: 404
        };
    };
</script>

<script lang="ts">
    import SvelteMarkdown from "svelte-markdown";
    import moment from "moment";
    import { onMount } from "svelte";
    import { session } from "$app/stores";
    import type { FetcherResponse } from "$lib/api/fetcher";

    export let event: Event;
    export let host: User;
    export let submitted: boolean;

    const now = moment();

    const submissionStartDate = moment(event.submissionStartDate);
    const submissionEndDate = moment(event.submissionEndDate);
    const votingEndDate = moment(event.voteEndDate);

    const submissionsOpen = now > submissionStartDate && now < submissionEndDate;

    let uploadForm: HTMLFormElement;
    let uploadRes: FetcherResponse<ArtEntry>;

    onMount(() => {
        setInterval(() => {
            timeleft = calcTimeLeft();
        }, 1000);
    });

    const dateToString = (date: moment.Moment): string => {
        return date.format("MMMM Do YYYY [at] HH:mm");
    };

    const calcTimeLeft = (): moment.Duration => {
        let otherdate: moment.Moment = moment();

        if (now < submissionStartDate) {
            otherdate = submissionStartDate;
        } else if (now < submissionEndDate) {
            otherdate = submissionEndDate;
        } else if (now < votingEndDate) {
            otherdate = votingEndDate;
        }

        return moment.duration(otherdate.diff(moment()), "milliseconds");
    };

    let timeleft = calcTimeLeft();

    const onUpload = async () => {
        uploadRes = await createArtSubmission(event.vanityUrl, new FormData(uploadForm));

        if (uploadRes.ok) submitted = true;
    };
</script>

<svelte:head>
    <title>VoteMyst | {event.title}</title>
</svelte:head>

<section>
    <h2 class="flex center sm-row space-between">
        <span class="title">{event.title}</span>

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
        </div>
    </h2>

    <p class="hosts">Hosted by: <a href="/users/{host?.username}">{host?.username}</a></p>

    <div class="time-wrapper">
        <div class="time">
            {#if now < votingEndDate}
                <div class="time-header">
                    Submissions open from
                    <span class="date">
                        {dateToString(submissionStartDate)}
                    </span>
                    to
                    <span class="date">
                        {dateToString(submissionEndDate)}
                    </span>
                </div>

                <div class="hr" />

                <div class="time-timer flex row center">
                    {#if now < submissionStartDate}
                        <span>Starts in</span>
                    {:else if now < submissionEndDate}
                        <span>Submissions due in</span>
                    {:else if now < votingEndDate}
                        <span>Voting ends in</span>
                    {/if}

                    <div class="timer flex row center">
                        {#if timeleft.years() !== 0}
                            <div class="unit">
                                <div class="amount">
                                    {timeleft.years()}
                                </div>

                                <div class="timeframe">years</div>
                            </div>
                        {/if}

                        {#if timeleft.months() !== 0 || timeleft.years() !== 0}
                            <div class="unit">
                                <div class="amount">
                                    {timeleft.months()}
                                </div>

                                <div class="timeframe">months</div>
                            </div>
                        {/if}

                        {#if timeleft.days() !== 0 || timeleft.months() !== 0}
                            <div class="unit">
                                <div class="amount">
                                    {timeleft.days()}
                                </div>

                                <div class="timeframe">days</div>
                            </div>
                        {/if}

                        {#if timeleft.hours() !== 0 || timeleft.days() !== 0}
                            <div class="unit">
                                <div class="amount">
                                    {timeleft.hours()}
                                </div>

                                <div class="timeframe">hours</div>
                            </div>
                        {/if}

                        {#if timeleft.minutes() !== 0 || timeleft.hours() !== 0}
                            <div class="unit">
                                <div class="amount">
                                    {timeleft.minutes()}
                                </div>

                                <div class="timeframe">minutes</div>
                            </div>
                        {/if}

                        {#if timeleft.seconds() !== 0 || timeleft.minutes() !== 0}
                            <div class="unit">
                                <div class="amount">
                                    {timeleft.seconds()}
                                </div>

                                <div class="timeframe">seconds</div>
                            </div>
                        {/if}
                    </div>
                </div>
            {:else}
                <p class="event-over">
                    This event is over. It ran from
                    <span class="date">
                        {dateToString(submissionStartDate)}
                    </span>
                    to
                    <span class="date">
                        {dateToString(submissionEndDate)}
                    </span>
                </p>
            {/if}
        </div>
    </div>

    {#if $session.user && submissionsOpen && event.type === EventType.ART}
        {#if !submitted}
            <div class="upload-submission-wrapper">
                <div class="upload-submission">
                    <p class="upload-header">Upload your submission</p>
                    <div class="hr" />
                    <p>Max file size: 5MB</p>

                    {#if uploadRes && !uploadRes.ok}
                        <p class="error">{uploadRes.message}</p>
                    {/if}

                    <form
                        class="flex col"
                        on:submit|preventDefault={onUpload}
                        bind:this={uploadForm}
                    >
                        <label for="file">File:</label>
                        <input
                            type="file"
                            name="file"
                            id="file"
                            accept="image/png, image/jpeg, image/jpg"
                        />

                        <button type="submit" class="btn btn-main">Upload</button>
                    </form>
                </div>
            </div>
        {/if}
    {/if}

    <p class="description">
        <SvelteMarkdown source={event.description} />
    </p>
</section>

<style lang="scss">
    h2 {
        margin-bottom: 0.5rem;

        .title {
            margin-bottom: 0.5rem;
        }

        .type {
            border-radius: var(--border-radius);
            padding: 0.1rem 2rem;
            color: var(--color-bg);
            display: inline-block;
            font-size: var(--fs-small);
            font-weight: normal;
            margin-bottom: 0.5rem;

            &.art {
                background-color: var(--color-green);
            }
        }
    }

    .hosts {
        margin: 0;
        margin-bottom: 2rem;
        font-size: var(--fs-small);
    }

    .time-wrapper {
        text-align: center;
    }

    .time {
        border-radius: var(--border-radius);
        margin-bottom: 2rem;

        .date {
            font-weight: bold;
        }

        .time-header {
            padding: 1rem;
            text-align: center;
        }

        .time-timer {
            justify-content: center;
            padding: 1rem;

            span {
                margin-right: 1rem;
            }

            .unit {
                border-left: 3px solid var(--color-bg-2);
                padding: 0 1rem;
                text-align: center;
                line-height: 1.5rem;

                .amount {
                    font-weight: bold;
                    font-size: var(--fs-large);
                }

                .timeframe {
                    font-size: 0.75rem;
                }
            }
        }
    }

    .event-over {
        padding: 0 1rem;
    }

    .hr {
        width: 100%;
        background-color: var(--color-bg-2);
        height: 3px;
    }

    .upload-submission-wrapper {
        text-align: center;
    }

    .upload-submission {
        border-radius: var(--border-radius);
        margin-bottom: 2rem;

        .upload-header {
            padding: 1rem;
            text-align: center;
            margin: 0;
        }

        form {
            text-align: left;
            display: inline-block;

            label {
                margin-bottom: 0.25rem;
                margin-right: 0.5rem;
            }

            input {
                margin-bottom: 1rem;
            }
        }
    }
</style>
