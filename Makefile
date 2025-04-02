# Variables
SOLUTION_FILE := BlueprintMonorepo.sln
PROJECT_DIRS := services

# Phony targets (targets that don't represent files)
.PHONY: all update-sln build clean format lint

# Default target
all: build

# Update the solution file with all projects found in PROJECT_DIRS
# This finds all .csproj files and adds only those not already in the solution.
update-sln:
	@echo "Checking for new projects to add to solution file: $(SOLUTION_FILE)..."
	@# Get projects currently in the solution, normalize paths, and sort
	@CURRENT_PROJECTS=$$(dotnet sln $(SOLUTION_FILE) list | grep ".csproj" | sed 's|\\|/|g' | sort || true); \
	# Find all potential projects, normalize paths, and sort
	@ALL_PROJECTS=$$(find $(PROJECT_DIRS) -name "*.csproj" -print | sed 's|\\|/|g' | sort); \
	# Find projects that are in ALL_PROJECTS but not in CURRENT_PROJECTS
	@PROJECTS_TO_ADD=$$(comm -13 <(echo "$$CURRENT_PROJECTS") <(echo "$$ALL_PROJECTS")); \
	# Add the missing projects, if any
	@if [ -n "$$PROJECTS_TO_ADD" ]; then \
		echo "Adding new projects to solution:"; \
		echo "$$PROJECTS_TO_ADD" | sed 's/^/  /' ; \
		echo "$$PROJECTS_TO_ADD" | xargs -I {} dotnet sln $(SOLUTION_FILE) add "{}" ; \
		echo "Solution file updated."; \
	else \
		echo "No new projects found to add."; \
	fi

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

