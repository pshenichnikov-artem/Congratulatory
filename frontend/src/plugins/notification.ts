import type { App } from 'vue'
import { notificationService } from '@/composables/useNotification'

export default {
  install: (app: App) => {
    app.config.globalProperties.$notification = notificationService
  },
}
