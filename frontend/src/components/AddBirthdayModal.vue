<template>
  <Teleport to="body">
    <div v-if="show" class="fixed inset-0 z-[9999] bg-black/80  overflow-y-auto">
      <div class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        <div class="fixed inset-0 transition-opacity"></div>
      
      <div class="inline-block w-full max-w-md p-6 my-8 overflow-hidden text-left align-middle transition-all transform backdrop-blur-lg shadow-xl rounded-3xl border" :class="[theme.accent, 'border-white/20']">
        <div class="flex justify-between items-center mb-6">
          <h3 class="text-2xl font-bold text-white">{{ props.editingBirthday ? 'Редактировать' : 'Добавить' }} день рождения</h3>
          <button @click="$emit('close')" class="text-gray-300 hover:text-white">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>

        <form @submit.prevent="handleSubmit" class="space-y-4">
          <ValidationInput
            id="fullName"
            v-model="form.fullName"
            label="Полное имя"
            type="text"
            placeholder="Иван Иванов"
            validation-rules="required|minLength:3"
            :trigger-validation="validationTrigger"
            @valid="updateValidation('fullName', $event)"
            background-color="bg-white/10"
            title-color="text-white"
            text-color="text-white"
            border-color="border-white/20"
          />

          <div class="space-y-2">
            <label class="block text-sm font-medium text-white">Дата рождения</label>
            <input
              v-model="form.dateOfBirth"
              type="date"
              :max="getYesterday()"
              min="1900-01-01"
              class="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-white/30"
              required
            />
          </div>

          <div class="space-y-2">
            <label class="block text-sm font-medium text-white">Фото</label>
            <div 
              @drop="handleDrop"
              @dragover.prevent
              @dragenter.prevent
              :class="[
                'border-2 border-dashed rounded-lg p-6 text-center transition-colors',
                isDragging ? 'border-white bg-white/10' : 'border-white/30 hover:border-white/50'
              ]"
            >
              <input
                type="file"
                accept="image/*"
                @change="handleFileChange"
                class="hidden"
                ref="fileInput"
              />
              <div v-if="!selectedFile && !currentPhoto">
                <svg class="mx-auto h-12 w-12 text-gray-400 mb-4" stroke="currentColor" fill="none" viewBox="0 0 48 48">
                  <path d="M28 8H12a4 4 0 00-4 4v20m32-12v8m0 0v8a4 4 0 01-4 4H12a4 4 0 01-4-4v-4m32-4l-3.172-3.172a4 4 0 00-5.656 0L28 28M8 32l9.172-9.172a4 4 0 015.656 0L28 28m0 0l4 4m4-24h8m-4-4v8m-12 4h.02" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                </svg>
                <p class="text-white mb-2">Перетащите фото сюда или</p>
                <button
                  type="button"
                  @click="fileInput?.click()"
                  class="px-4 py-2 text-white rounded-lg transition-colors"
                  :class="theme.button"
                >
                  Выбрать файл
                </button>
              </div>
              <div v-else-if="selectedFile" class="flex items-center justify-between">
                <span class="text-sm text-white">{{ selectedFile.name }}</span>
                <button
                  type="button"
                  @click="removeFile"
                  class="text-red-400 hover:text-red-300"
                >
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                  </svg>
                </button>
              </div>
              <div v-else class="flex flex-col items-center space-y-3">
                <img 
                  :src="currentPhoto || ''"
                  alt="Текущее фото"
                  class="w-24 h-24 rounded-full object-cover border-2 border-white/20"
                />
                <div class="flex space-x-2">
                  <button
                    type="button"
                    @click="fileInput?.click()"
                    class="px-3 py-1 text-sm text-white rounded-lg transition-colors"
                    :class="theme.button"
                  >
                    Изменить
                  </button>
                  <button
                    type="button"
                    @click="removeCurrentPhoto"
                    class="px-3 py-1 text-sm bg-red-600 hover:bg-red-700 text-white rounded-lg transition-colors"
                  >
                    Удалить
                  </button>
                </div>
              </div>
            </div>
          </div>

          <div class="flex justify-end space-x-3 pt-4">
            <button
              type="button"
              @click="$emit('close')"
              class="px-4 py-2 text-gray-300 bg-white/10 rounded-lg hover:bg-white/20 transition-colors"
            >
              Отмена
            </button>
            <button
              type="submit"
              :disabled="!isFormValid || loading.create"
              class="px-4 py-2 text-white rounded-lg font-medium transition-all duration-200 transform hover:scale-105 shadow-lg disabled:opacity-50"
              :class="theme.button"
            >
              <span v-if="!loading.create">{{ props.editingBirthday ? 'Сохранить' : 'Добавить' }}</span>
              <div v-else class="flex items-center">
                <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                Добавление...
              </div>
            </button>
          </div>
        </form>
      </div>
    </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import ValidationInput from '@/components/ValidationInput.vue'
