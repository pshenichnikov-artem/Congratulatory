import axios, { AxiosError } from 'axios'
import type { AxiosRequestConfig } from 'axios'
import { readonly } from 'vue'
import { ref, computed, reactive } from 'vue'
import { notificationService } from '@/composables/useNotification'
import { API_BASE_URL } from '@/constants/api'
import type { ApiResponse, ApiError } from '@/types/common/ApiResponse'

export interface RequestOptions {
  showSuccessNotification?: boolean
  showErrorNotification?: boolean
  successMessage?: string
  errorMessage?: string
  onSuccess?: (data: any) => void
  onError?: (error: any) => void
  onFinally?: () => void
  debounceDelay?: number
}

export interface LoadingState {
  [key: string]: boolean
}


export function useBaseApi<T>(
  endpoint: string,
  loadingKeys: string[] = ['get', 'search', 'create', 'update', 'delete'],
) {
  const baseUrl = `${API_BASE_URL}/${endpoint}`

  const loading = reactive<LoadingState>(
    loadingKeys.reduce((acc, key) => ({ ...acc, [key]: false }), {}),
  )

  const isLoading = computed(() => Object.values(loading).some((state) => state))

  const error = ref<any>(null)
  const data = ref<T | null>(null)
  const items = ref<T[]>([])
  const totalCount = ref<number>(0)

  async function makeRequest<R>(
    config: AxiosRequestConfig,
    loadingKey: string,
    options: RequestOptions = {},
  ): Promise<ApiResponse<R> | null> {
    const {
      showSuccessNotification = false,
      showErrorNotification = true,
      successMessage,
      errorMessage,
      onSuccess,
      onError,
      onFinally,
    } = options

    if (loadingKey in loading) {
      loading[loadingKey] = true
    }
    error.value = null

    try {
      const response = await axios.request<ApiResponse<R>>({
        ...config,
        url: config.url?.startsWith('http') ? config.url : `${baseUrl}${config.url || ''}`,
        validateStatus: () => true,
      })

      const responseData = response.data
      console.log(responseData)
      if (responseData.success) {
        if (
          responseData.data &&
          typeof responseData.data === 'object' &&
          'items' in responseData.data &&
          'totalCount' in responseData.data
        ) {
          items.value = (responseData.data.items as T[]) || []
          totalCount.value = typeof responseData.data.totalCount === 'number' 
            ? responseData.data.totalCount 
            : parseInt(String(responseData.data.totalCount), 10) || 0
        } else if (responseData.data && Array.isArray(responseData.data)) {
          items.value = responseData.data as T[]
          totalCount.value = responseData.data.length
        } else if (responseData.data) {
          data.value = responseData.data as T
        }

        if (responseData.data && showSuccessNotification) {
          notificationService.success(successMessage || "Успех")
        }

        if (onSuccess) {
          onSuccess(responseData.data)
        }

        if (onFinally) {
          onFinally()
        }

        return responseData
      } else {
        error.value = responseData.error || { message: responseData.error }

        if (showErrorNotification && !onError) {
          notificationService.error(
            errorMessage || responseData.error?.message || "Возникла ошибка",
          )
        }

        if (onError) {
          onError(error.value)
        }

        if (onFinally) {
          onFinally()
        }

        return responseData
      }
    } catch (err) {
      const axiosError = err as AxiosError<ApiResponse<any>>

      if (axiosError.code === 'ERR_NETWORK') {
        error.value = {
          code: 0,
          message: "Ошибка подключения к серверу",
        }

        if (showErrorNotification) {
          notificationService.error(errorMessage || "Ошибка подключения к серверу")
        }
      } else {
        error.value = axiosError.response?.data?.error || {
          code: axiosError.response?.status || 500,
          message: axiosError.message,
        }

        if (showErrorNotification) {
          const errorMsg =
            axiosError.response?.data?.error?.message ||
            axiosError.message ||
            errorMessage ||
            "Ошибка"
          notificationService.error(errorMsg)
        }
      }

      if (onError) {
        onError(error.value)
      }

      if (onFinally) {
        onFinally()
      }

      return null
    } finally {
      if (loadingKey in loading) {
        loading[loadingKey] = false
      }
    }
  }

  function withDebounce<F extends (...args: any[]) => Promise<any>>(fn: F, delay: number): F {
    let timeout: ReturnType<typeof setTimeout> | null = null

    const debouncedFn = async function (...args: any[]): Promise<any> {
      return new Promise((resolve) => {
        if (timeout) {
          clearTimeout(timeout)
        }
        timeout = setTimeout(() => {
          resolve(fn(...args))
          timeout = null
        }, delay)
      })
    } as F

    return debouncedFn
  }

  function resetState(): void {
    data.value = null
    items.value = []
    totalCount.value = 0
    error.value = null
  }

  return {
    baseUrl,
    loading: readonly(loading),
    isLoading,
    error,
    data,
    items,
    totalCount,
    makeRequest,
    withDebounce,
    resetState,
  }
}