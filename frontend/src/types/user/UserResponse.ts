import type { BirthdayResponse } from '../birthday/BirthdayResponse'
import type { UserAccountResponse } from '../userAccount/UserAccountResponse'

export interface UserResponse {
  id: string
  userName: string
  email: string
  createdAt: string
  updatedAt: string
  birthdays: BirthdayResponse[]
  userAccounts: UserAccountResponse[]
}

export interface LoginResponse {
  token: string
  user: UserResponse
}