<template>
    <teleport to="body">
        <div class="fixed top-4 right-4 z-50 w-full max-w-sm flex flex-col gap-2 pointer-events-none">
            <transition-group name="notification">
                <div v-for="notification in notifications" :key="notification.id" :class="[
                    'shadow-lg rounded-lg p-4 mb-2 transform transition-all duration-300 pointer-events-auto border',
                    notificationTypeClasses[notification.type],
                ]" :style="{
                    maxHeight: notification.visible ? '200px' : '0',
                    opacity: notification.visible ? '1' : '0',
                }">
                    <div class="flex items-start">
                        <div class="flex-shrink-0">
                            <IconsSet :name="notification.type" class="h-5 w-5 text-white" />
                        </div>
                        <div class="ml-3 flex-1">
                            <p class="text-sm font-semibold text-white">{{ notification.message }}</p>
                        </div>
                        <div class="ml-auto pl-3">
                            <button @click="closeNotification(notification.id)"
                                class="inline-flex rounded-md p-1.5 focus:outline-none focus:ring-2 focus:ring-offset-2 text-white hover:text-gray-200"
                                :class="buttonClass(notification.type)">
                                <span class="sr-only">Close</span>
                                <IconsSet name="close" class="h-4 w-4" />
                            </button>
                        </div>
                    </div>
                </div>
            </transition-group>
        </div>
    </teleport>
</template>

<script setup lang="ts">
import { notificationService } from '@/composables/useNotification'

const { notifications, closeNotification } = notificationService

const notificationTypeClasses = {
    success: 'bg-notification-success border-notification-success-border',
    error: 'bg-notification-error border-notification-error-border',
    info: 'bg-notification-info border-notification-info-border',
    warning: 'bg-notification-warning border-notification-warning-border',
}

const buttonClass = (type: string) => {
    return {
        success: 'focus:ring-notification-success',
        error: 'focus:ring-notification-error',
        info: 'focus:ring-notification-info',
        warning: 'focus:ring-notification-warning',
    }[type as keyof typeof notificationTypeClasses];
}
</script>

<style scoped>
.notification-enter-active,
.notification-leave-active {
    transition: all 0.3s ease;
}

.notification-enter-from,
.notification-leave-to {
    transform: translateX(30px);
    opacity: 0;
}

[class*="bg-notification-"] {
    box-shadow: 0 6px 16px rgba(0, 0, 0, 0.2);
}
</style>
