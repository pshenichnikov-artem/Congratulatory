<template>
  <Teleport to="body">
  <div v-if="show" class="fixed inset-0 z-[9999] bg-black/80  flex items-center justify-center">
    <div class="rounded-2xl p-6 border border-white/30 w-full max-w-md mx-4 shadow-2xl" :class="theme.solidBackground">
      <h3 class="text-xl font-semibold text-white mb-4">{{ title }}</h3>
      <p class="text-gray-300 mb-6">{{ message }}</p>
      
      <div class="flex space-x-3">
        <button
          @click="$emit('cancel')"
          class="flex-1 px-4 py-2 bg-white/10 text-white rounded-lg hover:bg-white/20 transition-colors"
        >
          {{ cancelText }}
        </button>
        <button
          @click="$emit('confirm')"
          :disabled="loading"
          class="flex-1 px-4 py-2 text-white rounded-lg font-medium transition-all duration-200 disabled:opacity-50"
          :class="buttonClass"
        >
          <span v-if="!loading">{{ confirmText }}</span>
          <div v-else class="flex items-center justify-center">
            <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
            {{ loadingText }}
          </div>
        </button>
      </div>
    </div>
  </div>
  </Teleport>
</template>

<script setup lang="ts">
import { useThemeStore } from '@/stores/theme'

interface Props {
  show: boolean
  title: string
  message: string
  confirmText?: string
  cancelText?: string
  loadingText?: string
  loading?: boolean
  buttonClass?: string
}

withDefaults(defineProps<Props>(), {
  confirmText: 'Подтвердить',
  cancelText: 'Отмена',
  loadingText: 'Загрузка...',
  loading: false,
  buttonClass: 'bg-gradient-to-r from-red-600 to-red-700 hover:from-red-700 hover:to-red-800'
})

defineEmits<{
  confirm: []
  cancel: []
}>()

const theme = useThemeStore()
</script>