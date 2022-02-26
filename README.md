# Heart-Diseases-Diagnostic-WEB-API
This is a WEB API used to diagnose cardiovascular diseases using AI algorithms.

## Описание конфигурационного файла:
```json
{
  "Logging": { //логирование
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "CheckAllowedLocalMachinesPeriod": 30, //период опроса доступных локальных машин из списка AllowedIPs
  "AllowedIPs": [ //список ip адресов локальных машин через запятую
    "127.0.0.1:5000" // пример
  ]
}

