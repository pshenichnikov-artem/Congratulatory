<template>
  <nav class="shadow-lg" :class="theme.navbar">
    <div class="max-w-7xl mx-auto px-2 sm:px-4 lg:px-8">
      <div class="flex justify-between items-center h-14 sm:h-16">
        <!-- Mobile menu button -->
        <button
          @click="mobileMenuOpen = !mobileMenuOpen"
          class="sm:hidden p-2 rounded-md text-gray-300 hover:text-white hover:bg-white/10"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path v-if="!mobileMenuOpen" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
            <path v-else stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>

        <!-- Desktop navigation -->
        <div class="hidden sm:flex sm:space-x-4 lg:space-x-8">
          <button
            v-for="tab in mainTabs"
            :key="tab.key"
            @click="handleTabClick(tab)"
            :class="[
              'inline-flex items-center px-2 lg:px-4 py-2 border-b-2 text-xs lg:text-sm font-medium transition-all duration-200 rounded-t-lg',
              activeTab === tab.key
                ? theme.activeTab
                : 'border-transparent text-gray-300 hover:text-white hover:border-gray-300 hover:bg-white/5'
            ]"
          >
            <component :is="tab.icon" class="w-4 h-4 lg:w-5 lg:h-5 sm:mr-1 lg:mr-2" />
            <span class="hidden md:inline">{{ tab.label }}</span>
          </button>
        </div>
        
        <div class="flex items-center space-x-2 sm:space-x-4">
          <!-- Desktop right tabs -->
          <div class="hidden sm:flex sm:space-x-4 lg:space-x-8">
            <button
              v-for="tab in rightTabs"
              :key="tab.key"
              @click="handleTabClick(tab)"
              :class="[
                'inline-flex items-center px-2 lg:px-4 py-2 border-b-2 text-xs lg:text-sm font-medium transition-all duration-200 rounded-t-lg',
                activeTab === tab.key
                  ? theme.activeTab
                  : 'border-transparent text-gray-300 hover:text-white hover:border-gray-300 hover:bg-white/5'
              ]"
            >
              <component :is="tab.icon" class="w-4 h-4 lg:w-5 lg:h-5 sm:mr-1 lg:mr-2" />
              <span class="hidden md:inline">{{ tab.label }}</span>
            </button>
          </div>
          
          <!-- Logout button -->
          <button
            @click="logout"
            class="inline-flex items-center px-2 sm:px-4 py-1 sm:py-2 border border-transparent text-xs sm:text-sm font-medium rounded-lg text-white bg-red-600 hover:bg-red-700 transition-colors duration-200"
          >
            <LogoutIcon class="w-4 h-4 sm:w-5 sm:h-5 sm:mr-2" />
            <span class="hidden sm:inline">Выйти</span>
          </button>
        </div>
      </div>

      <!-- Mobile menu -->
      <div v-if="mobileMenuOpen" class="sm:hidden border-t border-white/10 mt-2 pt-2 pb-3">
        <div class="space-y-1">
          <button
            v-for="tab in [...mainTabs, ...rightTabs]"
            :key="tab.key"
            @click="handleTabClick(tab); mobileMenuOpen = false"
            :class="[
              'flex items-center w-full px-3 py-2 text-sm font-medium rounded-lg transition-all duration-200',
              activeTab === tab.key
                ? 'bg-white/20 text-white'
                : 'text-gray-300 hover:text-white hover:bg-white/10'
            ]"
          >
            <component :is="tab.icon" class="w-5 h-5 mr-3" />
            {{ tab.label }}
          </button>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useThemeStore } from '@/stores/theme'

const router = useRouter()
const authStore = useAuthStore()
const theme = useThemeStore()
const mobileMenuOpen = ref(false)

const activeTab = computed(() => theme.currentTab)

const emit = defineEmits<{
  tabChange: [tab: string]
}>()

const mainTabs = [
  { key: 'family', label: 'Семья', icon: 'FamilyIcon' },
  { key: 'friends', label: 'Друзья', icon: 'FriendsIcon' },
  { key: 'colleagues', label: 'Коллеги', icon: 'ColleaguesIcon' },
  { key: 'calendar', label: 'Календарь', icon: 'CalendarIcon' }
]

const rightTabs = [
  { key: 'profile', label: 'Профиль', icon: 'ProfileIcon' }
]

function handleTabClick(tab: any) {
  theme.setTab(tab.key)
  
  if (tab.key === 'profile') {
    router.push('/profile')
  } else if (tab.key === 'calendar') {
    router.push('/calendar')
  } else if (['family', 'friends', 'colleagues'].includes(tab.key)) {
    router.push(`/${tab.key}`)
  }
}

function logout() {
  authStore.clearToken()
  router.push('/login')
}
</script>

<script lang="ts">
import { FamilyIcon, FriendsIcon, ColleaguesIcon, CalendarIcon, ProfileIcon, LogoutIcon } from '@/components/icons'

export default {
  components: {
    FamilyIcon,
    FriendsIcon,
    ColleaguesIcon,
    CalendarIcon,
    ProfileIcon,
    LogoutIcon
  }
}
</script>