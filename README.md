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
```
## Используемые датасеты:
1) https://www.kaggle.com/fedesoriano/heart-failure-prediction
2) https://www.kaggle.com/andrewmvd/heart-failure-clinical-data
3) https://www.kaggle.com/sulianova/cardiovascular-disease-dataset
4) https://www.kaggle.com/yassinehamdaoui1/cardiovascular-disease
5) https://www.kaggle.com/ronitf/heart-disease-uci

