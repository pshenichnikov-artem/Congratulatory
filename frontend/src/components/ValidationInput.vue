<template>
  <div class="mb-4">
    <label
      v-if="label"
      class="block text-sm font-medium mb-1"
      :class="[props.titleColor]"
      >{{ label }}</label
    >
    <div class="relative flex items-center">
      <input
        class="w-full border rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 pr-10"
        :class="[
          {
            [props.borderColor ? props.borderColor : '']: !error && !isRed && !isValid,
            'border-green-500 focus:ring-green-500': isValid && !error && !isEmpty,
            'border-red-500 focus:ring-red-500': isRed,
          },
          props.textColor,
          props.backgroundColor,
        ]"
        :id="id"
        :type="computedInputType"
        :placeholder="placeholder"
        :value="modelValue"
        @input="handleInput"
        @blur="handleBlur"
      />

      <button
        v-if="type === 'password'"
        type="button"
        class="absolute inset-y-0 right-2 text-gray-400 text-sm flex items-center justify-center"
        @click="togglePasswordVisibility"
      >
        <img
          :src="showPassword ? closeEye : openEye"
          alt="eye icon"
          class="w-6 h-6"
        />
      </button>
    </div>
    <p v-if="error" class="text-xs text-red-500 mt-1">
      {{ error }}
    </p>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, computed } from "vue";
import closeEye from '@/assets/images/close_eye.png'
import openEye from '@/assets/images/open_eye.png'

type ValidationRule = {
  name: string;
  param?: string;
};

export interface ValidationProps {
  id?: string;
  modelValue: string | number;
  label?: string;
  type?: string;
  placeholder?: string;
  validationRules?: string | ValidationRule[];
  errorMessages?: Record<string, string>;
  compareWith?: string | null;
  triggerValidation?: number;
  backgroundColor?: string | null;
  titleColor?: string | null;
  placeholderColor?: string | null;
  textColor?: string | null;
  borderColor?: string | null
}

const props = withDefaults(defineProps<ValidationProps>(), {
  id: "",
  modelValue: "",
  label: "",
  type: "text",
  placeholder: "",
  validationRules: "",
  errorMessages: () => ({}),
  compareWith: null,
  triggerValidation: 0,
  backgroundColor: "bg-form-light",
  titleColor: "text-text-form",
  placeholderColor: "text-gray-500",
  textColor: "text-black",
  borderColor: "border-gray-300",
});

const emit = defineEmits<{
  "update:modelValue": [value: string | number];
  input: [value: string];
  "update:validationErrors": [error: Record<string, string> | null];
  valid: [isValid: boolean];
}>();

const error = ref<string | null>(null);
const isValid = ref<boolean>(false);
const showPassword = ref<boolean>(false);
const wasValidated = ref<boolean>(false);
const forceShowError = ref<boolean>(false);
const isEmpty = computed(() => !props.modelValue);

const isRed = computed(() => {
  return (
    error.value !== null || (forceShowError.value && !isValid.value && !isEmpty.value)
  );
});

const computedInputType = computed((): string => {
  if (props.type !== "password") return props.type;
  return showPassword.value ? "text" : "password";
});

