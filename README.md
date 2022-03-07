# Heart-Diseases-Diagnostic-WEB-API
WEB API для диагностирования сердечно-сосудистых заболеваний с использованием алгоритмов искусственного интеллекта.

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

## HTTP методы:
1) /diagnose: Основной метод. Служит для самого диагностирования. На вход принимает [Required][FromQuery] AlgorithmsTypes algorithm, [Required][FromQuery] DataSetTypes dataSetType, [FromBody] JsonDocument data, [Required][FromHeader] string requestId.
    Структура data должна совпадать со структурой в примерах.
    В ответ выдает ActionResponse:
```json
{
  "answer": "OK",
  "requestId": "string",
  "value": 0
}
```
2) /cleveland-example: Отвечает примером класса ClevelandDataSet.На вход ничего не принимает. В ответ выдает ClevelandDataSet:
```json
{
  "age": 0,
  "sex": true,
  "chestPainType": 0,
  "restingBloodPressure": 0,
  "serumCholestoral": 0,
  "fastingBloodSugar": true,
  "restingElectrocardiographicResults": 0,
  "maximumHeartRateAchieved": 0,
  "exerciseInducedAngina": true,
  "stDepression": 0,
  "stSlope": 0,
  "numberOfMajorvessels": 0,
  "thalassemia": 0
}
```
3) /ping: Служит для проверки наличия связи. В данный момент бесполезен. На вход ничего не получает. В ответ всегда отвечает "Pong".

## Используемые датасеты:
1) https://www.kaggle.com/fedesoriano/heart-failure-prediction
2) https://www.kaggle.com/andrewmvd/heart-failure-clinical-data
3) https://www.kaggle.com/sulianova/cardiovascular-disease-dataset
4) https://www.kaggle.com/yassinehamdaoui1/cardiovascular-disease
5) https://www.kaggle.com/ronitf/heart-disease-uci

