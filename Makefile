SLN_FILE := BlueprintMonorepo.slnx
SLN_NAME := $(basename $(SLN_FILE))

# Find all .csproj files recursively, excluding common build/output/test folders.
# Adjust the -prune paths if your structure differs significantly.
# Using := for immediate evaluation on Makefile parse.
PROJECT_FILES := $(shell find . \
	-path ./bin -prune -o \
	-path ./obj -prune -o \
	-path ./test -prune -o \
	-path ./tests -prune -o \
	-path ./node_modules -prune -o \
	-path ./.git -prune -o \
	-name '*.csproj' -print | sed 's|^\./||') # Use sed to remove leading ./ for cleaner paths

.PHONY: update-sln
update-sln:
	@PROJECTS_TO_ADD=""; \
	for proj in $(PROJECT_FILES); do \
		proj_bwd="$$(echo "$$proj" | sed 's|/|\\\\|g')"; \
		if ! grep -q "$$proj" $(SLN_FILE) && ! grep -q "$$proj_bwd" $(SLN_FILE); then \
			PROJECTS_TO_ADD="$$PROJECTS_TO_ADD \"$$proj\""; \
		fi \
	done; \
	if [ -n "$$PROJECTS_TO_ADD" ]; then \
		eval dotnet sln $(SLN_FILE) add $$PROJECTS_TO_ADD; \
	fi


# Target to clean the generated solution file (optional).
.PHONY: clean
clean:
	@echo "--- Cleaning up ---"
	rm -f $(SLN_FILE)
	@echo "Removed $(SLN_FILE)."

# Format the code in the solution
format:
	@echo "Formatting solution: $(SLN_FILE)..."
	dotnet format $(SLN_FILE) --severity info

# Lint the code in the solution (check formatting and build warnings)
lint:
	@echo "Linting solution: $(SLN_FILE)..."
	@echo "Checking formatting..."
	dotnet format $(SLN_FILE) --verify-no-changes --severity info --verbosity diagnostic

