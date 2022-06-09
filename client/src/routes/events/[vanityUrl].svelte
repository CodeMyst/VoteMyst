<script lang="ts" context="module">
    import type { UserSession } from "src/hooks";
    import { EventType, getEvent, type Event } from "$lib/api/event";
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

            return {
                props: { event, host }
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

    export let event: Event;
    export let host: User;

    const now = moment();

    const submissionStartDate = moment(event.submissionStartDate);
    const submissionEndDate = moment(event.submissionEndDate);
    const votingEndDate = moment(event.votingEndDate);

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

                <div class="time-timer flex row center">
                    {#if now < submissionStartDate}
                        <span>Starts in</span>
                    {:else if now < submissionEndDate}
                        <span>Submissions due in</span>
                    {:else if now < votingEndDate}
                        <span>Voting end in</span>
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
                <p>
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
        border: 3px solid var(--color-bg-2);
        border-radius: var(--border-radius);
        margin-bottom: 2rem;
        display: inline-block;

        .date {
            font-weight: bold;
        }

        .time-header {
            padding: 1rem;
            text-align: center;
            border-bottom: 3px solid var(--color-bg-2);
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

    @media screen and (max-width: 620px) {
        .time {
            border: 0;
            border-top: 3px solid var(--color-bg-2);
            margin: -1rem;
        }
    }
</style>
