<script lang="ts">
    import { session } from "$app/stores";
    import { UserRole } from "./api/user";
</script>

<header>
    <div class="flex row space-between center">
        <a href="/" class="no-dec"><h1>VoteMyst</h1></a>

        {#if $session.user}
            <a href="/user/profile" class="btn user flex row center">
                {#if $session.user.role === UserRole.ADMIN}
                    <span class="admin">Admin</span>
                {/if}
                {$session.user.username}
                <img src={$session.user.avatarUrl} alt="{$session.user.username}'s avatar" />
            </a>
        {:else}
            <a href="/login" class="btn">Login / Register</a>
        {/if}
    </div>

    <h2>A website for hosting various events where users can vote on user-submitted content.</h2>
</header>

<nav>
    <ul class="flex sm-row center">
        <li><a href="/">Home</a></li>
        <li><a href="/events">Events</a></li>
        <li><a href="/legal">Terms of Service</a></li>

        {#if $session.user && $session.user.role === UserRole.ADMIN}
            <li class="admin-link"><a href="/host-event">Host Event</a></li>
        {/if}
    </ul>
</nav>

<style lang="scss">
    header {
        padding: 1rem 0;
    }

    h1 {
        margin: 0;
    }

    h2 {
        font-size: var(--fs-small);
        font-weight: normal;
    }

    nav {
        background-color: var(--color-bg-1);
        padding: 0.25rem 1rem;
        border-radius: var(--border-radius);
        border-bottom: 3px solid var(--color-bg-2);

        ul {
            list-style: none;
            padding: 0;

            li {
                margin-right: 0.75rem;

                &::after {
                    content: "-";
                    margin-left: 0.75rem;
                    color: var(--color-bg-3);
                }

                &:last-child::after {
                    content: "";
                    margin: 0;
                }

                &.admin-link {
                    background-color: var(--color-red);
                    padding: 0.25rem 0.5rem;
                    border-radius: var(--border-radius);
                    @include transition();

                    &:hover {
                        background-color: var(--color-bg);

                        a {
                            color: var(--color-red);
                        }
                    }

                    a {
                        color: var(--color-bg);
                    }
                }

                a {
                    text-decoration: none;
                }
            }
        }
    }

    .user {
        font-size: var(--fs-normal);
        text-decoration: none;
        word-break: break-word;

        .admin {
            background-color: var(--color-red);
            color: var(--color-bg);
            border-radius: var(--border-radius);
            padding: 0.25rem;
            margin-right: 0.5rem;
        }

        img {
            max-width: 28px;
            margin: 0;
            margin-left: 1rem;
            border-radius: var(--border-radius);
        }
    }

    @media screen and (max-width: 620px) {
        nav ul li {
            margin-bottom: 1rem;

            &::after {
                content: "";
            }

            &:last-child {
                margin-bottom: 0;
            }
        }
    }
</style>
