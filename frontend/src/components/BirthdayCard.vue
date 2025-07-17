<template>
  <div class="bg-white/5 backdrop-blur-sm rounded-xl sm:rounded-2xl p-3 sm:p-4 border border-white/10 hover:bg-white/10 transition-all duration-200">
    <div class="flex flex-col sm:flex-row sm:items-center space-y-3 sm:space-y-0 sm:space-x-4">
      <!-- Avatar -->
      <div class="flex items-center space-x-3 sm:space-x-0">
        <div class="flex-shrink-0">
          <img 
            v-if="birthday.photoPath" 
            :src="birthday.photoPath" 
            :alt="birthday.fullName"
            class="w-12 h-12 sm:w-16 sm:h-16 rounded-full object-cover border-2 border-white/20"
          />
          <div 
            v-else 
            class="w-12 h-12 sm:w-16 sm:h-16 rounded-full bg-gradient-to-br from-blue-400 to-purple-500 flex items-center justify-center text-white font-bold text-lg sm:text-xl"
          >
            {{ birthday.fullName.charAt(0).toUpperCase() }}
          </div>
        </div>
        
        <!-- Mobile: Name and actions in same row -->
        <div class="flex-1 sm:hidden min-w-0">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold text-white truncate pr-2 min-w-0 flex-1">{{ birthday.fullName }}</h3>
            <div class="flex space-x-2 flex-shrink-0">
              <button
                @click="editBirthday"
                class="p-1 text-gray-300 hover:text-white transition-colors"
              >
                <EditIcon class="w-4 h-4" />
              </button>
              <button
                @click="deleteBirthday"
                class="p-1 text-gray-300 hover:text-red-400 transition-colors"
              >
                <DeleteIcon class="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Desktop Content -->
      <div class="hidden sm:block flex-1 min-w-0">
        <div class="flex items-center justify-between mb-2">
          <h3 class="text-lg font-semibold text-white truncate">{{ birthday.fullName }}</h3>
          <div class="flex space-x-2">
            <button
              @click="editBirthday"
              class="p-1 text-gray-300 hover:text-white transition-colors"
            >
              <EditIcon class="w-4 h-4" />
            </button>
            <button
              @click="deleteBirthday"
              class="p-1 text-gray-300 hover:text-red-400 transition-colors"
            >
              <DeleteIcon class="w-4 h-4" />
            </button>
          </div>
        </div>
        
        <div class="flex items-center justify-between">
          <div class="flex items-center space-x-4">
            <span class="text-lg font-bold text-white bg-white/10 px-3 py-1 rounded-lg">
              {{ formatDate(birthday.dateOfBirth) }}
            </span>
            <span class="text-sm font-semibold px-2 py-1 rounded-lg" :class="getDaysColor(birthday.dateOfBirth)">
              {{ getDaysUntilBirthday(birthday.dateOfBirth) }}
            </span>
          </div>
          
          <!-- Social Network Reminder -->
          <div v-if="hasVerifiedAccounts()" class="flex items-center space-x-2">
            <span class="text-xs text-gray-300">Напомнить:</span>
            <div class="flex items-center space-x-2">
              <div v-if="hasTelegram()" class="flex items-center space-x-1">
                <button 
                  @click="openTelegramDatePicker"
                  class="px-2 py-1 text-xs rounded bg-blue-600 hover:bg-blue-700 text-white transition-colors"
                >
                  Telegram
                </button>
                <span v-if="getTelegramNotification()" class="text-xs text-gray-300">
                  {{ formatNotificationDate(getTelegramNotification()?.notificationDate) }}
                </span>
                <button 
                  v-if="getTelegramNotification()"
                  @click="deleteTelegramNotification"
                  class="text-xs text-red-400 hover:text-red-300"
                >
                  ✕
                </button>
              </div>
              <div v-if="hasVk()" class="flex items-center space-x-1">
                <button 
                  @click="openVkDatePicker"
                  class="px-2 py-1 text-xs rounded bg-blue-500 hover:bg-blue-600 text-white transition-colors"
                >
                  ВК
                </button>
                <span v-if="getVkNotification()" class="text-xs text-gray-300">
                  {{ formatNotificationDate(getVkNotification()?.notificationDate) }}
                </span>
                <button 
                  v-if="getVkNotification()"
                  @click="deleteVkNotification"
                  class="text-xs text-red-400 hover:text-red-300"
                >
                  ✕
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Mobile Content -->
      <div class="sm:hidden w-full space-y-3">
        <div class="flex flex-wrap items-center gap-2">
          <span class="text-base font-bold text-white bg-white/10 px-3 py-1 rounded-lg">
            {{ formatDate(birthday.dateOfBirth) }}
          </span>
          <span class="text-sm font-semibold px-2 py-1 rounded-lg" :class="getDaysColor(birthday.dateOfBirth)">
            {{ getDaysUntilBirthday(birthday.dateOfBirth) }}
          </span>
        </div>
        
        <!-- Mobile Social Network Reminder -->
        <div v-if="hasVerifiedAccounts()" class="space-y-2">
          <span class="text-xs text-gray-300">Напомнить:</span>
          <div class="flex flex-wrap gap-2">
            <div v-if="hasTelegram()" class="flex items-center space-x-1">
              <button 
                @click="openTelegramDatePicker"
                class="px-2 py-1 text-xs rounded bg-blue-600 hover:bg-blue-700 text-white transition-colors"
              >
                Telegram
              </button>
              <span v-if="getTelegramNotification()" class="text-xs text-gray-300">
                {{ formatNotificationDate(getTelegramNotification()?.notificationDate) }}
              </span>
              <button 
                v-if="getTelegramNotification()"
                @click="deleteTelegramNotification"
                class="text-xs text-red-400 hover:text-red-300"
              >
                ✕
              </button>
            </div>
            <div v-if="hasVk()" class="flex items-center space-x-1">
              <button 
                @click="openVkDatePicker"
                class="px-2 py-1 text-xs rounded bg-blue-500 hover:bg-blue-600 text-white transition-colors"
              >
                ВК
              </button>
              <span v-if="getVkNotification()" class="text-xs text-gray-300">
                {{ formatNotificationDate(getVkNotification()?.notificationDate) }}
              </span>
              <button 
                v-if="getVkNotification()"
                @click="deleteVkNotification"
                class="text-xs text-red-400 hover:text-red-300"
              >
                ✕
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Delete Confirmation Modal -->
    <ConfirmModal
      :show="showDeleteConfirm"
      title="Удалить день рождения"
      :message="`Вы уверены, что хотите удалить день рождения ${props.birthday.fullName}?`"
      confirm-text="Удалить"
      loading-text="Удаление..."
      :loading="loading.delete"
      @confirm="confirmDelete"
      @cancel="cancelDelete"
    />
    
    <!-- Date Picker Modal -->
    <DatePickerModal
      :show="showDatePicker"
      :current-date="getCurrentNotificationDate()"
      @confirm="confirmNotificationDate"
      @cancel="closeDatePicker"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import type { BirthdayResponse } from '@/types/birthday/BirthdayResponse'
