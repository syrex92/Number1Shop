FROM node:lts-alpine
#RUN apt-get -y update && apt-get -y install curl && apt-get clean
RUN apk add --no-cache curl

WORKDIR /app
COPY package.json .
RUN npm install
COPY . .

# Ensure Vite env exists during docker build.
# If developer didn't create .env locally, fall back to .env.sample.
RUN if [ ! -f .env ] && [ -f .env.sample ]; then cp .env.sample .env; fi

RUN npm run build

EXPOSE 8080

CMD [ "npm", "run", "preview" ]