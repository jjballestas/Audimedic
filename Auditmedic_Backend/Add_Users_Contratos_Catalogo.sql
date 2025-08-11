IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [NombreRol] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

CREATE TABLE [Usuarios] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [ContrasenaHash] nvarchar(max) NULL,
    [Activo] bit NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [RolId] int NOT NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Usuarios_Roles_RolId] FOREIGN KEY ([RolId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ProveedoresExternos] (
    [Id] int NOT NULL IDENTITY,
    [Proveedor] nvarchar(max) NOT NULL,
    [IdExterno] nvarchar(max) NOT NULL,
    [EmailConfirmado] bit NOT NULL,
    [FotoUrl] nvarchar(max) NULL,
    [UsuarioId] int NOT NULL,
    CONSTRAINT [PK_ProveedoresExternos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProveedoresExternos_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_ProveedoresExternos_UsuarioId] ON [ProveedoresExternos] ([UsuarioId]);

CREATE UNIQUE INDEX [IX_Usuarios_Email] ON [Usuarios] ([Email]);

CREATE INDEX [IX_Usuarios_RolId] ON [Usuarios] ([RolId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250805000610_InicialAutenticacion', N'9.0.7');

IF SCHEMA_ID(N'security') IS NULL EXEC(N'CREATE SCHEMA [security];');

ALTER SCHEMA [security] TRANSFER [Usuarios];

ALTER SCHEMA [security] TRANSFER [Roles];

ALTER SCHEMA [security] TRANSFER [ProveedoresExternos];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250805103319_EsquemaSecurity', N'9.0.7');

IF SCHEMA_ID(N'users') IS NULL EXEC(N'CREATE SCHEMA [users];');

CREATE TABLE [users].[HistoriasClinicas] (
    [Id] int NOT NULL IDENTITY,
    [NumeroHistoria] nvarchar(max) NOT NULL,
    [ArchivoUrl] nvarchar(max) NOT NULL,
    [FechaCarga] datetime2 NOT NULL,
    [FechaCirugia] datetime2 NOT NULL,
    [HoraCirugia] time NOT NULL,
    [TiempoQuirurgico] int NOT NULL,
    [TiempoAnestesico] int NOT NULL,
    [Sala] nvarchar(max) NOT NULL,
    [Estado] nvarchar(max) NOT NULL,
    [MedicoId] int NOT NULL,
    CONSTRAINT [PK_HistoriasClinicas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HistoriasClinicas_Usuarios_MedicoId] FOREIGN KEY ([MedicoId]) REFERENCES [security].[Usuarios] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_HistoriasClinicas_MedicoId] ON [users].[HistoriasClinicas] ([MedicoId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250805105309_Users_HistoriaClinica', N'9.0.7');

ALTER TABLE [users].[HistoriasClinicas] DROP CONSTRAINT [FK_HistoriasClinicas_Usuarios_MedicoId];

IF SCHEMA_ID(N'contratos') IS NULL EXEC(N'CREATE SCHEMA [contratos];');

IF SCHEMA_ID(N'catalogo') IS NULL EXEC(N'CREATE SCHEMA [catalogo];');

ALTER TABLE [users].[HistoriasClinicas] ADD [EntidadId] int NOT NULL DEFAULT 0;

CREATE TABLE [users].[Entidades] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [TipoEntidad] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Entidades] PRIMARY KEY ([Id])
);

CREATE TABLE [users].[Medicos] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    CONSTRAINT [PK_Medicos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Medicos_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [security].[Usuarios] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [catalogo].[Procedimientos] (
    [Id] int NOT NULL IDENTITY,
    [CodigoCUPS] nvarchar(450) NOT NULL,
    [Descripcion] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Procedimientos] PRIMARY KEY ([Id])
);

CREATE TABLE [catalogo].[UVB] (
    [Id] int NOT NULL IDENTITY,
    [Anio] int NOT NULL,
    [Valor] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_UVB] PRIMARY KEY ([Id])
);

CREATE TABLE [contratos].[MedicosEntidades] (
    [Id] int NOT NULL IDENTITY,
    [MedicoId] int NOT NULL,
    [EntidadId] int NOT NULL,
    [FechaInicio] datetime2 NULL,
    [FechaFin] datetime2 NULL,
    CONSTRAINT [PK_MedicosEntidades] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MedicosEntidades_Entidades_EntidadId] FOREIGN KEY ([EntidadId]) REFERENCES [users].[Entidades] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MedicosEntidades_Medicos_MedicoId] FOREIGN KEY ([MedicoId]) REFERENCES [users].[Medicos] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [users].[ProcedimientosHistoria] (
    [Id] int NOT NULL IDENTITY,
    [HistoriaClinicaId] int NOT NULL,
    [ProcedimientoId] int NOT NULL,
    [EsPrincipal] bit NOT NULL,
    [EsInherente] bit NOT NULL,
    [Bilateral] bit NOT NULL,
    [ViaQuirurgica] nvarchar(max) NOT NULL,
    [ValorCalculado] decimal(18,2) NULL,
    CONSTRAINT [PK_ProcedimientosHistoria] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProcedimientosHistoria_HistoriasClinicas_HistoriaClinicaId] FOREIGN KEY ([HistoriaClinicaId]) REFERENCES [users].[HistoriasClinicas] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProcedimientosHistoria_Procedimientos_ProcedimientoId] FOREIGN KEY ([ProcedimientoId]) REFERENCES [catalogo].[Procedimientos] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [catalogo].[TarifasSOAT] (
    [Id] int NOT NULL IDENTITY,
    [ProcedimientoId] int NOT NULL,
    [Anio] int NOT NULL,
    [Valor] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_TarifasSOAT] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TarifasSOAT_Procedimientos_ProcedimientoId] FOREIGN KEY ([ProcedimientoId]) REFERENCES [catalogo].[Procedimientos] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [contratos].[Contratos] (
    [Id] int NOT NULL IDENTITY,
    [MedicoEntidadId] int NOT NULL,
    [ManualTarifario] nvarchar(max) NOT NULL,
    [PorcentajeAjuste] decimal(18,2) NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_Contratos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Contratos_MedicosEntidades_MedicoEntidadId] FOREIGN KEY ([MedicoEntidadId]) REFERENCES [contratos].[MedicosEntidades] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [contratos].[TarifasContrato] (
    [Id] int NOT NULL IDENTITY,
    [ContratoId] int NOT NULL,
    [ProcedimientoId] int NOT NULL,
    [ValorFijo] decimal(18,2) NULL,
    [FactorPorcentaje] decimal(18,2) NULL,
    CONSTRAINT [PK_TarifasContrato] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TarifasContrato_Contratos_ContratoId] FOREIGN KEY ([ContratoId]) REFERENCES [contratos].[Contratos] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TarifasContrato_Procedimientos_ProcedimientoId] FOREIGN KEY ([ProcedimientoId]) REFERENCES [catalogo].[Procedimientos] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_HistoriasClinicas_EntidadId] ON [users].[HistoriasClinicas] ([EntidadId]);