import { useBirthdayApi } from '@/composables/useBirthdayApi'
import { useThemeStore } from '@/stores/theme'
import type { BirthdayCreateRequest } from '@/types/birthday/BirthdayCreateRequest'
import type { BirthdayResponse } from '@/types/birthday/BirthdayResponse'

const props = defineProps<{
  show: boolean
  relationshipType: string
  editingBirthday?: BirthdayResponse | null
}>()

const emit = defineEmits<{
  close: []
  success: []
}>()

const birthdayApi = useBirthdayApi()
const theme = useThemeStore()
const { loading } = birthdayApi

const form = ref<BirthdayCreateRequest>({
  fullName: '',
  dateOfBirth: '',
  relationshipType: props.relationshipType
})

const selectedFile = ref<File | null>(null)
const fileInput = ref<HTMLInputElement | null>(null)
const isDragging = ref(false)
const currentPhoto = ref<string | null>(null)
const photoToDelete = ref(false)

const validationTrigger = ref(0)
const fieldValidation = ref<Record<string, boolean>>({
  fullName: false,
  dateOfBirth: false
})

const isFormValid = computed((): boolean => 
  form.value.fullName.trim().length >= 3 && form.value.dateOfBirth !== ''
)

function getYesterday() {
  const yesterday = new Date()
  yesterday.setDate(yesterday.getDate() - 1)
  return yesterday.toISOString().split('T')[0]
}

watch(() => props.relationshipType, (newType) => {
  form.value.relationshipType = newType
})

watch(() => props.editingBirthday, (birthday) => {
  if (birthday) {
    form.value = {
      fullName: birthday.fullName,
      dateOfBirth: birthday.dateOfBirth.split('T')[0],
      relationshipType: birthday.relationshipType
    }
    currentPhoto.value = birthday.photoPath || null
    selectedFile.value = null
    photoToDelete.value = false

    fieldValidation.value = {
      fullName: true,
      dateOfBirth: true
    }
  } else {
    form.value = {
      fullName: '',
      dateOfBirth: '',
      relationshipType: props.relationshipType
    }
    currentPhoto.value = null
    selectedFile.value = null
    photoToDelete.value = false
    fieldValidation.value = {
      fullName: false,
      dateOfBirth: false
    }
  }
})

function updateValidation(field: string, isValid: boolean) {
  fieldValidation.value[field] = isValid
}

function handleFileChange(event: Event) {
  const target = event.target as HTMLInputElement
  if (target.files && target.files[0]) {
    selectedFile.value = target.files[0]
  }
}

function handleDrop(event: DragEvent) {
  event.preventDefault()
  isDragging.value = false
  
  if (event.dataTransfer?.files && event.dataTransfer.files[0]) {
    selectedFile.value = event.dataTransfer.files[0]
  }
}

function removeFile() {
  selectedFile.value = null
  if (fileInput.value) {
    fileInput.value.value = ''
  }
}

function removeCurrentPhoto() {
  currentPhoto.value = null
  photoToDelete.value = true
  selectedFile.value = null
}

async function handleSubmit() {
  validationTrigger.value++
  
  if (!isFormValid.value) return

  const submitData = {
    ...form.value,
    relationshipType: props.editingBirthday?.relationshipType || props.relationshipType,
    photo: selectedFile.value || undefined,
    deletePhoto: photoToDelete.value
  }
  
  const result = props.editingBirthday 
    ? await birthdayApi.updateBirthday(props.editingBirthday.id, submitData, {
        showSuccessNotification: true,
        successMessage: 'День рождения успешно обновлен'
      })
    : await birthdayApi.createBirthday(submitData, {
        showSuccessNotification: true,
        successMessage: 'День рождения успешно добавлен'
      })

  if (result) {
    form.value = {
      fullName: '',
      dateOfBirth: '',
      relationshipType: props.relationshipType
    }
    selectedFile.value = null
    fieldValidation.value = {
      fullName: false,
      dateOfBirth: false
    }
    emit('success')
    emit('close')
  }
}
</script>