<script lang="ts" context="module">
    import type { UserSession } from "src/hooks";

    export const load = async ({ session }: { session: UserSession | Record<string, never> }) => {
        if (session.user) {
            return {
                status: 200
            };
        } else {
            return {
                status: 401
            };
        }
    };
</script>

<script lang="ts">
    import {
        createEvent,
        EventSettings,
        EventType,
        type EventCreateResponse
    } from "$lib/api/event";
    import { goto } from "$app/navigation";

    let vanityUrl: string;
    let title: string;
    let description: string;
    let type: string;
    let randomizeEntries: boolean = true;
    let excludeStaffFromWinning: boolean = true;
    let requireVoteToWin: boolean = false;
    let revealDate: string;
    let submissionStartDate: string;
    let submissionEndDate: string;
    let votingEndDate: string;

    let eventResponse: EventCreateResponse | undefined = undefined;

    const onFormSubmit = async () => {
        let typeEnum: EventType = EventType.ART;
        let settingsEnum: EventSettings = EventSettings.none;

        switch (type) {
            case "art":
                typeEnum = EventType.ART;
                break;
            case "story":
                typeEnum = EventType.STORY;
                break;
            case "coding":
                typeEnum = EventType.CODING;
                break;
            case "gameJam":
                typeEnum = EventType.GAMEJAM;
                break;
        }

        if (randomizeEntries) {
            settingsEnum |= EventSettings.randomizeEntries;
        }
        if (excludeStaffFromWinning) {
            settingsEnum |= EventSettings.excludeStaffFromWinning;
        }
        if (requireVoteToWin) {
            settingsEnum |= EventSettings.requireVoteToWin;
        }

        eventResponse = await createEvent({
            vanityUrl: vanityUrl,
            title: title,
            description: description,
            type: typeEnum,
            settings: settingsEnum,
            revealDate: new Date(revealDate).toISOString(),
            submissionStartDate: new Date(submissionStartDate).toISOString(),
            submissionEndDate: new Date(submissionEndDate).toISOString(),
            voteEndDate: new Date(votingEndDate).toISOString()
        });

        if (eventResponse.ok) {
            goto(`/event/${eventResponse.event?.vanityUrl}`);
        }
    };
</script>

<section>
    <h2>Host a new event</h2>

    {#if eventResponse && !eventResponse.ok}
        <p class="error">Error creating the event: {eventResponse.message}</p>
    {/if}

    <form class="flex col" on:submit|preventDefault={onFormSubmit}>
        <label for="vanityUrl">Vanity URL</label>
        <input
            type="text"
            name="vanityUrl"
            id="vanityUrl"
            placeholder="Vanity Url..."
            maxlength="32"
            bind:value={vanityUrl}
        />

        <label for="title">
            Title: <span class="required">*</span>
        </label>
        <input
            type="text"
            name="title"
            id="title"
            placeholder="Title"
            maxlength="64"
            minlength="4"
            required
            bind:value={title}
        />

        <label for="description">
            Description: <span class="required">*</span>
        </label>
        <textarea
            name="description"
            id="description"
            cols="30"
            rows="10"
            placeholder="Description..."
            maxlength="512"
            required
            bind:value={description}
        />

        <label for="type">
            Type: <span class="required">*</span>
        </label>
        <select name="type" id="type" required bind:value={type}>
            <option value="art">Art</option>
            <option value="story">Story</option>
            <option value="coding">Coding</option>
            <option value="gameJam">Game Jam</option>
        </select>

        <h3>Settings: <span class="required">*</span></h3>

        <div class="flex row center setting">
            <label for="randomizeEntries">Randomize entries</label>
            <input
                type="checkbox"
                name="randomizeEntries"
                id="randomizeEntries"
                bind:checked={randomizeEntries}
            />
        </div>

        <div class="flex row center setting">
            <label for="excludeStaffFromWinning">Exclude staff from winning</label>
            <input
                type="checkbox"
                name="excludeStaffFromWinning"
                id="excludeStaffFromWinning"
                bind:checked={excludeStaffFromWinning}
            />
        </div>

        <div class="flex row center setting">
            <label for="requireVoteToWin">Require vote to win</label>
            <input
                type="checkbox"
                name="requireVoteToWin"
                id="requireVoteToWin"
                bind:checked={requireVoteToWin}
            />
        </div>

        <label for="revealDate">
            Reveal date: <span class="required">*</span>
        </label>
        <input
            type="datetime-local"
            name="revealDate"
            id="revealDate"
            required
            bind:value={revealDate}
        />

        <label for="submissionStartDate">
            Submissions start date: <span class="required">*</span>
        </label>
        <input
            type="datetime-local"
            name="submissionStartDate"
            id="submissionStartDate"
            required
            bind:value={submissionStartDate}
        />

        <label for="submissionEndDate">
            Submissions end date: <span class="required">*</span>
        </label>
        <input
            type="datetime-local"
            name="submissionEndDate"
            id="submissionEndDate"
            required
            bind:value={submissionEndDate}
        />

        <label for="votingEndDate">
            Voting end date: <span class="required">*</span>
        </label>
        <input
            type="datetime-local"
            name="votingEndDate"
            id="votingEndDate"
            bind:value={votingEndDate}
            required
        />

        <button type="submit" class="btn-main">Host new event</button>
    </form>
</section>

<style lang="scss">
    label {
        line-height: 2rem;
        margin-top: 1rem;
    }

    textarea {
        resize: vertical;
    }

    .setting {
        margin-bottom: 1rem;

        label {
            line-height: initial;
            margin: 0;
            margin-right: 2rem;
        }
    }

    button {
        margin-top: 2rem;
    }

    .error {
        background-color: var(--color-red);
        padding: 1rem;
        color: var(--color-bg);
        border-radius: var(--border-radius);
    }
</style>
