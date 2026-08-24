<template>
    <template v-if="grouped">
        <template v-for="({ name, description, fields: groupFields }) of fields">
            <h2 :id="slugify(name)">{{ name }}</h2>

            <p v-if="description" v-html="renderInlineCode(description)"/>

            <h3 :id="`${slugify(name)}-fields`">Fields</h3>

            <p v-for="field of groupFields">
                <code><strong>{{ field.name }}: </strong>{{ field.type }}</code>

                <br>
                <span v-html="renderInlineCode(field.description)"/>

                <template v-if="field.default">
                    <br>
                    <small><small><strong>Default: </strong><code>{{ field.default }}</code></small></small>
                </template>
            </p>
        </template>
    </template>

    <template v-else>
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

</template>

<script setup lang="ts">
import { useData } from 'vitepress'
import { computed, unref } from 'vue'
import { renderInlineCode } from './inlineCode'

const { frontmatter } = useData();

const fields = computed(() => unref(frontmatter).fields || []);
const grouped = computed(() => fields.value.some((field: any) => Array.isArray(field.fields)));

const slugify = (name: string) => name
    .replace(/([a-z\d])([A-Z])/g, '$1-$2')
    .replace(/[^a-zA-Z\d]+/g, '-')
    .toLowerCase()
    .replace(/^-|-$/g, '');
</script>
