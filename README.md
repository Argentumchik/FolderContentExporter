# FolderContentExporter

A lightweight WPF desktop app for scanning a folder's contents and exporting the file list to TXT, CSV, or JSON — with full control over which fields get exported.

## Features

- **Folder scanning** — pick any folder and list its files, with an optional recursive scan of subfolders.
- **Async & cancellable** — scanning runs off the UI thread with live progress reporting; Cancel actually stops an in-progress scan instead of just hiding it.
- **Sortable file list** — browse results in a DataGrid: name, size, extension, path, created and modified dates.
- **Configurable export** — export to **TXT**, **CSV**, or **JSON**, choosing exactly which fields to include (name, size, extension, path, created date, modified date) and the output file name.
- **Friendly error handling** — failures (access denied, missing path, file in use, etc.) are mapped to clear messages instead of raw exception text.
- **Consistent state handling** — the app tracks a single operation state under the hood, so you can't start a new scan while one is running, can't export before a scan finishes, and cancelling a folder picker just quietly aborts instead of throwing an error.

## Tech stack

- .NET 10
- WPF (MVVM)
- Microsoft.Extensions.DependencyInjection
- CsvHelper
- System.Text.Json

## Architecture

The app follows a fairly standard MVVM + DI structure:

| Folder | Contains |
|---|---|
| `ViewModels/` | `MainViewModel` (scanning & export orchestration), `ExportDialogViewModel` (export options) |
| `Services/` | `FileSystemService` (async recursive scanning), `FileExportService` (TXT/CSV/JSON writers), `FolderDialogService` (folder picker), `ErrorMapper` (exception → user-facing message) |
| `Interfaces/` | One interface per service above, so each can be swapped or mocked independently |
| `Dto/` | Plain data objects passed between layers (`ExportOptionsDto`, `AppError`, ...) |
| `View/` | `MainWindow`, `ExportDialogWindow` |

Services and view models are registered in a DI container and composed in `App.xaml.cs`.

## How to use

1. Click **Open folder** and pick a folder to scan.
2. Optionally check **Include sub** for a recursive scan of subfolders.
3. Click **Load files** — the progress bar and file count update live; **Cancel** stops the scan at any point.
4. Once loading finishes, click **Export**, pick which fields to include, choose a format (TXT / CSV / JSON) and a file name.
5. Pick a destination folder — the file list is saved there.

## Building from source

Requires the **.NET 10 SDK** and **Windows** (WPF doesn't build or run cross-platform).

```bash
git clone https://github.com/Argentumchik/FolderContentExporter.git
cd FolderContentExporter
dotnet build FolderContentExporter.slnx
```

A GitHub Actions workflow (`.github/workflows/build.yml`) builds the project on every push/PR to `master` and `dev`.

## Status

Actively developed. `master` holds the latest stable build; day-to-day work happens on `dev` before being merged.

## License

MIT — see [LICENSE.txt](LICENSE.txt).
