export interface ApiResponse<T = any> {
  success: boolean
  data?: T
  error?: ApiError
}

export interface ApiError<T = any> {
  message?: string
  details?: Record<string, string>
}