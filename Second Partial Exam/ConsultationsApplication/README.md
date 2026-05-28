# Систем за управување со термини за консултации
### Втор колоквиум — Интегрирани системи

---

## Контекст на апликацијата

Постоечкиот систем за управување со консултации е имплементиран во **ASP.NET Core Web API** со **Onion архитектура**. Системот поддржува CRUD операции за термини за консултации `Consultation` и управување со присуства `Attendance`. Задачата е да се прошири постоечката апликација со нови функционалности поврзани со конфигурација, кеширање и надворешна интеграција.

---

## Барање 1 — Конфигурација за различни околини

Апликацијата треба да поддржува три конфигурациски околини: **Development**, **Staging** и **Production**.

### Strongly-typed settings класи

| Класа | Полиња | Development | Staging | Production |
|---|---|---|---|---|
| `CacheSettings` | `ListCacheDurationMinutes` (int)<br>`DetailCacheDurationMinutes` (int) | `10`<br>`15` | `20`<br>`25` | `60`<br>`60` |
| `RateLimitSettings` | `PermitLimit` (int)<br>`WindowInSeconds` (int)<br>`Apply` | `10`<br>`150`<br>`true` | `50`<br>`60`<br>`true` | `100`<br>`60`<br>`true` |
| `ApiKeySettings` | `ApiKey` (string) | `HyeORCiubWiUO4E1m1h3dGPjPKWhND1f` | `rVRF58bzXD00bxIhQin2NCozkapmVRQy` | `Cnp8tzHRbwNQCgrBadTLBtnRvZtBcDYC` |

---

## Барање 2 — Кеширање на резултати

Имплементирај кеширање на резултати за следниот endpoint:

```
GET /api/consultation?roomName={roomName}&date={date}
```

### Правила

- Клучот мора да ги вклучува вредностите на query параметрите:
  ```
  consultations:{roomName}:{date}
  ```
- Времето на чување се чита од `CacheSettings` конфигурацијата.
- Кеширањето се имплементира во **сервисниот слој**.

---

## Барање 3 — Inbound REST API со API Key автентикација

Имплементирај надворешни (`external`) endpoint-и заштитени со **API Key автентикација** и **Rate Limiting**.

### Middleware логика

- Се применува само на патеки кои почнуваат со `/api/external`.
- Ако header-от недостасува → `401 Unauthorized` со порака: `API key is required`
- Ако клучот е невалиден → `401 Unauthorized` со порака: `Invalid API key`

---

### `POST /api/external/attendance/register`
> Пријавување на термин за консултации

Поради очекуван голем број повици, се имплементира **асинхроно процесирање** во интервал од **1 минута**.

**Request Body:**

| Поле | Тип | Опис |
|---|---|---|
| `ConsultationId` | required | ID на консултацијата |
| `UserId` | required | ID на корисникот |
| `RoomId` | required | ID на просторијата |
| `Comment` | optional | Коментар |

**Response:** `202 Accepted`

```json
{
  "id": "...",
  "status": "..."
}
```

---

### `GET /api/external/attendance/register/{id}/status`
> Статус на барањето за пријавување

**Response:**

```json
{
  "id": "...",
  "status": "...",
  "error": "..."
}
```

---

## Барање 4 — Rate Limiting

Rate Limiting се применува на сите `/api/external` endpoint-и според `RateLimitSettings` конфигурацијата.

### Правила

- Лимитот се применува **по API клуч** — секој клиент има свој независен бројач.
- При надминување на лимитот се враќа `429 Too Many Requests`.

---

## Барање 5 — ETL: Инкрементален import преку надворешно API

Имплементирај функционалност за **инкрементален ETL import** на термини за консултации од надворешно REST API.

### Компоненти

- **Strongly-typed HTTP client** кој го повикува надворешното API.
- **Сервис** кој ги трансформира податоците и ги запишува во локалната база — без дуплирање на постоечките записи.
- **BackgroundService** кој го извршува процесот на секои **5 минути**.
- Секое извршување се логира во ентитетот `EtlSyncLog`.

### Надворешно API

| | |
|---|---|
| **Документација** | https://integriranisistemi.finki.ukim.mk/docs |
| **Header** | `X-Api-Key` |
| **Вредност** | `gSAOEjaqdZW3MhlJL4miLerblYwlpq9W` |
