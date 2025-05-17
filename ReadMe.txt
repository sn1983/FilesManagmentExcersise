# Solution Documentation

## Overview

This solution is a file-based communication and monitoring system built on .NET Framework 4.7.2. It consists of two Windows Services (TX and RX), two configuration UIs (for TX and RX), and a service installer UI. The system is designed for robust, auditable file transfer and event signaling between two endpoints, with logging and notification capabilities.

---

## Components

### 1. TX Service

**Purpose:**  
- Periodically creates "flag" files in a source folder.
- Monitors the source folder and transmits (moves) any file—including flag files—to a destination folder.

**Key Features:**
- **Flag File Creation:**  
  Every 5 seconds, a flag file is generated in the source folder. The flag file contains a timestamp and the service name.
- **File Monitoring and Transfer:**  
  Uses a `FileSystemWatcher` to listen for new or changed files in the source folder. When a file is detected, it is moved to the destination folder.
- **Logging:**  
  Events are logged using Serilog and optionally to the Windows Event Log, based on configuration.
- **Configuration:**  
  Settings (source folder, destination folder, logging options) are loaded from a JSON file, with a UI provided for editing these settings.

---

### 2. RX Service

**Purpose:**  
- Monitors the destination folder for incoming files.
- Deletes flag files upon detection, logs the event, and sends an email notification.

**Key Features:**
- **File Monitoring:**  
  Uses a `FileSystemWatcher` to listen for new or changed files in the destination folder.
- **Flag File Handling:**  
  When a flag file is detected (based on its content/format), it is deleted.
- **Logging and Notification:**  
  Each flag file deletion is logged (Serilog and Event Log) and triggers an SMTP email notification.
- **Configuration:**  
  Settings (destination folder, SMTP server, email addresses, logging options) are loaded from a JSON file, with a UI provided for editing these settings.

---

### 3. Service Installer UI

**Purpose:**  
- Provides a simple WPF-based interface to install or uninstall the TX and RX Windows Services.

**Key Features:**
- **Service Selection:**  
  User can select either TX or RX service for installation/uninstallation.
- **Automated Installation:**  
  Uses `ManagedInstallerClass.InstallHelper` to install or uninstall the selected service.
- **Service Discovery:**  
  Automatically locates the service executable and retrieves the service name for management.

---

### 4. TX Service UI

**Purpose:**  
- WPF application for configuring TX service settings.

**Key Features:**
- **Edit and Save Settings:**  
  Allows the user to set the source folder, destination folder, and logging options.
- **Validation:**  
  Ensures required fields are filled before saving.
- **Persistence:**  
  Saves settings to a JSON file, which the TX service reads on startup.

---

### 5. RX Service UI

**Purpose:**  
- WPF application for configuring RX service settings.

**Key Features:**
- **Edit and Save Settings:**  
  Allows the user to set the destination folder, SMTP server, sender/recipient email, and logging options.
- **Validation:**  
  Ensures all required fields (including valid email addresses and port) are filled before saving.
- **Persistence:**  
  Saves settings to a JSON file, which the RX service reads on startup.

---

## Data Flow

1. **TX Service** creates a flag file in the source folder every 5 seconds.
2. **TX Service** monitors the source folder and moves any file (including flag files) to the destination folder.
3. **RX Service** monitors the destination folder. When a flag file is detected, it deletes the file, logs the event, and sends an email notification.
4. Both services can be installed/uninstalled using the Service Installer UI.
5. Both services have dedicated UIs for editing and saving their configuration in JSON format.

---

## Configuration

- **TX Settings:**  
  - SourceFolder (string)
  - DestinationFolder (string)
  - SysLog (bool)

- **RX Settings:**  
  - DestinationFolder (string)
  - SysLog (bool)
  - SmtpServer (string)
  - Port (int)
  - UseSsl (bool)
  - SenderEmail (string)
  - RecpientEmail (string)
  - SenderName (string)
  - Username (string)
  - Password (string)
  - logFolder (string)

All settings are persisted in JSON files, which are loaded by the services at startup.

---

## Logging

- **Serilog** is used for file-based logging.
- Optionally, events are also written to the Windows Event Log if enabled in settings.
- RX service logs flag file deletions and sends email notifications.

---

## Installation

- Use the Service Installer UI to install or uninstall the TX and RX services.
- The installer locates the correct executable and registers the service with Windows.

---

## Extensibility

- The solution is modular, with clear separation between service logic, configuration, and UI.
- JSON-based configuration allows for easy editing and deployment.
- Logging and notification mechanisms can be extended as needed.

---

## Summary

This solution provides a robust, configurable, and auditable mechanism for file-based signaling and transfer between two endpoints, with full support for logging, notification, and easy deployment/management via graphical UIs.
