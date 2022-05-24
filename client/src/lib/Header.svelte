<header>
    <div class="flex row space-between center">
        <a href="/" class="no-dec"><h1>VoteMyst</h1></a>

        <a href="/login" class="btn">Login / Register</a>
    </div>

    <h2>A website for hosting various events where users can vote on user-submitted content.</h2>
</header>

<nav>
    <ul class="flex sm-row center">
        <li><a href="/">Home</a></li>
        <li><a href="/events">Events</a></li>
        <li><a href="/legal">Terms of Service</a></li>
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
                }

                a {
                    text-decoration: none;
                }
            }
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
