<footer>
    <ul class="flex sm-row center">
        <li>
            Copyright © <a href="https://github.com/CodeMyst">CodeMyst</a>,
            <a href="https://github.com/YilianSource">Yilian</a>,
            <a href="https://github.com/AquaXV">AquaXV</a>,
            <a href="https://github.com/Duxez">Job</a>
            {new Date().getFullYear()}
        </li>

        <li><a href="/changelog">v2.0.0-beta</a></li>

        <li><a href="https://github.com/CodeMyst/VoteMyst">GitHub</a></li>

        <li><a href="/legal">Terms of Service</a></li>
    </ul>
</footer>

<style lang="scss">
    footer {
        background-color: var(--color-bg-1);
        padding: 0.25rem 1rem;
        border-radius: var(--border-radius);
        border-bottom: 3px solid var(--color-bg-2);
        flex-shrink: 0;
        margin-bottom: 1rem;

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
        footer ul li {
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
