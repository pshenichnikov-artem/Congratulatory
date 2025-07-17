import { reactive } from 'vue'
import { useBaseApi, type RequestOptions } from './useBaseApi'
import type { RegisterRequest } from '@/types/user/RegisterRequest'
import type { LoginRequest } from '@/types/user/LoginRequest'
import type { LoginResponse, UserResponse } from '@/types/user/UserResponse'
import type { UpdateUserRequest } from '@/types/user/UpdateUserRequest'
import type { ChangePasswordRequest } from '@/types/user/ChangePasswordRequest'
import type { UserAccountRequest } from '@/types/userAccount/UserAccountRequest'
import type { UserAccountResponse } from '@/types/userAccount/UserAccountResponse'

export function useUserApi() {
  const baseApi = useBaseApi<UserResponse>('user', [
    'register', 'login', 'get', 'update', 'changePassword', 'accounts'
  ])

  async function register(
    request: RegisterRequest,
    options: RequestOptions = {},
  ): Promise<LoginResponse | null> {
    const response = await baseApi.makeRequest<LoginResponse>(
      { method: 'POST', url: '/register', data: request },
      'register',
      options
    )
    return response?.data || null
  }

  async function login(
    request: LoginRequest,
    options: RequestOptions = {},
  ): Promise<LoginResponse | null> {
    const response = await baseApi.makeRequest<LoginResponse>(
      { method: 'POST', url: '/login', data: request },
      'login',
      options
    )
    return response?.data || null
  }

  async function getCurrentUser(
    options: RequestOptions = {},
  ): Promise<UserResponse | null> {
    const response = await baseApi.makeRequest<UserResponse>(
      { method: 'GET', url: '/me' },
      'get',
      options
    )
    return response?.data || null
  }

  async function updateUser(
    request: UpdateUserRequest,
    options: RequestOptions = {},
  ): Promise<UserResponse | null> {
    const response = await baseApi.makeRequest<UserResponse>(
      { method: 'PUT', url: '/me', data: request },
      'update',
      options
    )
    return response?.data || null
  }

  async function changePassword(
    request: ChangePasswordRequest,
    options: RequestOptions = {},
  ): Promise<boolean> {
    const response = await baseApi.makeRequest<void>(
      { method: 'PUT', url: '/me/password', data: request },
      'changePassword',
      options
    )
    return response !== null
  }

  async function getUserAccounts(
    options: RequestOptions = {},
  ): Promise<UserAccountResponse[] | null> {
    const response = await baseApi.makeRequest<UserAccountResponse[]>(
      { method: 'GET', url: '/me/accounts' },
      'accounts',
      options
    )
    return response?.data || null
  }

  async function upsertUserAccount(
    request: UserAccountRequest,
    options: RequestOptions = {},
  ): Promise<UserAccountResponse | null> {
    const response = await baseApi.makeRequest<UserAccountResponse>(
      { method: 'POST', url: '/me/accounts', data: request },
      'accounts',
      options
    )
    return response?.data || null
  }

  async function deleteUserAccount(
    accountId: number,
    options: RequestOptions = {},
  ): Promise<boolean> {
    const response = await baseApi.makeRequest<void>(
      { method: 'DELETE', url: `/me/accounts/${accountId}` },
      'accounts',
      options
    )
    return response !== null
  }

  const userApi = reactive({
    loading: baseApi.loading,
    isLoading: baseApi.isLoading,
    user: baseApi.data,
    error: baseApi.error,
    register,
    login,
    getCurrentUser,
    updateUser,
    changePassword,
    getUserAccounts,
    upsertUserAccount,
    deleteUserAccount,
    resetState: baseApi.resetState,
  })

  return userApi
}