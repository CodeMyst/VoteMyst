import { writable, type Writable } from "svelte/store";
import type { User } from "./api/user";

export const currentUserStore: Writable<User | null> = writable(null);
