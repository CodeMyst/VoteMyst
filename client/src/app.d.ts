/// <reference types="@sveltejs/kit" />

import type { User } from "$lib/api/user";

// See https://kit.svelte.dev/docs/types#app
// for information about these interfaces
declare global {
    declare namespace App {
        // interface Locals {}
        // interface Platform {}
        // interface Stuff {}

        interface Session {
            user: User;
            token: string;
        }
    }
}
