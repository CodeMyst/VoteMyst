<script lang="ts">
    import Sortable from "sortablejs";
    import { onMount } from "svelte";

    export let categories: string[] = [];

    let catElements: HTMLElement;

    onMount(() => {
        Sortable.create(catElements, {
            handle: ".reorder",
            animation: 150,
            easing: "cubic-bezier(1, 0, 0, 1)"
        });
    });

    const addCat = () => {
        categories.push("");

        categories = categories; // needed for some reason to update the array in DOM... /shrug
    };

    const removeCat = (idx: number) => {
        categories.splice(idx, 1);

        categories = categories; // needed for some reason to update the array in DOM... /shrug
    };
</script>

<div class="catlist flex col">
    {#if categories.length === 0}
        <span>At least one category is required.</span>
    {/if}

    <div class="cats" bind:this={catElements}>
        {#each categories as cat, idx}
            <div class="cat flex row center">
                {#if categories.length > 1}
                    <button class="reorder">
                        <svg
                            xmlns="http://www.w3.org/2000/svg"
                            class="ionicon"
                            viewBox="0 0 512 512"
                        >
                            <title>Reorder Four</title>
                            <path
                                fill="none"
                                stroke="currentColor"
                                stroke-linecap="round"
                                stroke-linejoin="round"
                                stroke-width="44"
                                d="M102 304h308M102 208h308M102 112h308M102 400h308"
                            />
                        </svg>
                    </button>
                {/if}

                <input
                    type="text"
                    name="catname"
                    placeholder="Category name..."
                    bind:value={categories[idx]}
                />

                <button class="remove" on:click|preventDefault={() => removeCat(idx)}>
                    <svg xmlns="http://www.w3.org/2000/svg" class="ionicon" viewBox="0 0 512 512">
                        <title>Remove Circle</title>
                        <path
                            d="M256 48C141.31 48 48 141.31 48 256s93.31 208 208 208 208-93.31 208-208S370.69 48 256 48zm80 224H176a16 16 0 010-32h160a16 16 0 010 32z"
                            fill="currentColor"
                        />
                    </svg>
                </button>
            </div>
        {/each}
    </div>

    <div class="addcat flex row">
        <button class="btn btn-main" on:click|preventDefault={addCat}>Add category</button>
    </div>
</div>

<style lang="scss">
    .catlist {
        background-color: var(--color-bg);
        border-radius: var(--border-radius);
        border-bottom: 3px solid var(--color-bg-2);
        padding: 1rem;

        span {
            margin-bottom: 1rem;
        }

        .cats {
            .cat {
                margin-bottom: 1rem;
                background-color: var(--color-bg-1);
                padding: 1rem;
                border-radius: var(--border-radius);

                .reorder {
                    margin-right: 1rem;
                    cursor: grab;
                }

                .remove {
                    margin-left: auto;

                    .ionicon {
                        color: var(--color-red);
                    }
                }

                .ionicon {
                    max-width: 25px;
                    color: var(--color-fg);
                }

                button {
                    border: none;
                    padding: 0.25rem;
                }
            }
        }

        .addcat {
            button {
                font-size: var(--fs-small);
                padding: 0.5rem 1rem;
            }
        }
    }

    :global(.sortable-drag) {
        opacity: 0;
    }
</style>
