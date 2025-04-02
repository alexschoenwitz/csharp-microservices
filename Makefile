# Variables
SOLUTION_FILE := BlueprintMonorepo.sln
PROJECT_DIRS := services

# Phony targets (targets that don't represent files)
.PHONY: all update-sln build clean format lint

# Default target
all: build

# Update the solution file with all projects found in PROJECT_DIRS
# This finds all .csproj files and adds them to the solution.
# dotnet sln add is generally idempotent (doesn't fail if project is already added).
update-sln:
	@echo "Updating solution file: $(SOLUTION_FILE)..."
	@find $(PROJECT_DIRS) -name "*.csproj" -print0 | xargs -0 -I {} dotnet sln $(SOLUTION_FILE) add "{}"
	@echo "Solution file updated."

# Build the solution
build:
	@echo "Building solution: $(SOLUTION_FILE)..."
	dotnet build $(SOLUTION_FILE)

# Clean the solution
clean:
	@echo "Cleaning solution: $(SOLUTION_FILE)..."
	dotnet clean $(SOLUTION_FILE)

# Format the code in the solution
format:
	@echo "Formatting solution: $(SOLUTION_FILE)..."
	dotnet format $(SOLUTION_FILE) --severity info

# Lint the code in the solution (check formatting and build warnings)
lint:
	@echo "Linting solution: $(SOLUTION_FILE)..."
	@echo "Checking formatting..."
	dotnet format $(SOLUTION_FILE) --verify-no-changes --severity info --verbosity diagnostic

