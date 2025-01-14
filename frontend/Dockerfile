# Build stage
FROM debian:bullseye-slim AS build-env

# Install required dependencies
RUN apt-get update && apt-get install -y \
    git curl unzip xz-utils && \
    rm -rf /var/lib/apt/lists/*

# Define Flutter SDK variables
ARG FLUTTER_SDK=/flutter
ARG FLUTTER_VERSION=stable
ENV FLUTTER_SDK=$FLUTTER_SDK
ENV PATH="$FLUTTER_SDK/bin:$FLUTTER_SDK/bin/cache/dart-sdk/bin:$PATH"

# Clone the Flutter SDK
RUN git clone https://github.com/flutter/flutter.git $FLUTTER_SDK && \
    cd $FLUTTER_SDK && git fetch && git checkout $FLUTTER_VERSION

# Set working directory
WORKDIR /frontend

# Copy project files
COPY . .

# Validate Flutter installation
RUN flutter doctor -v

# Enable web support
RUN flutter config --enable-web

# Install dependencies and build the web app
RUN flutter pub get
RUN flutter build web --base-href=/mediconnect/ && \
    ls -al /frontend/build/web

# Production stage
FROM nginx:alpine AS production-stage

# Copy the built web app to the NGINX HTML directory
COPY --from=build-env /frontend/build/web /usr/share/nginx/html

RUN ls -al /usr/share/nginx/html

# Expose the default NGINX port
EXPOSE 80

# Start NGINX server
CMD ["nginx", "-g", "daemon off;"]