// Кастомные паттерны валидации
const customPatterns: Record<string, RegExp> = {
  email: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
  phone: /^\+?[0-9]{10,15}$/,
  name: /^(?:[A-Z][a-z]{2,49}|[А-ЯЁ][а-яё]{2,49})$/,
  lastName: /^(?:[A-Z][a-z]{2,49}|[А-ЯЁ][а-яё]{2,49})$/,
  fullName: /^((?:[A-Z][a-z]{2,}|[А-ЯЁ][а-яё]{2,})\s+(?:[A-Z][a-z]{2,}|[А-ЯЁ][а-яё]{2,})(?:\s+(?:[A-Z][a-z]{2,}|[А-ЯЁ][а-яё]{2,}))?)$/,
  password: /^(?=.*[a-zа-яё])(?=.*[A-ZА-ЯЁ])(?=.*\d)(?=.*[!@#$%^&*]).{8,100}$/,
  onlyRussian: /^[А-ЯЁа-яё\s]+$/,
  onlyEnglish: /^[A-Za-z\s]+$/,
};

const togglePasswordVisibility = (): void => {
  showPassword.value = !showPassword.value;
};

const isValidated = computed((): boolean => {
  return isValid.value && !error.value;
});

function parseRules(rules: string | ValidationRule[] | undefined): ValidationRule[] {
  if (!rules) return [];

  if (Array.isArray(rules)) return rules;

  return rules.split("|").map((rule) => {
    const [name, param] = rule.split(":");
    return { name, param };
  });
}

function getValidators(): Record<string, (v: string, p?: string) => string | boolean> {
  return {
    required: (v) => !!v || props.errorMessages.required || "Поле обязательно для заполнения",
    minLength: (v, p) =>
      v.length >= +(p || 0) ||
      props.errorMessages.minLength ||
      `Минимальная длина ${p} символов`,
    maxLength: (v, p) =>
      v.length <= +(p || 0) ||
      props.errorMessages.maxLength ||
      `Максимальная длина ${p} символов`,
    min: (v, p) =>
      +v >= +(p || 0) || props.errorMessages.min || `Минимальное значение ${p}`,
    max: (v, p) =>
      +v <= +(p || 0) || props.errorMessages.max || `Максимальное значение ${p}`,
    pattern: (v, p) =>
      new RegExp(p || "").test(v) ||
      props.errorMessages.pattern ||
      "Неверный формат",
    date: (v) => {
      if (!v) return true;
      const date = new Date(v);
      return !isNaN(date.getTime()) || props.errorMessages.date || "Неверная дата";
    },
    match: (v) => {
      if (props.compareWith == null)
        return props.errorMessages.match || "Нет значения для сравнения";
      if (v !== props.compareWith)
        return props.errorMessages.match || "Пароли не совпадают";
      return true;
    },
    name: (v) => {
      if (v.length < 3)
        return props.errorMessages.nameMinLength || "Минимальная длина имени 3 символа";
      if (v.length > 50)
        return props.errorMessages.nameMaxLength || "Максимальная длина имени 50 символов";
      if (!/^[A-ZА-ЯЁ]/.test(v))
        return props.errorMessages.nameUppercase || "Имя должно начинаться с заглавной буквы";
      if (!customPatterns.name.test(v))
        return props.errorMessages.namePattern || "Неверный формат имени";
      return true;
    },
    lastName: (v) => {
      if (v.length < 3)
        return props.errorMessages.lastNameMinLength || "Минимальная длина фамилии 3 символа";
      if (v.length > 50)
        return props.errorMessages.lastNameMaxLength || "Максимальная длина фамилии 50 символов";
      if (!/^[A-ZА-ЯЁ]/.test(v))
        return props.errorMessages.lastNameUppercase || "Фамилия должна начинаться с заглавной буквы";
      if (!customPatterns.lastName.test(v))
        return props.errorMessages.lastNamePattern || "Неверный формат фамилии";
      return true;
    },
    email: (v) =>
      customPatterns.email.test(v) ||
      props.errorMessages.email ||
      "Неверный формат email",
    phone: (v) =>
      customPatterns.phone.test(v) ||
      props.errorMessages.phone ||
      "Неверный формат телефона",
    password: (v) => {
      if (v.length < 8)
        return props.errorMessages.passwordMinLength || "Минимальная длина пароля 8 символов";
      if (!/[A-ZА-ЯЁ]/.test(v))
        return props.errorMessages.passwordUppercase || "Пароль должен содержать заглавную букву";
      if (!/[!@#$%^&*]/.test(v))
        return props.errorMessages.passwordSpecial || "Пароль должен содержать специальный символ";
      if (!/\d/.test(v))
        return props.errorMessages.passwordDigit || "Пароль должен содержать цифру";
      if (!customPatterns.password.test(v))
        return props.errorMessages.passwordPattern || "Пароль не соответствует требованиям";
      return true;
    },
    onlyRussian: (v) => customPatterns.onlyRussian.test(v) || "Только русские буквы",
    onlyEnglish: (v) => customPatterns.onlyEnglish.test(v) || "Только английские буквы",
    passport: (v: string) => {
      if (!v)
        return props.errorMessages.passportRequired || "Паспорт обязателен";
      if (!/^\d{4}\s?\d{6}$/.test(v))
        return props.errorMessages.passportPattern || "Неверный формат паспорта";
      return true;
    },

    birthDocument: (v: string) => {
      if (!v)
        return props.errorMessages.birthDocumentRequired || "Свидетельство о рождении обязательно";
      if (!/^[IVXLCDM]{1,5}-\d{6}$/.test(v)) {
        return props.errorMessages.birthDocumentPattern || "Неверный формат свидетельства о рождении";
      }
      return true;
    },

    driverLicense: (v: string) => {
      if (!v)
        return props.errorMessages.driverLicenseRequired || "Водительские права обязательны";
      if (!/^\d{4}\s?\d{6}$/.test(v))
        return props.errorMessages.driverLicensePattern || "Неверный формат водительских прав";
      return true;
    },

    studentCard: (v: string) => {
      if (!v)
        return props.errorMessages.studentCardRequired || "Студенческий билет обязателен";
      if (!/^[А-Яа-яA-Za-z0-9\-]{5,20}$/.test(v)) {
        return props.errorMessages.studentCardPattern || "Неверный формат студенческого билета";
      }
      return true;
    },
    fullName: (v) => {
      const words = v.trim().split(/\s+/);

      if (words.length < 2 || words.length > 3) {
        return props.errorMessages.fullNameWordsCount || "Полное имя должно содержать 2-3 слова";
      }

      for (const word of words) {
        if (word.length < 3) {
          return props.errorMessages.fullNameWordLength || "Каждое слово должно содержать минимум 3 символа";
        }

        if (!/^[A-ZА-ЯЁ]/.test(word)) {
          return props.errorMessages.fullNameUppercase || "Каждое слово должно начинаться с заглавной буквы";
        }

        if (!/^[A-Za-zА-ЯЁа-яё]+$/.test(word)) {
          return props.errorMessages.fullNameLettersOnly || "Только буквы в имени";
        }
      }

      if (!customPatterns.fullName.test(v)) {
        return props.errorMessages.fullNamePattern || "Неверный формат полного имени";
      }

      return true;
    },
  };
}

function validate(): boolean {
  error.value = null;
  isValid.value = false;
  wasValidated.value = true;

  if (isEmpty.value) {
    const rules = parseRules(props.validationRules);

    if (!rules.some((r) => r.name === "required")) {
      isValid.value = true;
      emit("update:validationErrors", null);
      emit("valid", true);
      forceShowError.value = false;
      return true;
    } else if (forceShowError.value) {
      error.value = props.errorMessages.required || "Поле обязательно для заполнения";
      emit("update:validationErrors", { [props.id]: error.value });
      emit("valid", false);
      return false;
    }
    emit("valid", false);
    return false;
  }

  const rules = parseRules(props.validationRules);
  const validators = getValidators();

  for (const { name, param } of rules) {
    const validator = validators[name];
    if (validator) {
      const result = validator(
        typeof props.modelValue === "number"
          ? props.modelValue.toString()
          : props.modelValue,
        param
      );
      if (result !== true) {
        // Используем переданные сообщения об ошибках или берем из локализации
        error.value = result as string;
        emit("update:validationErrors", { [props.id]: error.value });
        emit("valid", false);
        return false;
      }
    }
  }

  isValid.value = true;
  emit("update:validationErrors", null);
  emit("valid", true);
  return true;
}

// Обработчик потери фокуса
function handleBlur(): void {
  wasValidated.value = true;

  // При потере фокуса выполняем валидацию только если поле не пустое или обязательное
  if (
    !isEmpty.value ||
    parseRules(props.validationRules).some((r) => r.name === "required")
  ) {
    validate();
  }
}

// Обработчик ввода значений
function handleInput(e: Event): void {
  const target = e.target as HTMLInputElement;
  const value = target.value;
  emit("update:modelValue", value);
  emit("input", value);

  // Если значение полностью стерли
  if (value === "") {
    const rules = parseRules(props.validationRules);
    error.value = null; // Всегда сбрасываем ошибку при пустом поле

    if (!rules.some((r) => r.name === "required")) {
      // Если поле не обязательное, считаем его валидным
      isValid.value = true;
      forceShowError.value = false; // Сбрасываем показ ошибок
      wasValidated.value = false; // Сбрасываем флаг валидации
      emit("valid", true);
    } else {
      // Если поле обязательное, но мы только что стерли значение - не показываем ошибку сразу
      isValid.value = false;
      forceShowError.value = false; // Скрываем ошибки, пока поле пустое
      emit("valid", false);
    }
  }
  // Проверяем валидацию при любом вводе, если значение не пустое
  else {
    // Если пользователь начал вводить, считаем что поле было валидировано
    wasValidated.value = true;
    validate();
  }
}

// Экспортируем метод validate и состояние isValidated для использования извне
defineExpose({
  validate,
  isValidated,
});

// Валидируем значение при монтировании компонента
onMounted(() => {
  if (props.modelValue) {
    wasValidated.value = true; // Устанавливаем флаг валидации, если есть начальное значение
    validate();
  }
});

// Следим за изменениями triggerValidation для запуска валидации
watch(
  () => props.triggerValidation,
  () => {
    if (props.triggerValidation > 0) {
      forceShowError.value = true; // Показываем ошибки при принудительной валидации
      validate();
    }
  }
);

// Следим за изменениями modelValue для автоматической валидации
watch(
  () => props.modelValue,
  () => {
    // Если поле не пустое и было уже провалидировано, то продолжаем валидацию
    if (!isEmpty.value && wasValidated.value) {
      validate();
    } else if (isEmpty.value) {
      // При пустом поле сбрасываем ошибки, но сохраняем состояние валидации
      error.value = null;
      if (!parseRules(props.validationRules).some((r) => r.name === "required")) {
        isValid.value = true;
        emit("valid", true);
      }
    }
  }
);

// Следим за изменениями compareWith для перевалидации полей, зависящих от других значений
watch(
  () => props.compareWith,
  () => {
    if (wasValidated.value && !isEmpty.value) {
      // Если поле уже было валидировано и не пустое, перепроверяем
      validate();
    }
  }
);
</script>
