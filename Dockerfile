ARG DOTNET_VERSION=8.0

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime

#PUPPETEER RECIPE
RUN apt-get update && apt-get -f install && apt-get -y install wget gnupg2 apt-utils
RUN wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN echo 'deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main' >> /etc/apt/sources.list
RUN apt-get update \
&& apt-get install -y google-chrome-stable --no-install-recommends --allow-downgrades fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf
ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome-stable"

ENV TZ=Asia/Bangkok
WORKDIR /app
COPY ./publish/ .
ENTRYPOINT [ "dotnet", "STD.dll" ]