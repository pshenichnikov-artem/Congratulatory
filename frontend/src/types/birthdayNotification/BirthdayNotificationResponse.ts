import type { UserAccountResponse } from "../userAccount/UserAccountResponse"

export interface BirthdayNotificationResponse {
  id: number
  birthdayId: number
  userAccount: UserAccountResponse
  notificationDate: string
}