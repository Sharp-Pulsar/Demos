# ----------------------------
# build from source
# ----------------------------
# https://jasonwatmore.com/angular-15-16-free-course-8-dockerize-app-with-nginx
FROM node:21 AS build

WORKDIR /app

COPY package*.json .
RUN npm install -g npm@10.2.5

COPY . .
RUN npm run build

# ----------------------------
# run with nginx
# ----------------------------
FROM nginx

RUN rm /etc/nginx/conf.d/default.conf
COPY nginx.conf /etc/nginx/conf.d
COPY --from=build /app/dist/angularapp /usr/share/nginx/html

EXPOSE 80
EXPOSE 443
