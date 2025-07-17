import { reactive } from 'vue'
import { useBaseApi, type RequestOptions} from './useBaseApi'
import type { BirthdayCreateRequest } from '@/types/birthday/BirthdayCreateRequest'
import type { BirthdayUpdateRequest } from '@/types/birthday/BirthdayUpdateRequest'
import type { BirthdayResponse } from '@/types/birthday/BirthdayResponse'
import type { BirthdaySearchRequest } from '@/types/birthday/BirthdaySearchRequest'
import type { BirthdayNotificationRequest } from '@/types/birthdayNotification/BirthdayNotificationRequest'
import type { BirthdayNotificationResponse } from '@/types/birthdayNotification/BirthdayNotificationResponse'

export function useBirthdayApi() {
  const baseApi = useBaseApi<BirthdayResponse>('birthdays', [
    'search', 'get', 'create', 'update', 'delete', 'notifications'
  ])

  async function searchBirthdays(
    request: BirthdaySearchRequest,
    options: RequestOptions = {},
  ): Promise<BirthdayResponse[]> {
    const response = await baseApi.makeRequest<BirthdayResponse[]>(
      { method: 'POST', url: '/search', data: request },
      'search',
      options
    )
    return response !== null && response.data ? response.data : []
  }

  async function getBirthday(
    id: number,
    options: RequestOptions = {},
  ): Promise<BirthdayResponse | null> {
    const response = await baseApi.makeRequest<BirthdayResponse>(
      { method: 'GET', url: `/${id}` },
      'get',
      options
    )
    return response?.data || null
  }

  async function createBirthday(
    request: BirthdayCreateRequest,
    options: RequestOptions = {},
  ): Promise<BirthdayResponse | null> {
    const formData = new FormData()
    formData.append('fullName', request.fullName)
    formData.append('dateOfBirth', request.dateOfBirth)
    formData.append('relationshipType', request.relationshipType)
    if (request.photo) {
      formData.append('photo', request.photo)
    }

    const response = await baseApi.makeRequest<BirthdayResponse>(
      { 
        method: 'POST', 
        data: formData, 
        headers: { 'Content-Type': 'multipart/form-data' } 
      },
      'create',
      options
    )
    return response?.data || null
  }

  async function updateBirthday(
    id: number,
    request: BirthdayUpdateRequest,
    options: RequestOptions = {},
  ): Promise<BirthdayResponse | null> {
    const formData = new FormData()
    formData.append('fullName', request.fullName)
    formData.append('dateOfBirth', request.dateOfBirth)
    formData.append('relationshipType', request.relationshipType)
    if (request.photo) {
      formData.append('photo', request.photo)
    }

    const response = await baseApi.makeRequest<BirthdayResponse>(
      { 
        method: 'PUT', 
        url: `/${id}`, 
        data: formData, 
        headers: { 'Content-Type': 'multipart/form-data' } 
      },
      'update',
      options
    )
    return response?.data || null
  }

  async function deleteBirthday(
    id: number,
    options: RequestOptions = {},
  ): Promise<boolean> {
    const response = await baseApi.makeRequest<void>(
      { method: 'DELETE', url: `/${id}` },
      'delete',
      options
    )
    return response !== null
  }

  async function getBirthdayNotifications(
    birthdayId: number,
    options: RequestOptions = {},
  ): Promise<BirthdayNotificationResponse[] | null> {
    const response = await baseApi.makeRequest<BirthdayNotificationResponse[]>(
      { method: 'GET', url: `/${birthdayId}/notifications` },
      'notifications',
      options
    )
    return response?.data || null
  }

  async function upsertBirthdayNotification(
    birthdayId: number,
    request: BirthdayNotificationRequest,
    options: RequestOptions = {},
  ): Promise<BirthdayNotificationResponse | null> {
    const response = await baseApi.makeRequest<BirthdayNotificationResponse>(
      { method: 'POST', url: `/${birthdayId}/notifications`, data: request },
      'notifications',
      options
    )
    return response?.data || null
  }

  async function deleteBirthdayNotification(
    birthdayId: number,
    notificationId: number,
    options: RequestOptions = {},
  ): Promise<boolean> {
    const response = await baseApi.makeRequest<void>(
      { method: 'DELETE', url: `/${birthdayId}/notifications/${notificationId}` },
      'notifications',
      options
    )
    return response !== null
  }

  const birthdayApi = reactive({
    loading: baseApi.loading,
    isLoading: baseApi.isLoading,
    birthday: baseApi.data,
    birthdays: baseApi.items,
    totalCount: baseApi.totalCount,
    error: baseApi.error,
    searchBirthdays,
    getBirthday,
    createBirthday,
    updateBirthday,
    deleteBirthday,
    getBirthdayNotifications,
    upsertBirthdayNotification,
    deleteBirthdayNotification,
    resetState: baseApi.resetState,
  })

  return birthdayApi
}