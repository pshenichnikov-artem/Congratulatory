<template>
  <teleport to="body">
    <div v-if="show" class="fixed inset-0 z-[9999] bg-black/80 flex items-center justify-center">
    <div class="rounded-2xl p-6 border border-white/30 w-full max-w-md mx-4 shadow-2xl" :class="theme.solidBackground">
      <h3 class="text-xl font-semibold text-white mb-4">Выберите дату напоминания</h3>
      
      <div class="mb-6">
        <div class="grid grid-cols-2 gap-4 mb-4">
          <div>
            <label class="block text-sm font-medium text-white mb-2">Месяц</label>
            <select 
              v-model="selectedMonth"
              class="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-white/30"
            >
              <option v-for="(month, index) in months" :key="index" :value="index + 1" class="bg-gray-800">
                {{ month }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-white mb-2">День</label>
            <select 
              v-model="selectedDay"
              class="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-white/30"
            >
              <option v-for="day in daysInMonth" :key="day" :value="day" class="bg-gray-800">
                {{ day }}
              </option>
            </select>
          </div>
        </div>
        <div>
          <label class="block text-sm font-medium text-white mb-2">Время</label>
          <input 
            v-model="selectedTime"
            type="time"
            class="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-white/30"
          />
        </div>
      </div>
      
      <div class="flex space-x-3">
        <button
          @click="$emit('cancel')"
          class="flex-1 px-4 py-2 bg-white/10 text-white rounded-lg hover:bg-white/20 transition-colors"
        >
          Отмена
        </button>
        <button
          @click="confirmDate"
          :disabled="!selectedMonth || !selectedDay"
          class="flex-1 px-4 py-2 text-white rounded-lg font-medium transition-all duration-200 disabled:opacity-50"
          :class="theme.button"
        >
          Подтвердить
        </button>
      </div>
    </div>
    </div>
  </teleport>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useThemeStore } from '@/stores/theme'

interface Props {
  show: boolean
  currentDate?: string
}

const props = defineProps<Props>()

const emit = defineEmits<{
  confirm: [date: string]
  cancel: []
}>()

watch(() => props.currentDate, (currentDate) => {
  if (currentDate) {
    const [datePart, timePart] = currentDate.split(' ')
    const [month, day] = datePart.split('-')
    selectedMonth.value = parseInt(month)
    selectedDay.value = parseInt(day)
    selectedTime.value = timePart || '09:00'
  } else {
    selectedMonth.value = 1
    selectedDay.value = 1
    selectedTime.value = '09:00'
  }
})

const theme = useThemeStore()

const selectedMonth = ref<number>(1)
const selectedDay = ref<number>(1)
const selectedTime = ref<string>('09:00')

const months = [
  'Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
  'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'
]

const daysInMonth = computed(() => {
  const daysCount = new Date(2024, selectedMonth.value, 0).getDate()
  return Array.from({ length: daysCount }, (_, i) => i + 1)
})

function confirmDate() {
  const date = `${selectedMonth.value.toString().padStart(2, '0')}-${selectedDay.value.toString().padStart(2, '0')} ${selectedTime.value}`
  emit('confirm', date)
}
</script>