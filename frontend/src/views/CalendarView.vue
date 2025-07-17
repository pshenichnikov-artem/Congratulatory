<template>
  <div class="min-h-screen relative overflow-hidden" :class="theme.background">
    <!-- Animated background elements -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="absolute -top-40 -right-40 w-80 h-80 bg-green-500 rounded-full mix-blend-multiply filter blur-xl opacity-30 animate-blob"></div>
      <div class="absolute -bottom-40 -left-40 w-80 h-80 bg-emerald-500 rounded-full mix-blend-multiply filter blur-xl opacity-30 animate-blob animation-delay-2000"></div>
      <div class="absolute top-40 left-40 w-80 h-80 bg-teal-500 rounded-full mix-blend-multiply filter blur-xl opacity-30 animate-blob animation-delay-4000"></div>
    </div>

    <div class="relative z-10">
      
      <div class="max-w-7xl mx-auto px-2 sm:px-4 lg:px-8 py-4 sm:py-8">
        <div class="bg-white/10 backdrop-blur-lg rounded-2xl sm:rounded-3xl shadow-2xl p-3 sm:p-6 border border-white/20">
          <h1 class="text-2xl sm:text-3xl font-bold text-white mb-4 sm:mb-8">Календарь</h1>
          
          <!-- Month Navigation -->
          <div class="flex items-center justify-between mb-4 sm:mb-8">
            <button 
              @click="previousMonth"
              class="p-2 sm:p-3 rounded-lg sm:rounded-xl transition-all duration-200 transform hover:scale-110 shadow-lg" 
              :class="theme.button"
            >
              <svg class="w-5 h-5 sm:w-6 sm:h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path>
              </svg>
            </button>
            <div class="text-center">
              <h2 class="text-2xl sm:text-4xl font-bold text-white mb-1 bg-gradient-to-r from-white to-gray-300 bg-clip-text text-transparent">
                {{ currentMonthName }}
              </h2>
            </div>
            <button 
              @click="nextMonth"
              class="p-2 sm:p-3 rounded-lg sm:rounded-xl transition-all duration-200 transform hover:scale-110 shadow-lg" 
              :class="theme.button"
            >
              <svg class="w-5 h-5 sm:w-6 sm:h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
              </svg>
            </button>
          </div>

          <!-- Calendar Grid -->
          <div class="bg-white/5 backdrop-blur-sm rounded-2xl sm:rounded-3xl p-3 sm:p-6 border border-white/10 shadow-2xl">
            <!-- Days of Week -->
            <div class="grid grid-cols-7 gap-1 sm:gap-3 mb-3 sm:mb-6">
              <div v-for="day in daysOfWeek" :key="day" class="text-center text-white font-bold py-2 sm:py-3 text-sm sm:text-lg">
                {{ day }}
              </div>
            </div>
            
            <!-- Calendar Days -->
            <div class="grid grid-cols-7 gap-1 sm:gap-3">
              <div 
                v-for="day in calendarDays" 
                :key="day.date"
                class="min-h-[60px] sm:min-h-[80px] p-1 sm:p-2 rounded border transition-all duration-300 hover:scale-105 relative overflow-hidden"
                :class="{
                  'bg-white/10 border-white/20 text-white': day.isCurrentMonth,
                  'bg-white/5 border-white/10 text-gray-400': !day.isCurrentMonth,
                  'bg-gradient-to-br from-emerald-500/30 to-green-500/30 border-emerald-400/50 shadow-lg shadow-emerald-500/20': day.hasBirthday && day.isCurrentMonth,
                  'bg-gradient-to-br from-emerald-500/20 to-green-500/20 border-emerald-400/30': day.hasBirthday && !day.isCurrentMonth
                }"
              >
                <!-- Day Number -->
                <div class="font-bold text-xs sm:text-sm mb-1" :class="{
                  'text-white': day.isCurrentMonth,
                  'text-gray-500': !day.isCurrentMonth,
                  'text-emerald-100': day.hasBirthday
                }">
                  {{ day.day }}
                </div>
                
                <!-- Birthday Indicators -->
                <div v-if="day.birthdays.length > 0" class="space-y-1">
                  <div 
                    class="flex items-center space-x-1 sm:space-x-2 p-0.5 sm:p-1 rounded transition-all duration-200 cursor-pointer"
                    :class="{
                      'bg-emerald-400/20 border border-emerald-400/30': day.isCurrentMonth,
                      'bg-emerald-400/10 border border-emerald-400/20': !day.isCurrentMonth
                    }"
                    @click="openBirthdayModal(day.birthdays)"
                  >
                    <!-- Avatar -->
                    <div class="flex-shrink-0">
                      <img 
                        v-if="day.birthdays[0].photoPath" 
                        :src="day.birthdays[0].photoPath" 
                        :alt="day.birthdays[0].fullName"
                        class="w-3 h-3 sm:w-4 sm:h-4 rounded-full object-cover border border-white/30"
                      />
                      <div 
                        v-else 
                        class="w-3 h-3 sm:w-4 sm:h-4 rounded-full bg-gradient-to-br from-emerald-400 to-green-500 flex items-center justify-center text-white font-bold text-xs"
                      >
                        {{ day.birthdays[0].fullName.charAt(0).toUpperCase() }}
                      </div>
                    </div>
                    
                    <!-- Name or count -->
                    <span class="text-xs font-medium text-white truncate flex-1 hidden sm:inline">
                      {{ day.birthdays.length === 1 ? day.birthdays[0].fullName.split(' ')[0] : `${day.birthdays[0].fullName.split(' ')[0]} +${day.birthdays.length - 1}` }}
                    </span>
                  </div>
                </div>
                
                <!-- Birthday count badge -->
                <div v-if="day.hasBirthday" class="absolute top-0.5 right-0.5 sm:top-1 sm:right-1">
                  <div class="w-3 h-3 sm:w-4 sm:h-4 bg-emerald-500 rounded-full flex items-center justify-center">
                    <span class="text-xs font-bold text-white">{{ day.birthdays.length }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Birthday List Modal -->
      <BirthdayListModal
        :show="showBirthdayModal"
        :birthdays="selectedBirthdays"
        @close="closeBirthdayModal"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'

import BirthdayListModal from '@/components/BirthdayListModal.vue'
import { useThemeStore } from '@/stores/theme'
import { useBirthdayApi } from '@/composables/useBirthdayApi'
import type { BirthdayResponse } from '@/types/birthday/BirthdayResponse'

const theme = useThemeStore()
const birthdayApi = useBirthdayApi()

const currentMonth = ref(new Date().getMonth())
const birthdays = ref<BirthdayResponse[]>([])
const showBirthdayModal = ref(false)
const selectedBirthdays = ref<BirthdayResponse[]>([])

const months = [
  'Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
  'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'
]

const daysOfWeek = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс']

const currentMonthName = computed(() => months[currentMonth.value])

const calendarDays = computed(() => {
  const year = new Date().getFullYear()
  const firstDay = new Date(year, currentMonth.value, 1)
  const lastDay = new Date(year, currentMonth.value + 1, 0)
  const startDate = new Date(firstDay)
  startDate.setDate(startDate.getDate() - (firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1))
  
  const days = []
  const currentDate = new Date(startDate)
  
  for (let i = 0; i < 42; i++) {
    const dayBirthdays = birthdays.value.filter(birthday => {
      const birthDate = new Date(birthday.dateOfBirth)
      return birthDate.getDate() === currentDate.getDate() && 
             birthDate.getMonth() === currentDate.getMonth()
    })
    
    days.push({
      date: currentDate.toISOString().split('T')[0],
      day: currentDate.getDate(),
      isCurrentMonth: currentDate.getMonth() === currentMonth.value,
      hasBirthday: dayBirthdays.length > 0,
      birthdays: dayBirthdays
    })
    
    currentDate.setDate(currentDate.getDate() + 1)
  }
  
  return days
})

function previousMonth() {
  currentMonth.value = currentMonth.value === 0 ? 11 : currentMonth.value - 1
}

function nextMonth() {
  currentMonth.value = currentMonth.value === 11 ? 0 : currentMonth.value + 1
}

async function loadBirthdays() {
  const result = await birthdayApi.searchBirthdays({
    filter: {},
    sort: { sortBy: 'dateOfBirth', direction: 1 },
  })
  if (result) {
    birthdays.value = result
  }
}

function openBirthdayModal(dayBirthdays: BirthdayResponse[]) {
  selectedBirthdays.value = dayBirthdays
  showBirthdayModal.value = true
}

function closeBirthdayModal() {
  showBirthdayModal.value = false
  selectedBirthdays.value = []
}

onMounted(() => {
  theme.setTab('calendar')
  loadBirthdays()
})
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