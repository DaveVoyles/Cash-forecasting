# BUILD
FROM node:8.11.2-alpine as build
WORKDIR /usr/src/app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run-script "build" -- --prod

# RUN
FROM nginx:alpine
COPY --from=build /usr/src/app/dist/cashforecasting /usr/share/nginx/html
EXPOSE 80 443
CMD [ "nginx", "-g", "daemon off;" ]
