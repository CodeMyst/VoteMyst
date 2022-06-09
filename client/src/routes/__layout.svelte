<script lang="ts">
    import "../app.scss";
    import Header from "$lib/Header.svelte";
    import Footer from "$lib/Footer.svelte";
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
