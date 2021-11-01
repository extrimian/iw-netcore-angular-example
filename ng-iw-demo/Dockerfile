FROM node:14.17.0-alpine as build-step

RUN mkdir -p /app
WORKDIR /app
COPY package.json /app
RUN yarn install
COPY . /app
RUN yarn build

FROM nginx:1.20.1
COPY --from=build-step /app/dist/ng-iw-demo /usr/share/nginx/html
EXPOSE 80