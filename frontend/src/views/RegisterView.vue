<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-emerald-900 via-teal-900 to-cyan-900 px-4 py-8 relative overflow-hidden">
    <!-- Animated background elements -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="absolute -top-40 -right-40 w-80 h-80 bg-emerald-500 rounded-full mix-blend-multiply filter blur-xl opacity-70 animate-blob"></div>
      <div class="absolute -bottom-40 -left-40 w-80 h-80 bg-teal-500 rounded-full mix-blend-multiply filter blur-xl opacity-70 animate-blob animation-delay-2000"></div>
      <div class="absolute top-40 left-40 w-80 h-80 bg-cyan-500 rounded-full mix-blend-multiply filter blur-xl opacity-70 animate-blob animation-delay-4000"></div>
    </div>
    
    <div class="max-w-sm w-full space-y-8 relative z-10">
      <div class="bg-white/10 backdrop-blur-lg rounded-3xl shadow-2xl p-6 border border-white/20">
        <div class="text-center mb-8">
          <div class="mx-auto w-16 h-16 bg-gradient-to-r from-emerald-500 to-teal-500 rounded-2xl flex items-center justify-center mb-4">
            <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z"></path>
            </svg>
          </div>
          <h2 class="text-3xl font-bold text-white mb-2">Создать аккаунт</h2>
          <p class="text-gray-300">Присоединяйтесь к нам сегодня</p>
        </div>

        <form @submit.prevent="handleRegister" class="space-y-5">
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

          <ValidationInput
            id="confirmPassword"
            v-model="confirmPassword"
            label="Подтверждение пароля"
            type="password"
            placeholder="••••••••"
            validation-rules="required|match"
            :compare-with="form.password"
            :trigger-validation="validationTrigger"
            @valid="updateValidation('confirmPassword', $event)"
            background-color="bg-white/10"
            title-color="text-white"
            text-color="text-white"
            border-color="border-white/20"
          />

          <button
            type="submit"
            :disabled="!isFormValid || loading.register"
            class="w-full group relative flex justify-center py-4 px-4 border border-transparent rounded-xl text-sm font-medium text-white bg-gradient-to-r from-emerald-600 to-teal-600 hover:from-emerald-700 hover:to-teal-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-emerald-500 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 transform hover:scale-105 shadow-lg hover:shadow-xl"
          >
            <span v-if="!loading.register" class="flex items-center">
              <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
              </svg>
              Создать аккаунт
            </span>
            <div v-else class="flex items-center">
              <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Создание...
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
              Уже есть аккаунт?
              <router-link to="/login" class="font-medium text-emerald-400 hover:text-emerald-300 transition-colors ml-1">
                Войти
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
import type { RegisterRequest } from '@/types/user/RegisterRequest'

const router = useRouter()
const userApi = useUserApi()
const authStore = useAuthStore()

const form = ref<RegisterRequest>({
  email: '',
  password: ''
})

const confirmPassword = ref('')
const validationTrigger = ref(0)
const fieldValidation = ref<Record<string, boolean>>({
  email: false,
  password: false,
  confirmPassword: false
})

const isFormValid = computed(() => 
  Object.values(fieldValidation.value).every(valid => valid)
)

const { loading } = userApi

function updateValidation(field: string, isValid: boolean) {
  fieldValidation.value[field] = isValid
}

async function handleRegister() {
  validationTrigger.value++
  
  if (!isFormValid.value) return

  const result = await userApi.register(form.value, {
    showSuccessNotification: true,
    successMessage: 'Регистрация прошла успешно'
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