CREATE INDEX [IX_Contratos_MedicoEntidadId] ON [contratos].[Contratos] ([MedicoEntidadId]);

CREATE INDEX [IX_Medicos_UsuarioId] ON [users].[Medicos] ([UsuarioId]);

CREATE INDEX [IX_MedicosEntidades_EntidadId] ON [contratos].[MedicosEntidades] ([EntidadId]);

CREATE INDEX [IX_MedicosEntidades_MedicoId] ON [contratos].[MedicosEntidades] ([MedicoId]);

CREATE UNIQUE INDEX [IX_Procedimientos_CodigoCUPS] ON [catalogo].[Procedimientos] ([CodigoCUPS]);

CREATE INDEX [IX_ProcedimientosHistoria_HistoriaClinicaId] ON [users].[ProcedimientosHistoria] ([HistoriaClinicaId]);

CREATE INDEX [IX_ProcedimientosHistoria_ProcedimientoId] ON [users].[ProcedimientosHistoria] ([ProcedimientoId]);

CREATE INDEX [IX_TarifasContrato_ContratoId] ON [contratos].[TarifasContrato] ([ContratoId]);

CREATE INDEX [IX_TarifasContrato_ProcedimientoId] ON [contratos].[TarifasContrato] ([ProcedimientoId]);

CREATE INDEX [IX_TarifasSOAT_ProcedimientoId] ON [catalogo].[TarifasSOAT] ([ProcedimientoId]);

ALTER TABLE [users].[HistoriasClinicas] ADD CONSTRAINT [FK_HistoriasClinicas_Entidades_EntidadId] FOREIGN KEY ([EntidadId]) REFERENCES [users].[Entidades] ([Id]) ON DELETE CASCADE;

ALTER TABLE [users].[HistoriasClinicas] ADD CONSTRAINT [FK_HistoriasClinicas_Medicos_MedicoId] FOREIGN KEY ([MedicoId]) REFERENCES [users].[Medicos] ([Id]) ON DELETE CASCADE;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250811201213_Add_Users_Contratos_Catalogo', N'9.0.7');

COMMIT;
GO

