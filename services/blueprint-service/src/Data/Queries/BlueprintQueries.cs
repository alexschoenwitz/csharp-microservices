namespace BlueprintService.Data.Queries
{
    public static class BlueprintQueries
    {
        public const string CreateTable = @"
            CREATE TABLE IF NOT EXISTS blueprints (
                id UUID PRIMARY KEY,
                name TEXT NOT NULL,
                description TEXT NOT NULL,
                created_at TIMESTAMPTZ NOT NULL,
                updated_at TIMESTAMPTZ NOT NULL,
                metadata JSONB NOT NULL DEFAULT '{}'
            );
            
            CREATE INDEX IF NOT EXISTS idx_blueprints_name ON blueprints(name);
        ";

        public const string GetById = @"
            SELECT 
                id,
                name,
                description,
                created_at,
                updated_at,
                metadata
            FROM 
                blueprints
            WHERE 
                id = @Id;
        ";

        public const string GetAll = @"
            SELECT 
                id,
                name,
                description,
                created_at,
                updated_at,
                metadata
            FROM 
                blueprints
            ORDER BY 
                name ASC
            LIMIT 
                @Limit
            OFFSET 
                @Offset;
        ";

        public const string Insert = @"
            INSERT INTO blueprints (
                id,
                name, 
                description,
                created_at,
                updated_at,
                metadata
            ) VALUES (
                @Id,
                @Name,
                @Description,
                @CreatedAt,
                @UpdatedAt,
                @Metadata::jsonb
            ) RETURNING 
                id, 
                name, 
                description, 
                created_at, 
                updated_at, 
                metadata;
        ";

        public const string Update = @"
            UPDATE 
                blueprints
            SET 
                name = @Name,
                description = @Description,
                updated_at = @UpdatedAt,
                metadata = @Metadata::jsonb
            WHERE 
                id = @Id
            RETURNING 
                id, 
                name, 
                description, 
                created_at, 
                updated_at, 
                metadata;
        ";

        public const string Delete = @"
            DELETE FROM
                blueprints
            WHERE 
                id = @Id;
        ";

        public const string Count = @"
            SELECT 
                COUNT(*)
            FROM 
                blueprints;
        ";

        public const string HealthCheck = @"
            SELECT 1;
        ";
    }
}
