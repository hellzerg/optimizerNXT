## ⚡ OptimizerNXT

### The finest Windows Optimizer CLI

OptimizerNXT is a **powerful, YAML-driven CLI tool** for automating Windows tweaks, managing system services, controlling processes, and optimizing workflows—all from the command line.  

<img src="https://raw.githubusercontent.com/hellzerg/optimizerNXT/refs/heads/main/images/banner.svg">

---

## 🌟 Summary

OptimizerNXT empowers system administrators, IT professionals, and power users to automate **Windows tweaks, network configuration, startup management, service control, process monitoring, and script execution**, all using **secure, YAML-based configurations**.  

> “Automate. Secure. Optimize. Next-level.”

---

## 🚀 Key Features

| Icon | Feature | Description |
|------|--------|-------------|
| 🗄️ | **Registry Management** | Add, update, or delete registry keys and values efficiently. |
| 🌐 | **DNS Control** | Switch between multiple DNS configurations quickly. |
| ⚡ | **Startup & Hosts Editing** | Modify startup programs and hosts file safely. |
| 🧹 | **UWP App Removal** | Batch uninstall unwanted Windows apps. |
| 🔒 | **Process Control** | Block, terminate, or allow processes with advanced conditions. |
| 🛠️ | **Service Management** | Start, stop, disable, enable, or set demand-start services. |
| 🖥️ | **Shell Execution** | Run CMD, PowerShell, or REG scripts inline or from files. |
| 📄 | **Conditional Operations** | Apply tweaks based on system state, user, or environment. |
| 🔐 | **Signed YAML Enforcement** | Only executes YAML files signed with trusted certificate. |

---

## 📥 Downloads

Find the latest release of OptimizerNXT on the [Releases](https://github.com/hellzerg/optimizerNXT/releases) page.

---

## 📥 Changelog

Stay updated with the latest changes through the [Changelog](https://github.com/hellzerg/optimizerNXT/blob/main/CHANGELOG.md).

---

## 📚 Documentation 

OptimizerNXT uses **YAML-driven configurations** with support for:  

- Multiple operations per YAML  
- Conditional execution based on environment or system state  
- Sequenced tasks with dependencies  
- Only YAML files **signed with the repo's PFX certificate** are executed. Unsigned or tampered files will be rejected. 

> ⚠ **IMPORTANT:** Each YAML file must have a corresponding `.sig` file with the **same filename** in the same location.  

### Example Usage
```powershell
# Run a specific YAML configuration
optimizerNXT.exe apply "<yaml_file>"

# Run a folder containing many YAML files
optimizerNXT.exe apply "<yaml_folder>"
```

---

## 🏢 Hub for YAML Packages

OptimizerNXT will feature a **centralized Hub** for hosting curated YAML packages for various automation tasks, including:  

- Performance optimization  
- Privacy & security tweaks  
- System cleanup  
- Application management  
- Network configuration  

All YAML packages in the Hub will be **cryptographically signed**, ensuring authenticity and safety for execution.  

---

## 🛡️ Security Policy

- OptimizerNXT executes **locally only** and does **not collect user data**.  
- All YAML configurations must be signed with repo's trusted **PFX certificate**.  
- Unsigned or invalid YAML files are automatically rejected.  
- Users are encouraged to test YAML packages in isolated environments first.  

Learn about our security measures in the [Security Policy](https://github.com/hellzerg/optimizerNXT/blob/main/SECURITY.md).

---

## 💻 Compatibility

- Requires .NET Framework 4.8
- Compatible with Windows 7, 8, 8.1, 10, 11
- Also compatible with all Windows Server editions starting from 2008

---

## 📊 Details

- Latest version: v1.0.0
- Released: 2026-Jan-26

---

## ❤️ Buy me a delicious espresso ☕

If you find this tool useful, consider showing your support by [donating through PayPal](https://www.paypal.com/paypalme/supportoptimizer).
