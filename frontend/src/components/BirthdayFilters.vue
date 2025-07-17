<template>
  <div class="bg-white/5 rounded-xl sm:rounded-2xl p-3 sm:p-4 mb-4 sm:mb-6 space-y-3 sm:space-y-4">
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-4">
      <!-- Name Filter -->
      <div>
        <label class="block text-sm text-white mb-1">Поиск по имени</label>
        <input
          :value="filters.name"
          @input="updateFilter('name', ($event.target as HTMLInputElement)?.value || '')"
          type="text"
          placeholder="Введите имя"
          class="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-white/30 text-sm"
        />
      </div>
      
      <!-- Month Filter -->
      <div>
        <label class="block text-sm text-white mb-1">Месяц</label>
        <select
          :value="filters.month"
          @change="updateFilter('month', ($event.target as HTMLSelectElement)?.value || '')"
          class="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-white/30 text-sm"
        >
          <option value="" class="bg-gray-800">Все месяцы</option>
          <option v-for="(month, index) in months" :key="index" :value="index + 1" class="bg-gray-800">
            {{ month }}
          </option>
        </select>
      </div>
      
      <!-- Upcoming Days Filter -->
      <div>
        <label class="block text-sm text-white mb-1">Ближайшие дни</label>
        <select
          :value="filters.upcomingDays"
          @change="updateFilter('upcomingDays', ($event.target as HTMLSelectElement)?.value || '')"
          class="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-white/30 text-sm"
        >
          <option value="" class="bg-gray-800">Все</option>
          <option value="7" class="bg-gray-800">7 дней</option>
          <option value="30" class="bg-gray-800">30 дней</option>
          <option value="90" class="bg-gray-800">90 дней</option>
        </select>
      </div>
      
      <!-- Sort -->
      <div>
        <label class="block text-sm text-white mb-1">Сортировка</label>
        <select
          :value="sortBy"
          @change="$emit('update:sortBy', ($event.target as HTMLSelectElement)?.value || '')"
          class="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-white/30 text-sm"
        >
          <option value="name" class="bg-gray-800">По имени</option>
          <option value="dateofbirth" class="bg-gray-800">По дате рождения</option>
          <option value="upcomingdays" class="bg-gray-800">По ближайшим дням</option>
        </select>
      </div>
    </div>
    
    <div class="flex flex-col sm:flex-row sm:justify-between sm:items-center space-y-2 sm:space-y-0">
      <div class="flex flex-col sm:flex-row sm:items-center space-y-2 sm:space-y-0 sm:space-x-2">
        <label class="text-sm text-white">Направление:</label>
        <button
          @click="$emit('toggleSort')"
          class="px-3 py-1 rounded-lg transition-colors text-sm w-full sm:w-auto"
          :class="theme.button"
        >
          {{ sortDirection === 1 ? '↑ По возрастанию' : '↓ По убыванию' }}
        </button>
      </div>
      
      <button
        @click="$emit('clearFilters')"
        class="px-3 py-1 text-sm bg-white/10 text-white rounded-lg hover:bg-white/20 transition-colors w-full sm:w-auto"
      >
        Очистить фильтры
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useThemeStore } from '@/stores/theme'

interface Filters {
  name: string
  month: string
  upcomingDays: string
}

interface Props {
  filters: Filters
  sortBy: string
  sortDirection: number
}

const props = defineProps<Props>()

const emit = defineEmits<{
  'update:filters': [filters: Filters]
  'update:sortBy': [sortBy: string]
  toggleSort: []
  clearFilters: []
}>()

const theme = useThemeStore()

const months = [
  'Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
  'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'
]

function updateFilter(key: keyof Filters, value: string) {
  const newFilters = { ...props.filters, [key]: value }
  emit('update:filters', newFilters)
}
</script>