<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-900 via-violet-900 to-slate-900 px-4 relative overflow-hidden">
    <!-- Animated background elements -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="absolute -top-40 -right-40 w-80 h-80 bg-violet-500 rounded-full mix-blend-multiply filter blur-xl opacity-70 animate-blob"></div>
      <div class="absolute -bottom-40 -left-40 w-80 h-80 bg-blue-500 rounded-full mix-blend-multiply filter blur-xl opacity-70 animate-blob animation-delay-2000"></div>
      <div class="absolute top-40 left-40 w-80 h-80 bg-fuchsia-500 rounded-full mix-blend-multiply filter blur-xl opacity-70 animate-blob animation-delay-4000"></div>
    </div>
    
    <div class="max-w-sm w-full space-y-8 relative z-10">
      <div class="bg-white/10 backdrop-blur-lg rounded-3xl shadow-2xl p-6 border border-white/20">
        <div class="text-center mb-8">
          <div class="mx-auto w-16 h-16 bg-gradient-to-r from-violet-500 to-fuchsia-500 rounded-2xl flex items-center justify-center mb-4">
            <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path>
            </svg>
          </div>
          <h2 class="text-3xl font-bold text-white mb-2">Добро пожаловать</h2>
          <p class="text-gray-300">Войдите в свой аккаунт</p>
        </div>

        <form @submit.prevent="handleLogin" class="space-y-6">
          <ValidationInput
            id="email"
            v-model="form.email"
            label="Email"
            type="email"
            placeholder="example@email.com"
            validation-rules="required|email"
            :trigger-validation="validationTrigger"
            @valid="updateValidation('email', $event)"
            background-color="bg-white/10"
            title-color="text-white"
            text-color="text-white"
            border-color="border-white/20"
          />

          <ValidationInput
            id="password"
            v-model="form.password"
            label="Пароль"
            type="password"
            placeholder="••••••••"
            validation-rules="required|password"
            :trigger-validation="validationTrigger"
            @valid="updateValidation('password', $event)"
            background-color="bg-white/10"
            title-color="text-white"
            text-color="text-white"
            border-color="border-white/20"
          />

          <button
            type="submit"
            :disabled="!isFormValid || loading.login"
            class="w-full group relative flex justify-center py-4 px-4 border border-transparent rounded-xl text-sm font-medium text-white bg-gradient-to-r from-violet-600 to-fuchsia-600 hover:from-violet-700 hover:to-fuchsia-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-violet-500 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 transform hover:scale-105 shadow-lg hover:shadow-xl"
          >
            <span v-if="!loading.login" class="flex items-center">
              <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 16l-4-4m0 0l4-4m-4 4h14m-5 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h7a3 3 0 013 3v1"></path>
              </svg>
              Войти
            </span>
            <div v-else class="flex items-center">
              <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Вход...
            </div>
          </button>
        </form>

        <div class="mt-8">
          <div class="relative">
            <div class="absolute inset-0 flex items-center">
              <div class="w-full border-t border-white/20"></div>
            </div>
            <div class="relative flex justify-center text-sm">
              <span class="px-2 bg-transparent text-gray-300">или</span>
            </div>
          </div>
          
          <div class="mt-6 text-center">
            <p class="text-gray-300">
              Нет аккаунта?
              <router-link to="/register" class="font-medium text-violet-400 hover:text-violet-300 transition-colors ml-1">
                Создать аккаунт
              </router-link>
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useUserApi } from '@/composables/useUserApi'
import { useAuthStore } from '@/stores/auth'
import ValidationInput from '@/components/ValidationInput.vue'
import type { LoginRequest } from '@/types/user/LoginRequest'

const router = useRouter()
const userApi = useUserApi()
const authStore = useAuthStore()

const form = ref<LoginRequest>({
  email: '',
  password: ''
})

const validationTrigger = ref(0)
const fieldValidation = ref<Record<string, boolean>>({
  email: false,
  password: false
})

const isFormValid = computed(() => 
  Object.values(fieldValidation.value).every(valid => valid)
)

const { loading } = userApi

function updateValidation(field: string, isValid: boolean) {
  fieldValidation.value[field] = isValid
}

async function handleLogin() {
  validationTrigger.value++
  console.error('VITE_API_URL =', import.meta.env.VITE_API_URL)
  if (!isFormValid.value) return
  
  const result = await userApi.login(form.value, {
    showSuccessNotification: true,
    successMessage: 'Успешный вход в систему'
  })

  if (result) {
    authStore.setToken(result.token)
    router.push('/')
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
