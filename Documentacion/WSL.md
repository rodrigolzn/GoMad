# Uso en WSL

## Estado del workspace

- La solucion principal de VS Code y .NET en WSL es `GoMad.sln`.
- El proyecto web `GoMad.csproj` compila y ejecuta en Linux con .NET 8.
- El proyecto movil `GoMad.Mobile` queda en modo placeholder cuando se abre o compila desde Linux/WSL para evitar errores de carga y build.

## Flujo recomendado en VS Code con WSL

1. Abre esta carpeta desde la extension Remote - WSL.
2. Trabaja y ejecuta el proyecto web con `dotnet restore GoMad.sln` y `dotnet build GoMad.sln`.
3. Si necesitas compilar la app movil, hazlo fuera de WSL en Windows o macOS con los workloads de .NET MAUI instalados.

## Integracion con VS Code

- Se configuro `GoMad.sln` como solucion por defecto del workspace.
- Puedes ejecutar la tarea `run web` desde VS Code para levantar la aplicacion web en `http://127.0.0.1:5057`.
- Tambien puedes usar la configuracion de depuracion `Launch GoMad Web`.

## Comandos utiles

```bash
dotnet restore GoMad.sln
dotnet build GoMad.sln
dotnet run --project GoMad.csproj
```