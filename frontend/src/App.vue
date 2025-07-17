<script setup lang="ts">
import { RouterView } from 'vue-router'
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import Navbar from '@/components/Navbar.vue'
import Notification from '@/components/Notification.vue'

const route = useRoute()
const showNavbar = computed(() => {
  return !['login', 'register'].includes(route.name as string)
})
</script>

<template>
  <div>
    <Navbar v-if="showNavbar" />
    <RouterView />
    <Notification />
  </div>
</template>
