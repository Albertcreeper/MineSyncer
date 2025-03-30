# 🌍 MineSyncer

MineSyncer is a simple C# WPF utility designed to synchronize a local Minecraft world between two systems (e.g., a desktop PC and a laptop) via a cloud storage folder like OneDrive, Dropbox, or Google Drive.

It compares the local world with a remote copy (stored in a synced cloud directory) and determines whether to push changes to the remote or pull the latest version from it.

> 🚧 This tool was built for personal use and currently supports only a single Minecraft world. Support for multiple worlds and a new .NET MAUI interface is planned.


## 🚀 What It Does

- Push (Local to Remote): If the local world is newer, the changed files are copied to the remote folder.

- Pull (Remote to Local): If the remote world is newer, the updated files are copied to the local Minecraft directory.

- Backups: Optional backups are created before overwriting files during sync.


## 🔧 Configuration Overview

App Configuration File (App.config)

``` xml
<configuration>
  <appSettings>
    <RepositoryName>Ananastown</RepositoryName>
    <RepositoriesPath>C:\Users\alber\...\MineSyncer.ini</RepositoriesPath>
    <LogConfigPath>log4net.config</LogConfigPath>
  </appSettings>
</configuration>
```
- RepositoryName: The name of the Minecraft world.
- RepositoriesPath: Path to the INI file containing local world configuration.
- LogConfigPath: Path to the logging configuration (e.g., log4net).

Local INI File Example (MineSyncer.ini)
``` ini
[Ananastown]
Name=Ananastown
Path=C:\Users\alber\...\minecraftWorlds\nIwYANKBAQA=
RemoteRepositoryConfigPath=C:\Users\alber\OneDrive\Savegames\Minecraft\MineSyncer.ini
Variables=%REMOTEPATH%=C:\Users\alber\OneDrive\Savegames\Minecraft
CreateBackup=True
BackupPath=D:\Backup\Minecraft\Ananastown
Version=33
ProcessName=Minecraft.Windows
StartCommandLine=& "...\Minecraft.Windows.exe"
```

Remote INI File Example
``` ini
[Ananastown]
Name=Ananastown
RepositoryType=File
Path=%REMOTEPATH%\nIwYANKBAQA=
BackupPath=%REMOTEPATH%\Backups\Ananastown
CreateBackup=True
Version=33
```

## 🚑 How It Works

1. MineSyncer compares the Version values of the local and remote repositories.
2. If the local version is higher → Push to remote.
3. If the remote version is higher → Pull to local.
4. Backups are created before overwriting if enabled.
5. The tool resolves variables like %REMOTEPATH% using values in the configuration.

## 🪤 Future Plans

- ✨ .NET MAUI UI with support for multiple Minecraft worlds.
- World management panel with sync history and manual override options.
- Improved status messages and automatic detection of Minecraft save directories.

## 📚 Tech Stack

- Language: C#
- Framework: .NET (WPF)
- Configuration: .ini + App.config
- Logging: log4net

## 📌 Disclaimer

MineSyncer is a personal utility created for synchronizing a Minecraft world across devices using a shared cloud storage folder. It is not production-ready and may require adjustments for other environments or setups.

Use at your own risk and don't forget to back up your world!