import type { UserResponse } from '@/types/user/UserResponse'
import { EditIcon, DeleteIcon } from '@/components/icons'
import ConfirmModal from '@/components/ConfirmModal.vue'
import DatePickerModal from '@/components/DatePickerModal.vue'
import { useThemeStore } from '@/stores/theme'
import { useBirthdayApi } from '@/composables/useBirthdayApi'
import { useUserApi } from '@/composables/useUserApi'
import type { BirthdayNotificationResponse } from '@/types/birthdayNotification/BirthdayNotificationResponse'

const props = defineProps<{
  birthday: BirthdayResponse
}>()

const emit = defineEmits<{
  refresh: []
  edit: [birthday: BirthdayResponse]
}>()

const theme = useThemeStore()
const birthdayApi = useBirthdayApi()
const { loading } = birthdayApi
const userApi = useUserApi()
const userInfo = ref<UserResponse | null>(null)
const showDatePicker = ref(false)
const selectedPlatform = ref('')
const notifications = ref<BirthdayNotificationResponse[]>([])

function formatDate(dateString: string) {
  return new Date(dateString).toLocaleDateString('ru-RU', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  })
}

function getDaysUntilBirthday(dateString: string) {
  const today = new Date()
  const birthday = new Date(dateString)
  birthday.setFullYear(today.getFullYear())
  
  if (birthday < today) {
    birthday.setFullYear(today.getFullYear() + 1)
  }
  
  const diffTime = birthday.getTime() - today.getTime()
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
  
  if (diffDays === 0) return 'Сегодня!'
  if (diffDays === 1) return 'Завтра'
  return `Через ${diffDays} дней`
}

