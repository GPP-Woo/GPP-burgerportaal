import pluginVue from "eslint-plugin-vue";
import { defineConfigWithVueTs, vueTsConfigs } from "@vue/eslint-config-typescript";
import pluginVitest from "@vitest/eslint-plugin";
import skipFormatting from "@vue/eslint-config-prettier/skip-formatting";
import tseslint from "typescript-eslint";
import js from "@eslint/js";

// To allow more languages other than `ts` in `.vue` files, uncomment the following lines:
// import { configureVueProject } from '@vue/eslint-config-typescript'
// configureVueProject({ scriptLangs: ['ts', 'tsx'] })
// More info at https://github.com/vuejs/eslint-config-typescript/#advanced-setup

export default defineConfigWithVueTs(
  { name: "app/files-to-lint", files: ["**/*.{ts,mts,tsx,vue}"] },

  { name: "app/files-to-ignore", ignores: ["**/dist/**", "**/dist-ssr/**", "**/coverage/**"] },
  js.configs.recommended,
  tseslint.configs.recommended,
  pluginVue.configs["flat/essential"],
  vueTsConfigs.recommended,
  { ...pluginVitest.configs.recommended, files: ["src/**/__tests__/*"] },
  skipFormatting,
  {
    rules: {
      // custom rules here
    }
  }
);
