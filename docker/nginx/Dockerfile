FROM alpine/git as project

WORKDIR /source

RUN git clone --branch main https://github.com/VladislavPetyukevich/GulagGazRoom.git

# Frontend
# FrontendAdmin

FROM node:latest as builder

WORKDIR /source

COPY --from=project /source/GulagGazRoom  .

RUN npm ci --prefix /source/Frontend
RUN npm ci --prefix /source/FrontendAdmin

RUN npm run build --prefix /source/Frontend
RUN npm run build --prefix /source/FrontendAdmin

FROM nginx

COPY nginx.conf /etc/nginx
COPY mime.types /etc/nginx

RUN mkdir -p /etc/ssl/

RUN mkdir -p /home/html/interview
RUN mkdir -p /home/html/admin

COPY --from=builder /source/Frontend/build  /home/html/interview
COPY --from=builder /source/FrontendAdmin/build  /home/html/admin

EXPOSE 8088

CMD ["nginx-debug", "-g", "daemon off;"]