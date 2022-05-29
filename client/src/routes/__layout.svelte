<script lang="ts">
    import "../app.scss";
    import Header from "$lib/Header.svelte";
    import Footer from "$lib/Footer.svelte";
    import { onMount } from "svelte";
    import { currentUserStore } from "$lib/stores";
    import { getSelf } from "$lib/api/auth";

    onMount(async () => {
        $currentUserStore = await getSelf();
    });
</script>

<div id="container">
    <Header />

    <main>
        <slot />
    </main>

    <Footer />
</div>

<style lang="scss">
    main {
        flex: 1 0 auto;
    }
</style>
