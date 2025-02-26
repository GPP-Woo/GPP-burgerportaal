import { OurComponents } from "./components/register";

declare module "@vue/runtime-core" {
  // eslint-disable-next-line @typescript-eslint/no-empty-object-type
  export interface GlobalComponents extends OurComponents {}
}
