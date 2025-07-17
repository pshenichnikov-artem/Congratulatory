<template>
  <div class="min-h-screen relative overflow-hidden" :class="theme.background">
    <!-- Animated background elements -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="absolute -top-40 -right-40 w-80 h-80 bg-gray-500 rounded-full mix-blend-multiply filter blur-xl opacity-30 animate-blob"></div>
      <div class="absolute -bottom-40 -left-40 w-80 h-80 bg-slate-500 rounded-full mix-blend-multiply filter blur-xl opacity-30 animate-blob animation-delay-2000"></div>
      <div class="absolute top-40 left-40 w-80 h-80 bg-zinc-500 rounded-full mix-blend-multiply filter blur-xl opacity-30 animate-blob animation-delay-4000"></div>
    </div>

    <div class="relative z-10">
      
      <div class="max-w-4xl mx-auto px-2 sm:px-4 lg:px-8 py-4 sm:py-8">
        <div class="bg-white/10 backdrop-blur-lg rounded-2xl sm:rounded-3xl shadow-2xl p-3 sm:p-6 border border-white/20">
          <h1 class="text-2xl sm:text-3xl font-bold text-white mb-4 sm:mb-8">Профиль</h1>

          <!-- User Info -->
          <div v-if="loading.get" class="flex justify-center py-8">
            <div class="animate-spin rounded-full h-8 w-8 border-b-2" :class="theme.spinner"></div>
          </div>
          
          <div v-else-if="userInfo" class="bg-white/5 rounded-xl sm:rounded-2xl p-3 sm:p-6 mb-4 sm:mb-8">
            <h2 class="text-lg sm:text-xl font-semibold text-white mb-3 sm:mb-4">Информация о пользователе</h2>
            <div>
              <label class="block text-sm text-gray-300">Email</label>
              <p class="text-white font-medium">{{ userInfo.email }}</p>
            </div>
          </div>

          <!-- Change Password Form -->
          <div class="bg-white/5 rounded-xl sm:rounded-2xl p-3 sm:p-6 mb-4 sm:mb-8">
            <h2 class="text-lg sm:text-xl font-semibold text-white mb-3 sm:mb-4">Изменить пароль</h2>
            <form @submit.prevent="handleChangePassword" class="space-y-4">
              <ValidationInput
                id="oldPassword"
                v-model="passwordForm.currentPassword"
                label="Старый пароль"
                type="password"
                validation-rules="required"
                :trigger-validation="validationTrigger"
                @valid="updateValidation('oldPassword', $event)"
                background-color="bg-white/10"
                title-color="text-white"
                text-color="text-white"
                border-color="border-white/20"
              />

              <ValidationInput
                id="newPassword"
                v-model="passwordForm.newPassword"
                label="Новый пароль"
                type="password"
                validation-rules="required|password"
                :trigger-validation="validationTrigger"
                @valid="updateValidation('newPassword', $event)"
                background-color="bg-white/10"
                title-color="text-white"
                text-color="text-white"
                border-color="border-white/20"
              />

              <ValidationInput
                id="confirmPassword"
                v-model="confirmPassword"
                label="Подтверждение нового пароля"
                type="password"
                validation-rules="required|match"
                :compare-with="passwordForm.newPassword"
                :trigger-validation="validationTrigger"
                @valid="updateValidation('confirmPassword', $event)"
                background-color="bg-white/10"
                title-color="text-white"
                text-color="text-white"
                border-color="border-white/20"
              />

              <button
                type="submit"
                :disabled="!isPasswordFormValid || loading.changePassword"
                class="w-full sm:w-auto px-4 sm:px-6 py-2 text-white rounded-lg font-medium transition-all duration-200 transform hover:scale-105 shadow-lg disabled:opacity-50"
                :class="theme.button"
              >
                <span v-if="!loading.changePassword">Изменить пароль</span>
                <div v-else class="flex items-center">
                  <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                  Изменение...
                </div>
              </button>
            </form>
          </div>

          <!-- Social Networks -->
          <div class="bg-white/5 rounded-xl sm:rounded-2xl p-3 sm:p-6">
            <h2 class="text-lg sm:text-xl font-semibold text-white mb-3 sm:mb-4">Социальные сети</h2>
            
            <!-- Bot Links -->
            <div class="bg-blue-500/10 border border-blue-500/20 rounded-lg p-3 mb-4">
              <div class="flex items-start space-x-2">
                <svg class="w-5 h-5 text-blue-400 mt-0.5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
                <div class="flex-1">
                  <p class="text-sm text-blue-200 mb-2">Для получения уведомлений укажите аккаунт от соцсети и отправьте сообщение боту:</p>
                  <div class="space-y-2">
                    <div class="flex items-center space-x-2">
                      <TelegramIcon class="w-4 h-4 text-blue-400" />
                      <a href="https://t.me/Congratulatory32Bot" target="_blank" class="text-blue-300 hover:text-blue-200 text-sm underline">
                        @Congratulatory32Bot
                      </a>
                    </div>
                    <div class="flex items-center space-x-2">
                      <VkIcon class="w-4 h-4 text-blue-400" />
                      <a href="https://vk.com/club231455992" target="_blank" class="text-blue-300 hover:text-blue-200 text-sm underline">
                        vk.com/club231455992
                      </a>
                    </div>
                  </div>
                  <p class="text-xs text-blue-300 mt-2">Нажмите "Начать" в боте или пришлите в чат "обновить", чтобы подтвердить свой аккаунт</p>
                </div>
              </div>
            </div>
            <div class="space-y-4">
              <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between p-3 sm:p-4 bg-white/5 rounded-lg space-y-3 sm:space-y-0">
                <div class="flex items-center space-x-3 flex-1">
                  <div class="w-8 h-8 bg-blue-500 rounded-full flex items-center justify-center">
                    <TelegramIcon class="w-4 h-4 text-white" />
                  </div>
                  <div class="flex-1">
                    <p class="text-white font-medium">Telegram</p>
                    <p class="text-sm text-gray-300">
                      {{ getTelegramAccount() ? `@${getTelegramAccount()?.userName}` : 'Не подключен' }}
                      <span v-if="getTelegramAccount()?.isVerified" class="text-green-400 ml-2">✓ Подтвержден</span>
                      <span v-else-if="getTelegramAccount()" class="text-yellow-400 ml-2">⚠ Не подтвержден</span>
                    </p>
                  </div>
                </div>
                <div class="flex space-x-2 w-full sm:w-auto">
                  <div v-if="getTelegramAccount()" class="flex space-x-2 w-full sm:w-auto">
                    <button
                      @click="editTelegramAccount"
                      class="flex-1 sm:flex-none px-3 py-1 text-sm rounded-lg transition-colors"
                      :class="theme.button"
                    >
                      Изменить
                    </button>
                    <button
                      @click="deleteTelegramAccount"
                      class="flex-1 sm:flex-none px-3 py-1 text-sm rounded-lg transition-colors bg-red-600 hover:bg-red-700"
                    >
                      Удалить
                    </button>
                  </div>
                  <button
                    v-else
                    @click="editTelegramAccount"
                    class="w-full sm:w-auto px-3 py-1 text-sm rounded-lg transition-colors"
                    :class="theme.button"
                  >
                    Добавить
                  </button>
                </div>
              </div>

              <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between p-3 sm:p-4 bg-white/5 rounded-lg space-y-3 sm:space-y-0">
                <div class="flex items-center space-x-3 flex-1">
                  <div class="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center">
                    <VkIcon class="w-4 h-4 text-white" />
                  </div>
                  <div class="flex-1">
                    <p class="text-white font-medium">ВКонтакте</p>
                    <p class="text-sm text-gray-300">
                      {{ getVkAccount() ? getVkAccount()?.userName : 'Не подключен' }}
                      <span v-if="getVkAccount()?.isVerified" class="text-green-400 ml-2">✓ Подтвержден</span>
                      <span v-else-if="getVkAccount()" class="text-yellow-400 ml-2">⚠ Не подтвержден</span>
                    </p>
                  </div>
                </div>
                <div class="flex space-x-2 w-full sm:w-auto">
                  <div v-if="getVkAccount()" class="flex space-x-2 w-full sm:w-auto">
                    <button
                      @click="editVkAccount"
                      class="flex-1 sm:flex-none px-3 py-1 text-sm rounded-lg transition-colors"
                      :class="theme.button"
                    >
                      Изменить
                    </button>
                    <button
                      @click="deleteVkAccount"
                      class="flex-1 sm:flex-none px-3 py-1 text-sm rounded-lg transition-colors bg-red-600 hover:bg-red-700"
                    >
                      Удалить
                    </button>
                  </div>
                  <button
                    v-else
                    @click="editVkAccount"
                    class="w-full sm:w-auto px-3 py-1 text-sm rounded-lg transition-colors"
                    :class="theme.button"
                  >
                    Добавить
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Account Edit Overlay -->
    <div v-if="showAccountEdit" class="fixed inset-0 z-50 bg-black/50 backdrop-blur-sm flex items-center justify-center">
      <div class="bg-white/10 backdrop-blur-lg rounded-2xl p-6 border border-white/20 w-full max-w-md mx-4">
        <h3 class="text-xl font-semibold text-white mb-4">
          {{ editingPlatform === 'telegram' ? 'Telegram' : 'ВКонтакте' }} аккаунт
        </h3>
        
        <div class="mb-6">
          <label class="block text-sm font-medium text-white mb-2">Никнейм</label>
          <input
            v-model="accountUserName"
            type="text"
            :placeholder="'username'"
            class="w-full px-4 py-2 bg-white/10 border border-white/20 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-white/30"
          />
        </div>
        
        <div class="flex space-x-3">
          <button
            @click="cancelEdit"
            class="flex-1 px-4 py-2 bg-white/10 text-white rounded-lg hover:bg-white/20 transition-colors"
          >
            Отмена
          </button>
          <button
            @click="saveAccount"
            :disabled="!accountUserName.trim() || loading.accounts"
            class="flex-1 px-4 py-2 text-white rounded-lg font-medium transition-all duration-200 disabled:opacity-50"
            :class="theme.button"
          >
            <span v-if="!loading.accounts">Сохранить</span>
            <div v-else class="flex items-center justify-center">
              <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
              Сохранение...
            </div>
          </button>
        </div>
      </div>
    </div>
    
    <!-- Delete Confirmation Modal -->
    <ConfirmModal
      :show="showDeleteConfirm"
      :title="`Удалить ${deletingAccount?.platform === 'telegram' ? 'Telegram' : 'ВКонтакте'} аккаунт`"
      message="Вы уверены, что хотите удалить этот аккаунт? Это действие нельзя отменить."
      confirm-text="Удалить"
      loading-text="Удаление..."
      :loading="loading.accounts"
      :button-class="theme.button"
      @confirm="confirmDelete"
      @cancel="cancelDelete"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'

