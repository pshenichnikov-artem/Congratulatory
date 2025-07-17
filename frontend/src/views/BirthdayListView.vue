<template>
  <div class="min-h-screen relative overflow-hidden" :class="theme.background">
    <!-- Animated background elements -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="absolute -top-40 -right-40 w-80 h-80 bg-violet-500 rounded-full mix-blend-multiply filter blur-xl opacity-30 animate-blob"></div>
      <div class="absolute -bottom-40 -left-40 w-80 h-80 bg-blue-500 rounded-full mix-blend-multiply filter blur-xl opacity-30 animate-blob animation-delay-2000"></div>
      <div class="absolute top-40 left-40 w-80 h-80 bg-fuchsia-500 rounded-full mix-blend-multiply filter blur-xl opacity-30 animate-blob animation-delay-4000"></div>
    </div>

    <div class="relative z-10">
      
      <AddBirthdayModal 
        :show="showAddModal" 
        :relationship-type="relationshipTypes[currentTab] || 'family'"
        :editing-birthday="editingBirthday"
        @close="closeModal"
        @success="handleAddSuccess"
      />
      
      <div class="max-w-7xl mx-auto px-2 sm:px-4 lg:px-8 py-4 sm:py-8">
        <div class="bg-white/10 backdrop-blur-lg rounded-2xl sm:rounded-3xl shadow-2xl p-3 sm:p-6 border border-white/20">
          <div class="flex flex-col sm:flex-row sm:justify-between sm:items-center mb-4 sm:mb-6 space-y-3 sm:space-y-0">
            <h1 class="text-2xl sm:text-3xl font-bold text-white">
              {{ getTitle() }}
            </h1>
            <button
              @click="openAddModal"
              class="inline-flex items-center justify-center px-4 py-2 text-white rounded-xl font-medium transition-all duration-200 transform hover:scale-105 shadow-lg w-full sm:w-auto"
              :class="theme.button"
            >
              <PlusIcon class="w-5 h-5 mr-2" />
              Добавить
            </button>
          </div>

          <div class="mb-4 sm:mb-6">
            <BirthdayFilters
              :filters="filters"
              :sort-by="sortBy"
              :sort-direction="sortDirection"
              @update:filters="filters = $event"
              @update:sort-by="sortBy = $event"
              @toggle-sort="toggleSortDirection"
              @clear-filters="clearFilters"
            />
          </div>

          <div v-if="loading.search" class="flex justify-center py-8 sm:py-12">
            <div class="animate-spin rounded-full h-8 w-8 sm:h-12 sm:w-12 border-b-2" :class="theme.spinner"></div>
          </div>

          <div v-else-if="birthdays.length === 0" class="text-center py-8 sm:py-12">
            <div class="text-gray-300 text-base sm:text-lg">
              Список пуст
            </div>
          </div>

          <div v-else>
            <div class="space-y-3 sm:space-y-4">
              <BirthdayCard
                v-for="birthday in birthdays"
                :key="birthday.id"
                :birthday="birthday"
                @refresh="handleRefresh"
                @edit="openEditModal"
              />
            </div>
            
            <div v-if="loadingMore" class="flex justify-center py-3 sm:py-4">
              <div class="animate-spin rounded-full h-6 w-6 sm:h-8 sm:w-8 border-b-2" :class="theme.spinner"></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'

import AddBirthdayModal from '@/components/AddBirthdayModal.vue'
import BirthdayCard from '@/components/BirthdayCard.vue'
import BirthdayFilters from '@/components/BirthdayFilters.vue'
import { useBirthdayApi } from '@/composables/useBirthdayApi'
import type { BirthdayResponse } from '@/types/birthday/BirthdayResponse'
import { PlusIcon } from '@/components/icons'
import { useThemeStore } from '@/stores/theme'

const router = useRouter()
const birthdayApi = useBirthdayApi()
const { loading } = birthdayApi

const route = useRoute()
const currentTab = computed(() => route.name as string || 'family')
const birthdays = ref<BirthdayResponse[]>([])

const filters = ref({
  name: '',
  month: '',
  upcomingDays: ''
})

const sortBy = ref('name')
const sortDirection = ref(0)


const showAddModal = ref(false)
const editingBirthday = ref<BirthdayResponse | null>(null)
const hasMore = ref(true)
const loadingMore = ref(false)
const theme = useThemeStore()

const relationshipTypes: Record<string, string> = {
  family: 'family',
  friends: 'friends',
  colleagues: 'colleagues'
}

function getTitle() {
  const titles: Record<string, string> = {
    family: 'Семья',
    friends: 'Друзья',
    colleagues: 'Коллеги',
    calendar: 'Календарь',
    profile: 'Профиль'
  }
  return titles[currentTab.value] || 'Дни рождения'
}

async function loadCurrentTabBirthdays() {
  await loadBirthdays(relationshipTypes[currentTab.value] || undefined)
}

async function loadBirthdays(relationshipType?: string, reset = true) {
  if (reset) {
    birthdays.value = []
    hasMore.value = true
  }
  
  const searchRequest = {
    filter: {
      relationshipType,
      name: filters.value.name || undefined,
      month: filters.value.month ? parseInt(filters.value.month) : undefined,
      upcomingDays: filters.value.upcomingDays ? parseInt(filters.value.upcomingDays) : undefined
    },
    sort: {
      sortBy: sortBy.value,
      direction: sortDirection.value
    }
  }
  
  const result = await birthdayApi.searchBirthdays(searchRequest)
  if (result) {
    if (reset) {
      birthdays.value = result
    } else {
      birthdays.value.push(...result)
    }
    hasMore.value = result.length === 10
  }
}

async function handleRefresh() {
  await loadBirthdays(relationshipTypes[currentTab.value] || undefined)
}

function openAddModal() {
  editingBirthday.value = null
  showAddModal.value = true
}

function openEditModal(birthday: BirthdayResponse) {
  editingBirthday.value = birthday
  showAddModal.value = true
}

function closeModal() {
  showAddModal.value = false
  editingBirthday.value = null
}

async function handleAddSuccess() {
  await handleRefresh()
}

function toggleSortDirection() {
  sortDirection.value = sortDirection.value === 1 ? 0 : 1
  loadCurrentTabBirthdays()
}

function clearFilters() {
  filters.value = {
    name: '',
    month: '',
    upcomingDays: ''
  }
  loadCurrentTabBirthdays()
}

watch(() => route.name, () => {
  theme.setTab(currentTab.value)
  loadCurrentTabBirthdays()
}, { immediate: true })

watch([() => filters.value.name, () => filters.value.month, () => filters.value.upcomingDays, sortBy], () => {
  loadCurrentTabBirthdays()
}, { deep: true })
</script>

<style scoped>
@keyframes blob {
  0% {
    transform: translate(0px, 0px) scale(1);
  }
  33% {
    transform: translate(30px, -50px) scale(1.1);
  }
  66% {
    transform: translate(-20px, 20px) scale(0.9);
  }
  100% {
    transform: translate(0px, 0px) scale(1);
  }
}

.animate-blob {
  animation: blob 7s infinite;
}

.animation-delay-2000 {
  animation-delay: 2s;
}

.animation-delay-4000 {
  animation-delay: 4s;
}
</style>