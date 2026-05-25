<template>
    <h2 id="fields">Fields</h2>

    <p v-for="({ name, description, type, default: defaultValue }) of fields">
        <code><strong>{{ name }}: </strong>{{ type }}</code>

        <br>
        <span v-html="renderInlineCode(description)"/>

        <template v-if="defaultValue">
            <br>
            <small><small><strong>Default: </strong><code>{{ defaultValue }}</code></small></small>
        </template>
    </p>

</template>

<script setup lang="ts">
import { useData } from 'vitepress'
import { computed, unref } from 'vue'
import { renderInlineCode } from './inlineCode'

const { frontmatter } = useData();

const fields = computed(() => unref(frontmatter).fields || []);
</script>
