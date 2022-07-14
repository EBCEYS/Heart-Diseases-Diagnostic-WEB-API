# Heart-Diseases-Diagnostic-WEB-API
WEB API для диагностирования сердечно-сосудистых заболеваний с использованием алгоритмов искусственного интеллекта.

## Описание конфигурационного файла:
```json
{
    "AllowedHosts": "*",
  "CheckAllowedLocalMachinesPeriod": 5, //период опроса машин с ai-flask-server-ом в секундах
  "AllowedIPs": [ //ip машин с ai-flask-server-ом. При использовании распределителя нагрузки - указать его ip.
    "127.0.0.1:8800"
  ],
  "UseRabbitMQ": true, // если true, то вариант использования решения с RabbitMQ. Иначе запросы будут слаться на вышеуказанные адреса.
  "RabbitMQSettings": { // настройки в случае использования RabbitMQ
    "Queue": "rpc_queue", // имя очереди
    "HostName": "rabbitmq", // адресс
    "Port": 5672, // порт
    "UserName": "RestServer", // имя
    "Password": "Gizmo", // пароль
    "Timeout": 20 // таймут ожидания ответа на запрос в секундах
  }
}
```

## HTTP методы:
1) /diagnose: Основной метод. Служит для самого диагностирования. На вход принимает [Required][FromPath] AlgorithmsTypes algorithm, [Required][FromPath] DataSetTypes dataSetType, [FromBody] JsonDocument data, [Required][FromHeader] string requestId.
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

## Варианты использования решения
### Вариант 1
Используемое ПО:
1. [Restserver](https://github.com/EBCEYS/Heart-Diseases-Diagnostic-WEB-API) (в процессе разработки);
2. RabbitMQ;
3. [RabbitMQToHTTPLoadBalancingService](https://github.com/EBCEYS/RabbitMQToHTTPLoadBalancingService) (в процессе разработки);
4. [AIServer](https://github.com/EBCEYS/ai-flask-server) (в процессе разработки);
5. DBAdapterService (в процессе разработки);
6. PostgreSQL база данных.

![First variant](https://github.com/EBCEYS/Heart-Diseases-Diagnostic-WEB-API/blob/main/ProjectScheme1.png)
### Вариант 2
Используемое ПО:
1. [Restserver](https://github.com/EBCEYS/Heart-Diseases-Diagnostic-WEB-API) (в процессе разработки);
2. CustomHTTPLoadDistributor (в процессе разработки);
3. RabbitMQ;
4. [RabbitMQToHTTPLoadBalancingService](https://github.com/EBCEYS/RabbitMQToHTTPLoadBalancingService) (в процессе разработки);
5. [AIServer](https://github.com/EBCEYS/ai-flask-server) (в процессе разработки);
6. DBAdapterService (в процессе разработки);
7. PostgreSQL база данных.

![Second variant](https://github.com/EBCEYS/Heart-Diseases-Diagnostic-WEB-API/blob/main/ProjectScheme2.png)
### Вариант 3
Используемое ПО:
1. [Restserver](https://github.com/EBCEYS/Heart-Diseases-Diagnostic-WEB-API) (в процессе разработки);
2. CustomHTTPLoadBalancer (в процессе разработки);
3. [AIServer](https://github.com/EBCEYS/ai-flask-server) (в процессе разработки);

![Third variant](https://github.com/EBCEYS/Heart-Diseases-Diagnostic-WEB-API/blob/main/ProjectScheme3.png)

Описание настроек ПО для каждого варианта будет описано после завершения разработки.

## Используемые датасеты:
1. https://www.kaggle.com/fedesoriano/heart-failure-prediction
2. https://www.kaggle.com/andrewmvd/heart-failure-clinical-data
3. https://www.kaggle.com/sulianova/cardiovascular-disease-dataset
4. https://www.kaggle.com/yassinehamdaoui1/cardiovascular-disease
5. https://www.kaggle.com/ronitf/heart-disease-uci

