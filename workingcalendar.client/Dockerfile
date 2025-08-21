FROM node:22.18.0 AS client
WORKDIR /app
COPY ./workingcalendar.client/package*.json ./
RUN npm install
COPY ./workingcalendar.client .
EXPOSE 3000
CMD ["npm", "run", "dev"]