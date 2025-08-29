migrationBuilder.CreateTable(
    name: "Tarefas",
    columns: table => new
    {
        Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
        Titulo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
        Descricao = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
        Data = table.Column<DateTime>(type: "TEXT", nullable: false),
        Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Tarefas", x => x.Id);
    });
