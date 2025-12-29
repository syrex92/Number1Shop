FROM node:lts-alpine
#RUN apt-get -y update && apt-get -y install curl && apt-get clean
RUN apk add --no-cache curl

WORKDIR /app
COPY package.json .
RUN npm install
COPY . .

RUN npm run build

EXPOSE 8080

CMD [ "npm", "run", "preview" ]