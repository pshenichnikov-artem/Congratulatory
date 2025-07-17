import type { BirthdayNotificationResponse } from "../birthdayNotification/BirthdayNotificationResponse"

export interface BirthdayResponse {
  id: number
  fullName: string
  dateOfBirth: string
  photoPath?: string
  relationshipType: string,
  notification: BirthdayNotificationResponse
}