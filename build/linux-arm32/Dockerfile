ARG tag=3.1
FROM mcr.microsoft.com/dotnet/core/runtime:${tag} AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443
COPY qemu-arm-static /usr/bin/
COPY Sannel.House.Devices/ .
RUN ln -s  /var/run/secrets/kubernetes.io/serviceaccount/ca.crt /etc/ssl/certs/kubernetes.pem \
    && groupadd -g 1200 house \
    && useradd -m house -g house -u 1100 \
    && chown -R house:house /app \
    && rm -f /usr/bin/qemu-arm-static
USER 1200
ENTRYPOINT ["dotnet", "Sannel.House.Devices.dll"]