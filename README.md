# 🎮 FIAP Cloud Games – Payment Processing Function

**Azure Function** responsável por processar eventos de pedidos e orquestrar a criação de pagamentos, integrando o **Azure Service Bus** com a **API de Payments**, acessada via **Azure API Management (APIM)**.

## 🚀 Tech Challenge – FIAP (Fase 3)

Este projeto integra o FIAP Cloud Game apresentado como Tech Challenge do curso de pós-graduação em Arquitetura de Sistemas .NET

## 🧩 Visão Geral da Solução

A Function é disparada automaticamente sempre que um evento de pedido é publicado na fila do Azure Service Bus.

### 🔄 Fluxo de Processamento

  > 1 → A API de Orders / Games publica um evento na fila order-created

  > 2 → O Azure Service Bus armazena a mensagem

  > 3 → A Azure Function consome o evento

  > 4 → A Function chama a Payments API via Azure API Management

  > 5 → A API de Payments valida, aprova ou recusa o pagamento e persiste os dados

## 🧪 Configuração Local

Crie o arquivo `local.settings.json` na raiz do projeto:

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "SERVICEBUS_CONNECTION": "<service-bus-connection-string>"
  },
  "FunctionOptions": {
    "BaseUrl": "https://fcg-apigtw.azure-api.net/payments-api",
    "ApimSubscriptionKey": "<apim-subscription-key>"
  }
}
```

## 🌐 Variáveis de Ambiente (Azure)

As seguintes variáveis devem ser configuradas no App Configuration da Function:

* SERVICEBUS_CONNECTION

* FunctionOptions__BaseUrl

* FunctionOptions__ApimSubscriptionKey

## ▶️ Executando Localmente

```bash
func start
```
A Function ficará escutando a fila configurada no Service Bus e processará automaticamente as mensagens recebidas.

## 🛠️ Tecnologias Utilizadas

* ⚡ .NET 8 (Isolated Worker)

* ☁️ Azure Functions

* 📨 Azure Service Bus

* 🌐 Azure API Management (APIM)

* 🔁 HttpClient Factory
