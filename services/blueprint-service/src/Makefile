# Blueprint Service Makefile

# Variables
PROJECT_NAME = BlueprintService
DOCKER_IMAGE = blueprint-service
TAG = latest
DEV_TAG = dev
DOTNET = dotnet
DOCKER = docker
KUBE = kubectl
PROTO_DIR = proto
PROTO_OUT_DIR = ./BlueprintProto
PROTO_TOOLS_VERSION = 3.21.12
K8S_BASE_DIR = k8s/base
K8S_DEV_DIR = k8s/overlays/dev

.PHONY: help build run test clean docker-build docker-run proto k8s-apply-dev k8s-delete-dev db-setup db-reset

# Default target
help:
	@echo "Blueprint Service Makefile"
	@echo ""
	@echo "Usage:"
	@echo "  make build           - Build the service"
	@echo "  make run             - Run the service locally"
	@echo "  make test            - Run tests"
	@echo "  make clean           - Clean build artifacts"
	@echo "  make proto           - Generate code from proto files"
	@echo "  make docker-build    - Build Docker image"
	@echo "  make docker-run      - Run service in Docker"
	@echo "  make k8s-apply-dev   - Apply development Kubernetes configuration"
	@echo "  make k8s-delete-dev  - Delete development Kubernetes resources"
	@echo "  make db-setup        - Set up the database schema"
	@echo "  make db-reset        - Reset the database"
	@echo ""

# Build the service
build:
	@echo "Building $(PROJECT_NAME)..."
	$(DOTNET) build $(PROJECT_NAME).csproj -c Release

# Run the service locally
run:
	@echo "Running $(PROJECT_NAME)..."
	$(DOTNET) run --project $(PROJECT_NAME).csproj

# Run tests
test:
	@echo "Running tests..."
	$(DOTNET) test

# Clean build artifacts
clean:
	@echo "Cleaning build artifacts..."
	$(DOTNET) clean
	rm -rf bin obj

# Generate code from proto files
proto:
	@echo "Generating code from proto files..."
	@mkdir -p $(PROTO_OUT_DIR)
	$(DOTNET) tool restore
	$(DOTNET) tool run protobuf-tools generate --proto $(PROTO_DIR)/blueprint.proto --csharp_out=$(PROTO_OUT_DIR)

# Build Docker image
docker-build:
	@echo "Building Docker image..."
	$(DOCKER) build -t $(DOCKER_IMAGE):$(TAG) .
	$(DOCKER) build -t $(DOCKER_IMAGE):$(DEV_TAG) --build-arg ASPNETCORE_ENVIRONMENT=Development .

# Run service in Docker
docker-run:
	@echo "Running service in Docker..."
	$(DOCKER) run -p 8080:8080 \
		-e ASPNETCORE_ENVIRONMENT=Development \
		-e Database__ConnectionString="Host=host.docker.internal;Port=5432;Database=blueprints;Username=postgres;Password=postgres;" \
		$(DOCKER_IMAGE):$(DEV_TAG)

# Apply development Kubernetes configuration
k8s-apply-dev:
	@echo "Applying development Kubernetes configuration..."
	$(KUBE) apply -k $(K8S_DEV_DIR)

# Delete development Kubernetes resources
k8s-delete-dev:
	@echo "Deleting development Kubernetes resources..."
	$(KUBE) delete -k $(K8S_DEV_DIR)

# Set up the database schema
db-setup:
	@echo "Setting up database schema..."
	@if [ -z "$(PGPASSWORD)" ]; then \
		echo "Please set PGPASSWORD environment variable"; \
		exit 1; \
	fi
	@echo "CREATE DATABASE blueprints;" | psql -h localhost -U postgres -p 5432 postgres || true
	@cat db/schema.sql | psql -h localhost -U postgres -p 5432 blueprints

# Reset the database
db-reset:
	@echo "Resetting database..."
	@if [ -z "$(PGPASSWORD)" ]; then \
		echo "Please set PGPASSWORD environment variable"; \
		exit 1; \
	fi
	@echo "DROP DATABASE IF EXISTS blueprints;" | psql -h localhost -U postgres -p 5432 postgres
	@echo "CREATE DATABASE blueprints;" | psql -h localhost -U postgres -p 5432 postgres
	@cat db/schema.sql | psql -h localhost -U postgres -p 5432 blueprints
