import { createApp } from 'vue'
import './assets/tailwind.css'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import  notification  from '@/plugins/notification'
import { useAuthStore } from '@/stores/auth'

const app = createApp(App)

app.use(createPinia())

const authStore = useAuthStore()
authStore.initializeAuth()

app.use(notification)

app.use(router)



app.mount('#app')