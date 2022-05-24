import adapter from "@sveltejs/adapter-auto";
import preprocess from "svelte-preprocess";

/** @type {import('@sveltejs/kit').Config} */
const config = {
    // Consult https://github.com/sveltejs/svelte-preprocess
    // for more information about preprocessors
    preprocess: [
        preprocess({
            scss: {
                prependData: `@use "src/mixins.scss" as *;`
            }
        })
    ],

    kit: {
        adapter: adapter(),

        vite: {
            css: {
                preprocessorOptions: {
                    scss: {
                        additionalData: `@use "src/mixins.scss" as *;`
                    }
                }
            }
        }
    }
};

export default config;