import ValidationInput from '@/components/ValidationInput.vue'
import ConfirmModal from '@/components/ConfirmModal.vue'
import { useUserApi } from '@/composables/useUserApi'
import { useThemeStore } from '@/stores/theme'
import type { UserResponse } from '@/types/user/UserResponse'
import type { ChangePasswordRequest } from '@/types/user/ChangePasswordRequest'

const router = useRouter()
const userApi = useUserApi()
const theme = useThemeStore()
const { loading } = userApi

const userInfo = ref<UserResponse | null>(null)
const passwordForm = ref<ChangePasswordRequest>({
  currentPassword: '',
  newPassword: ''
})
const confirmPassword = ref('')
const validationTrigger = ref(0)
const fieldValidation = ref<Record<string, boolean>>({
  oldPassword: false,
  newPassword: false,
  confirmPassword: false
})
const showAccountEdit = ref(false)
const editingPlatform = ref('')
const accountUserName = ref('')
const showDeleteConfirm = ref(false)
const deletingAccount = ref<{ platform: string; id: number } | null>(null)

const isPasswordFormValid = computed(() => 
  Object.values(fieldValidation.value).every(valid => valid)
)

function updateValidation(field: string, isValid: boolean) {
  fieldValidation.value[field] = isValid
}

async function handleChangePassword() {
  validationTrigger.value++
  
  if (!isPasswordFormValid.value) return

  const result = await userApi.changePassword(passwordForm.value, {
    showSuccessNotification: true,
    successMessage: 'Пароль успешно изменен'
  })

  if (result) {
    passwordForm.value = {
      currentPassword: '',
      newPassword: ''
    }
    confirmPassword.value = ''
    fieldValidation.value = {
      oldPassword: false,
      newPassword: false,
      confirmPassword: false
    }
  }
}

