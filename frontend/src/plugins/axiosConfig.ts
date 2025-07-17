import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

axios.interceptors.request.use(
  (config) => {
    const authStore = useAuthStore()
    const token = authStore.getToken

    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error),
)
