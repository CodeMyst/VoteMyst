<script lang="ts">
    import { goto } from "$app/navigation";
    import { page, session } from "$app/stores";
    import { createAccount, getSelf } from "$lib/api/auth";
    import { getUserByUsername } from "$lib/api/user";
    import { deleteCookie, setCookie } from "$lib/util/cookies";

    let usernameValid = true;
    let usernameErrorMsg: string;

    let createAccountError = false;

    let username = $page.url.searchParams.get("username");
    let usernameInput: HTMLInputElement;

    const usernameRegex = /^[\w.-]+$/m;

    const onUsernameInput = async () => {
        await validateUsername();
    };

    const onFormSubmit = async () => {
        if (!username) return;

        await validateUsername();

        if (!usernameValid) {
            usernameInput.focus();
            return;
        }

        const token = await createAccount(username);

        if (!token) {
            createAccountError = true;
            return;
        }

        setCookie("votemyst", token, 30);
        deleteCookie("votemyst-registration");

        const self = await getSelf();

        if (!self) return;

        $session.user = self;

        goto("/");
    };

    const validateUsername = async () => {
        if (!username) return;

        if (await getUserByUsername(username)) {
            usernameValid = false;
            usernameErrorMsg = "This username is already taken.";
        } else if (username.length === 0) {
            usernameValid = false;
            usernameErrorMsg = "The username can't be empty.";
        } else if (!usernameRegex.test(username)) {
            usernameValid = false;
            usernameErrorMsg = "The username contains invalid symbols.";
        } else {
            usernameValid = true;
            usernameErrorMsg = "";
        }
    };
</script>

<section>
    <h2>Create a new account</h2>

    <p>
        The username has to be unique, it can contain alphanumeric characters, and only the symbols:
        <code>.</code>, <code>-</code>, <code>_</code>
    </p>

    <form class="flex col" on:submit|preventDefault={onFormSubmit}>
        {#if createAccountError}
            <p class="eror-message">
                There was an issue creating the account. Please try again. If the issue persists
                please <a href="/contact">contact us</a>.
            </p>
        {/if}

        <label for="username">
            Username: <span class="required">*</span>

            {#if !usernameValid}
                <span class="error">{usernameErrorMsg}</span>
            {/if}
        </label>

        <input
            type="text"
            required
            name="username"
            id="username"
            placeholder="Username..."
            bind:value={username}
            bind:this={usernameInput}
            on:input={onUsernameInput}
            maxlength="20"
            minlength="2"
        />

        <button type="submit" class="btn-main">Create account</button>
    </form>
</section>

<style lang="scss">
    label {
        line-height: 2rem;

        .error {
            color: var(--color-red);
            font-weight: normal;
        }
    }

    button {
        margin-top: 2rem;
    }

    label,
    input,
    button {
        width: 100%;
    }
</style>