async function loadUserInfo() {
  const result = await userApi.getCurrentUser()
  if (result) {
    userInfo.value = result
  }
}

function getTelegramAccount() {
  return userInfo.value?.userAccounts?.find(account => account.platform === 'telegram')
}

function getVkAccount() {
  return userInfo.value?.userAccounts?.find(account => account.platform === 'vk')
}

function editTelegramAccount() {
  editingPlatform.value = 'telegram'
  accountUserName.value = getTelegramAccount()?.userName || ''
  showAccountEdit.value = true
}

function editVkAccount() {
  editingPlatform.value = 'vk'
  accountUserName.value = getVkAccount()?.userName || ''
  showAccountEdit.value = true
}

function cancelEdit() {
  showAccountEdit.value = false
  editingPlatform.value = ''
  accountUserName.value = ''
}

async function saveAccount() {
  if (!accountUserName.value.trim()) return
  
  const result = await userApi.upsertUserAccount({
    platform: editingPlatform.value,
    userName: accountUserName.value.trim()
  }, {
    showSuccessNotification: true,
    successMessage: 'Аккаунт успешно сохранен'
  })
  
  if (result) {
    await loadUserInfo()
    cancelEdit()
  }
}

function deleteTelegramAccount() {
  const account = getTelegramAccount()
  if (account) {
    deletingAccount.value = { platform: 'telegram', id: account.id }
    showDeleteConfirm.value = true
  }
}

function deleteVkAccount() {
  const account = getVkAccount()
  if (account) {
    deletingAccount.value = { platform: 'vk', id: account.id }
    showDeleteConfirm.value = true
  }
}

async function confirmDelete() {
  if (!deletingAccount.value) return
  
  const result = await userApi.deleteUserAccount(deletingAccount.value.id, {
    showSuccessNotification: true,
    successMessage: 'Аккаунт успешно удален'
  })
  
  if (result) {
    await loadUserInfo()
    showDeleteConfirm.value = false
    deletingAccount.value = null
  }
}

function cancelDelete() {
  showDeleteConfirm.value = false
  deletingAccount.value = null
}

function handleTabChange() {
  // Navigation is handled by Navbar
}

onMounted(() => {
  theme.setTab('profile')
  loadUserInfo()
})
</script>

<script lang="ts">
import { TelegramIcon, VkIcon } from '@/components/icons'

export default {
  components: {
    TelegramIcon,
    VkIcon
  }
}
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