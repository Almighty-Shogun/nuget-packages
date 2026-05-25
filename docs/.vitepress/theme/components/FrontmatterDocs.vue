<template>
    <Parameters v-if="frontmatter.params"/>
    <Fields v-if="frontmatter.fields"/>
    <Returns v-if="frontmatter.returns"/>
</template>

<script setup lang="ts">
import Fields from './Fields.vue'
import { useData } from 'vitepress'
import Returns from './Returns.vue'
import Parameters from './Parameters.vue'

const { frontmatter } = useData();
</script>
