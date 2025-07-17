export interface BirthdayCreateRequest {
  fullName: string
  dateOfBirth: string
  photo?: File
  relationshipType: string
}