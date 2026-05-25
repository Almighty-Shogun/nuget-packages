<template>
    <h2 id="returns">Returns</h2>

    <span v-if="isString" v-html="renderInlineCode(returns)"/>

    <p v-for="({ name, description, type }) of returns" v-else>
        <code><strong>{{ name }}: </strong>{{ type }}</code>

        <br>
        <span v-html="renderInlineCode(description)"/>
    </p>
</template>

<script setup lang="ts">
import { useData } from 'vitepress'
import { computed, unref } from 'vue'
import { renderInlineCode } from './inlineCode'

const { frontmatter } = useData();

const isString = computed(() => typeof unref(frontmatter).returns === "string");
const returns = computed(() => unref(frontmatter).returns || (isString.value ? null : []));
</script>
