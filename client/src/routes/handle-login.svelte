<script lang="ts">
    import { goto } from "$app/navigation";
    import { session } from "$app/stores";
    import { getSelf } from "$lib/api/auth";
    import { onMount } from "svelte";

    onMount(async () => {
        const self = await getSelf();

        if (!self) return;

        $session.user = self;

        goto("/");
    });
</script>