function getDaysColor(dateString: string) {
  const today = new Date()
  const birthday = new Date(dateString)
  birthday.setFullYear(today.getFullYear())
  
  if (birthday < today) {
    birthday.setFullYear(today.getFullYear() + 1)
  }
  
  const diffTime = birthday.getTime() - today.getTime()
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
  
  if (diffDays === 0) return 'bg-red-500 text-white'
  if (diffDays <= 7) return 'bg-yellow-500 text-black'
  return 'bg-green-500 text-white'
}

async function loadUserInfo() {
  const result = await userApi.getCurrentUser()
  if (result) {
    userInfo.value = result
  }
}

async function loadNotifications() {
  const result = await birthdayApi.getBirthdayNotifications(props.birthday.id)
  if (result) {
    notifications.value = result
  }
}

function getTelegramAccount() {
  return userInfo.value?.userAccounts?.find(account => account.platform === 'telegram')
}

function getVkAccount() {
  return userInfo.value?.userAccounts?.find(account => account.platform === 'vk')
}

function hasVerifiedAccounts() {
  return hasTelegram() || hasVk()
}

function hasTelegram() {
  const account = getTelegramAccount()
  return account && account.isVerified
}

function hasVk() {
  const account = getVkAccount()
  return account && account.isVerified
}

onMounted(() => {
  loadUserInfo()
  loadNotifications()
})

function editBirthday() {
  emit('edit', props.birthday)
}

const showDeleteConfirm = ref(false)

function deleteBirthday() {
  showDeleteConfirm.value = true
}

async function confirmDelete() {
  const result = await birthdayApi.deleteBirthday(props.birthday.id, {
    showSuccessNotification: true,
    successMessage: 'День рождения успешно удален'
  })
  
  if (result) {
    emit('refresh')
    showDeleteConfirm.value = false
  }
}

function cancelDelete() {
  showDeleteConfirm.value = false
}

function openTelegramDatePicker() {
  selectedPlatform.value = 'telegram'
  showDatePicker.value = true
}

function openVkDatePicker() {
  selectedPlatform.value = 'vk'
  showDatePicker.value = true
}

function closeDatePicker() {
  showDatePicker.value = false
  selectedPlatform.value = ''
}

function getTelegramNotification() {
  return notifications.value.find(n => n.userAccount.platform === "telegram") ?? null
}

function getVkNotification() {
  return notifications.value.find(n => n.userAccount.platform === "vk") ?? null
}

function formatNotificationDate(dateString: string | null | undefined) {
  if (!dateString) return ''

  const [datePart, timePart] = dateString.split(' ')
  const [month, day] = datePart.split('-')
  return `${day}.${month} ${timePart || ''}`
}

async function confirmNotificationDate(date: string) {
  const account = selectedPlatform.value === 'telegram' ? getTelegramAccount() : getVkAccount()
  if (!account) return
  
  const result = await birthdayApi.upsertBirthdayNotification(props.birthday.id, {
    userAccountId: account.id,
    notificationDate: date
  }, {
    showSuccessNotification: true,
    successMessage: 'Напоминание установлено'
  })
  
  if (result) {
    await loadNotifications()
    closeDatePicker()
  }
}

async function deleteTelegramNotification() {
  const notification = getTelegramNotification()
  if (!notification) return
  
  const result = await birthdayApi.deleteBirthdayNotification(props.birthday.id, notification.id, {
    showSuccessNotification: true,
    successMessage: 'Напоминание удалено'
  })
  
  if (result) {
    await loadNotifications()
  }
}

async function deleteVkNotification() {
  const notification = getVkNotification()
  if (!notification) return
  
  const result = await birthdayApi.deleteBirthdayNotification(props.birthday.id, notification.id, {
    showSuccessNotification: true,
    successMessage: 'Напоминание удалено'
  })
  
  if (result) {
    await loadNotifications()
  }
}

function getCurrentNotificationDate() {
  if (selectedPlatform.value === 'telegram') {
    return getTelegramNotification()?.notificationDate
  } else if (selectedPlatform.value === 'vk') {
    return getVkNotification()?.notificationDate
  }
  return undefined
}
</script>