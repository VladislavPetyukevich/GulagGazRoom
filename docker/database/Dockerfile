FROM postgres

RUN ln -snf /usr/share/zoneinfo/Europe/Minsk /etc/localtime && echo Europe/Minsk > /etc/timezone

WORKDIR /docker-entrypoint-initdb.d

VOLUME  ["/etc/postgresql", "/var/log/postgresql", "/var/lib/postgresql/data"]