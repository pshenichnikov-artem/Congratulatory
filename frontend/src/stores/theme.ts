import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useThemeStore = defineStore('theme', () => {
  const currentTab = ref('family')

  const themes = {
    family: {
      background: 'bg-gradient-to-br from-slate-900 via-violet-900 to-slate-900',
      navbar: 'bg-gradient-to-r from-slate-900 via-violet-900 to-slate-900',
      button: 'bg-gradient-to-r from-violet-600 to-fuchsia-600 hover:from-violet-700 hover:to-fuchsia-700',
      accent: 'text-violet-300 bg-violet-500/20',
      spinner: 'border-violet-400',
      solidBackground: 'bg-violet-800'
    },
    friends: {
      background: 'bg-gradient-to-br from-emerald-900 via-teal-900 to-cyan-900',
      navbar: 'bg-gradient-to-r from-emerald-900 via-teal-900 to-cyan-900',
      button: 'bg-gradient-to-r from-emerald-600 to-teal-600 hover:from-emerald-700 hover:to-teal-700',
      accent: 'text-emerald-300 bg-emerald-500/20',
      spinner: 'border-emerald-400',
      solidBackground: 'bg-emerald-800'
    },
    colleagues: {
      background: 'bg-gradient-to-br from-amber-900 via-orange-900 to-yellow-900',
      navbar: 'bg-gradient-to-r from-amber-900 via-orange-900 to-yellow-900',
      button: 'bg-gradient-to-r from-amber-600 to-orange-600 hover:from-amber-700 hover:to-orange-700',
      accent: 'text-amber-300 bg-amber-500/20',
      spinner: 'border-amber-400',
      solidBackground: 'bg-amber-800'
    },
    calendar: {
      background: 'bg-gradient-to-br from-blue-900 via-indigo-900 to-purple-900',
      navbar: 'bg-gradient-to-r from-blue-900 via-indigo-900 to-purple-900',
      button: 'bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700',
      accent: 'text-blue-300 bg-blue-500/20',
      spinner: 'border-blue-400',
      solidBackground: 'bg-blue-800'
    },
    profile: {
      background: 'bg-gradient-to-br from-gray-900 via-slate-900 to-zinc-900',
      navbar: 'bg-gradient-to-r from-gray-900 via-slate-900 to-zinc-900',
      button: 'bg-gradient-to-r from-gray-600 to-slate-600 hover:from-gray-700 hover:to-slate-700',
      accent: 'text-gray-300 bg-gray-500/20',
      spinner: 'border-gray-400',
      solidBackground: 'bg-gray-800'
    }
  }

  const currentTheme = computed(() => themes[currentTab.value as keyof typeof themes] || themes.family)

  function setTab(tab: string) {
    currentTab.value = tab
  }

  return {
    currentTab,
    setTab,
    background: computed(() => currentTheme.value.background),
    navbar: computed(() => currentTheme.value.navbar),
    button: computed(() => currentTheme.value.button),
    accent: computed(() => currentTheme.value.accent),
    spinner: computed(() => currentTheme.value.spinner),
    solidBackground: computed(() => currentTheme.value.solidBackground),
    activeTab: computed(() => {
      const colors = {
        family: 'border-violet-400 text-violet-300 bg-violet-500/20 shadow-lg shadow-violet-500/25',
        friends: 'border-emerald-400 text-emerald-300 bg-emerald-500/20 shadow-lg shadow-emerald-500/25',
        colleagues: 'border-amber-400 text-amber-300 bg-amber-500/20 shadow-lg shadow-amber-500/25',
        calendar: 'border-blue-400 text-blue-300 bg-blue-500/20 shadow-lg shadow-blue-500/25',
        profile: 'border-gray-400 text-gray-300 bg-gray-500/20 shadow-lg shadow-gray-500/25'
      }
      return colors[currentTab.value as keyof typeof colors] || colors.family
    })
  }
})