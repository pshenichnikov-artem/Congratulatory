FROM node:22-alpine AS build-stage
WORKDIR /app

ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

COPY package*.json ./
RUN npm install

COPY . .

RUN npm run build

FROM nginx:1.25.2

COPY --from=build-stage /app/dist /usr/share/nginx/html

# Копируем свой конфиг nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
