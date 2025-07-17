<template>
  <Teleport to="body">
    <div v-if="show" class="fixed inset-0 z-[9999] bg-black/80 flex items-center justify-center">
      <div class="rounded-2xl p-6 border border-white/30 w-full max-w-md mx-4 shadow-2xl" :class="theme.solidBackground">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-xl font-semibold text-white">Дни рождения</h3>
          <button @click="$emit('close')" class="text-gray-300 hover:text-white">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>
        
        <div class="space-y-3">
          <div 
            v-for="birthday in birthdays" 
            :key="birthday.id"
            class="flex items-center space-x-3 p-3 bg-white/5 rounded-lg"
          >
            <div class="flex-shrink-0">
              <img 
                v-if="birthday.photoPath" 
                :src="birthday.photoPath" 
                :alt="birthday.fullName"
                class="w-10 h-10 rounded-full object-cover border-2 border-white/20"
              />
              <div 
                v-else 
                class="w-10 h-10 rounded-full bg-gradient-to-br from-emerald-400 to-green-500 flex items-center justify-center text-white font-bold"
              >
                {{ birthday.fullName.charAt(0).toUpperCase() }}
              </div>
            </div>
            <div class="flex-1">
              <div class="text-white font-medium">{{ birthday.fullName }}</div>
              <div class="text-gray-300 text-sm">{{ formatAge(birthday.dateOfBirth) }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { useThemeStore } from '@/stores/theme'
import type { BirthdayResponse } from '@/types/birthday/BirthdayResponse'

interface Props {
  show: boolean
  birthdays: BirthdayResponse[]
}

defineProps<Props>()

defineEmits<{
  close: []
}>()

const theme = useThemeStore()

function formatAge(dateString: string) {
  const birthDate = new Date(dateString)
  const today = new Date()
  const age = today.getFullYear() - birthDate.getFullYear()
  return `${age} лет`
}
</script>