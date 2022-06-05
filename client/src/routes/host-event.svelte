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

<h2>Host a new event</h2>
