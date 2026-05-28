
# Систем за управување со работилници (Workshops)
## Втор колоквиум — Интегрирани системи (Вежба)

Стартер код: [WorkshopApplicationStarter.zip](https://github.com/user-attachments/files/28350159/WorkshopApplicationStarter.zip)


---

## Контекст на апликацијата

Постоечкиот систем за управување со работилници е имплементиран во ASP.NET Core Web API со Onion архитектура (Domain → Repository → Service → Web). Системот поддржува CRUD операции за работилници (`Workshop`) и управување со записи за учество (`Enrollment`). Ваша задача е да ја проширите постоечката апликација со нови функционалности поврзани со конфигурација, кеширање, надворешна интеграција, испраќање email и Excel import/export според барањата подолу.

### Архитектура на проектот

```
Domain/          — Модели, DTO-а, Enums, Config класи
Repository/      — ApplicationDbContext, IRepository<T>, Repository<T>
Service/         — Бизнис логика, интерфејси и имплементации
Web/             — Контролери, Middleware, Mapper, Program.cs
TestExamIS.Tests/ — Тестови кои ја валидираат вашата имплементација
```

### Модели

| Модел | Опис |
|-------|------|
| `Workshop` | Работилница со Title, StartTime, EndTime, VenueId, RegisteredParticipants |
| `Enrollment` | Запис за учество: Notes, EnrollmentStatus, UserId, VenueId, WorkshopId |
| `Venue` | Просторија: Name, Capacity |
| `Assignment` | Доделување на корисник на работилница |
| `InboundEventEntry` | Запис за асинхроно процесирање на надворешни барања |
| `EtlSyncLog` | Евиденција за ETL извршувања |
| `WorkshopApplicationUser` | Корисник (Identity) со FirstName, LastName, Role |

---

## Барање 1 — Конфигурација за различни околини (21 поени)

Апликацијата треба да поддржува три различни конфигурациски околини: **Development**, **Staging** и **Production**. Креирајте соодветни `appsettings.{Environment}.json` датотеки.

Во `Program.cs` регистрирајте ги strongly-typed settings класите со `builder.Services.Configure<T>()`.

### CacheSettings

| Поле | Development | Staging | Production |
|------|-------------|---------|------------|
| `ListCacheDurationMinutes` | 10 | 20 | 60 |
| `DetailCacheDurationMinutes` | 15 | 25 | 60 |

### RateLimitSettings

| Поле | Development | Staging | Production |
|------|-------------|---------|------------|
| `PermitLimit` | 10 | 50 | 100 |
| `WindowInSeconds` | 150 | 60 | 60 |

### ApiKeySettings

| Поле | Development | Staging | Production |
|------|-------------|---------|------------|
| `ApiKey` | `Tk9wXm2RwQs4LnY7HjB3FcDfE5gA8uZo` | `Qw7rTm5YxKs2VnZ8PjD4BcEfG6hA9uLo` | `Xn3pRm8WzFs6KnQ1YjC5DcHfB7gA4uTo` |

### EmailSettings (за сите околини)

| Поле | Вредност |
|------|----------|
| `SmtpHost` | `smtp.gmail.com` |
| `SmtpPort` | `587` |
| `Username` | (по ваш избор) |
| `Password` | (по ваш избор) |
| `FromAddress` | `workshops@workshop.edu` |
| `FromName` | `Workshop System` |
| `UseSsl` | `true` |

---

## Барање 2 — Кеширање на резултати (15 поени)

Потребно е да се имплементира кеширање на резултати во сервисниот слој.

### Имплементација: `WorkshopService`

Имплементирајте го интерфејсот `IWorkshopService` во `Service/Implementation/WorkshopService.cs`.

**Зависности за инјектирање:**
- `IRepository<Workshop>` — за пристап до базата
- `IMemoryCache` — за кеширање
- `IOptions<CacheSettings>` — за читање на конфигурацијата

**Правила за кеширање:**
- Се применува на методот `GetAllAsync(string? venueName, DateOnly? date)`
- Клучот мора да ги вклучува вредностите на query параметрите: `workshops:{venueName}:{date}`
  - Пример: `workshops:Hall-A:2026-06-15` или `workshops::` (кога нема филтри)
- Времетраењето на кешот се чита од `CacheSettings.ListCacheDurationMinutes`

**Регистрација:**
- Додајте `builder.Services.AddMemoryCache()` во `Program.cs`

### Endpoint

`GET /api/Workshop?venueName={venueName}&date={date}` — Листа на работилници со опционално филтрирање.

---

## Барање 3 — Inbound REST API со API Key автентикација (34 + 31 поени)

Потребно е да се имплементира посебен сет на надворешни (external) endpoint-и кои овозможуваат на надворешни системи да пријавуваат учество на работилници. Овие endpoint-и се заштитени со API Key автентикација и подлежат на Rate Limiting.

### 3.1 — API Key Middleware

Имплементирајте `ApiKeyAuthMiddleware` во `Web/Middleware/ApiKeyAuthMiddleware.cs`.

**Правила:**
- Се применува **само** на патеки кои почнуваат со `/api/external`
- За сите останати патеки, middleware-от го пропушта барањето без проверка
- Доколку header-от `X-Api-Key` **недостасува**, враќа **401 Unauthorized** со порака: `API key is required`
- Доколку клучот е **невалиден**, враќа **401 Unauthorized** со порака: `Invalid API key`
- Очекуваниот клуч се чита од `IOptions<ApiKeySettings>`
- **ВАЖНО:** По запишување на одговорот, middleware-от мора да прекине (`return`) — не смее да го повика `_next(context)`

**Регистрација во `Program.cs`:**
```csharp
app.UseMiddleware<ApiKeyAuthMiddleware>();
```

### 3.2 — Надворешни endpoint-и (InboundController)

Имплементирајте `InboundController` во `Web/Controllers/InboundController.cs`.

**Route:** `/api/external/enrollment`

#### `POST /api/external/enrollment/register`

Прима барање за пријавување за работилница. Бидејќи се очекуваат голем број повици, барањето се **зачувува асинхроно** како `InboundEventEntry` со статус `Pending` и се процесира подоцна.

**Тело на барањето:**
```json
{
  "workshopId": "guid",
  "userId": "string",
  "venueId": "guid",
  "notes": "string (optional)"
}
```

**Одговор (202 Accepted):**
```json
{
  "status": "Pending",
  "id": "guid-of-inbound-event-entry"
}
```

#### `GET /api/external/enrollment/register/{id}/status`

Го враќа статусот на претходно поднесено барање.

**Одговор (200 OK):**
```json
{
  "status": "Pending|Completed|Failed",
  "id": "guid",
  "error": "null or error message"
}
```

### 3.3 — InboundEventEntryService

Имплементирајте `IInboundEventEntryService` во `Service/Implementation/InboundEventEntryService.cs`.

**Зависности:** `IRepository<InboundEventEntry>`

| Метод | Опис |
|-------|------|
| `CreateAsync(string rawPayload)` | Креира нов запис со `Status = Pending`, `ReceivedAt = DateTime.UtcNow` |
| `GetByIdNotNullAsync(Guid id)` | Го враќа записот или фрла `InvalidOperationException` |

### 3.4 — InboundEventEntryProcessor

Имплементирајте `IInboundEventEntryProcessor` во `Service/Implementation/InboundEventEntryProcessor.cs`.

**Зависности:** `IEnrollmentService`, `IRepository<InboundEventEntry>`

#### `ProcessPendingEventsAsync()`
1. Зема до **10** записи со `Status == Pending`
2. За секој запис повикува `ProcessEventEntry(entry)`

#### `ProcessEventEntry(InboundEventEntry entry)`
1. Го десеријализира `RawPayload` во `EnrollmentRequestDto`
   - ⚠️ **ЗАДОЛЖИТЕЛНО:** `new JsonSerializerOptions { PropertyNameCaseInsensitive = true }`
2. Валидира дека `UserId` не е null/empty
3. Креира `Enrollment` преку `IEnrollmentService.CreateAsync()`
4. Го означува записот како `Completed` со `ProcessedAt = DateTime.UtcNow` и `EnrollmentId = enrollment.Id`
5. При грешка: `Status = Failed`, `ErrorMessage = ex.Message`

### 3.5 — BackgroundService за процесирање

Имплементирајте `BackgroundService` во `Service/Jobs/BackgroundEnrollmentEntryService.cs` кој повикува `ProcessPendingEventsAsync()` **на секоја 1 минута**.

---

## Барање 4 — Rate Limiting (13 поени)

На сите надворешни endpoint-и (`/api/external`) да се примени Rate Limiting.

### Правила:
- Лимитот се применува **по API клуч** — секој клиент има свој независен бројач
- Конфигурацијата се чита од `RateLimitSettings` (PermitLimit, WindowInSeconds)
- При надминување на лимитот се враќа **429 Too Many Requests**

### Имплементација во `Program.cs`:

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddPolicy("external-api", context =>
    {
        var settings = context.RequestServices
            .GetRequiredService<IOptions<RateLimitSettings>>().Value;
        var apiKey = context.Request.Headers["X-Api-Key"].ToString();
        return RateLimitPartition.GetFixedWindowLimiter(apiKey, _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = settings.PermitLimit,
                Window = TimeSpan.FromSeconds(settings.WindowInSeconds),
                QueueLimit = 0
            });
    });
});
```

**Во pipeline-от:**
```csharp
app.UseRateLimiter();
```

**На контролерот:**
```csharp
[EnableRateLimiting("external-api")]
```

---

## Барање 5 — ETL: Инкрементален import преку надворешно API (58 + 13 поени)

Потребно е да се имплементира функционалност за инкрементален import (ETL) на работилници од надворешно REST API.

### 5.1 — Документација за надворешното API

| | |
|---|---|
| **Base URL** | `https://integriranisistemi.finki.ukim.mk/` |
| **Endpoint** | `GET /api/external/workshops?modifiedSince={datetime}` |
| **Header** | `X-Api-Key: kP7vXm2RwQs9TnY4LjB6HcDfE8gA1uZo` |

**Response:**
```json
{
  "items": [
    {
      "externalId": "string",
      "venueName": "string",
      "title": "string",
      "startTime": "2026-05-20T17:14:51.722Z",
      "endTime": "2026-05-20T17:14:51.722Z",
      "lastModifiedUtc": "2026-05-20T17:14:51.722Z",
      "status": "string"
    }
  ],
  "page": 0,
  "pageSize": 0,
  "totalCount": 0,
  "totalPages": 0,
  "hasNextPage": true
}
```

### 5.2 — WorkshopsApiClient

Имплементирајте `IWorkshopsApiClient<ExternalWorkshopsDto>` во `Service/Implementation/WorkshopsApiClient.cs`.

**Зависности:** `HttpClient` (typed client)

- Повикува `GET /api/external/workshops?modifiedSince={date}`
- Го десеријализира одговорот во `ExternalWorkshopsDto`

**Регистрација во `Program.cs`:**
```csharp
builder.Services.Configure<WorkshopsApiSettings>(
    builder.Configuration.GetSection("WorkshopsApiSettings"));

builder.Services.AddHttpClient<IWorkshopsApiClient<ExternalWorkshopsDto>, WorkshopsApiClient>(
    (sp, http) =>
    {
        var settings = sp.GetRequiredService<IOptions<WorkshopsApiSettings>>().Value;
        http.BaseAddress = new Uri(settings.BaseAddress);
        http.DefaultRequestHeaders.Add("X-Api-Key", settings.ApiKey);
    });
```

### 5.3 — EtlSyncService

Имплементирајте `IEtlSyncService` во `Service/Implementation/EtlSyncService.cs`.

**Зависности:**
- `IRepository<EtlSyncLog>`
- `IWorkshopsApiClient<ExternalWorkshopsDto>`
- `IWorkshopsRepository`
- `IRepository<Venue>`

**Алгоритам на `SyncAllAsync()`:**

1. Креира `EtlSyncLog` со `JobName = "WorkshopsSync"`, `StartedAt = DateTime.UtcNow`
2. Го бара последниот **успешен** sync log (`JobName == "WorkshopsSync" && Success == true`), подреден по `StartedAt` опаѓачки
3. Ја зема `StartedAt` датата (или `DateTime.MinValue` ако нема претходен log)
4. Го повикува API клиентот со таа дата
5. Ги чита сите `Venue` од базата и прави речник `Name → Id`
6. За секој `ExternalWorkshopDto`:
   - Генерира детерминистичко `Id` со `GuidHelper.FromLegacyId("Workshop", externalId)`
   - Креира `Workshop` објект со `StartTime`, `EndTime`, `Title`, `RoomId`
   - Ги филтрира само оние чие `VenueName` постои во речникот
7. Запишува преку `BulkInsertOrUpdateAsync` (insert or update, без дуплирање)
8. `log.Success = true`
9. При грешка: `log.Success = false`, `log.ErrorMessage = ex.Message`
10. Секогаш (finally): `log.CompletedAt = DateTime.UtcNow`, `InsertAsync(log)`

### 5.4 — SyncWorkshopsBackgroundService

Имплементирајте `BackgroundService` во `Service/Jobs/SyncWorkshopsBackgroundService.cs`.

- Инјектира `IServiceScopeFactory`
- Во `ExecuteAsync`: loop на секои **5 минути**, резолвира `IEtlSyncService` од scope, повикува `SyncAllAsync()`
- Регистрација: `builder.Services.AddHostedService<SyncWorkshopsBackgroundService>()`

---

## Барање 6 — Email со MailKit (25 поени)

Потребно е да се имплементира испраќање на email пораки преку SMTP со помош на **MailKit**.

### 6.1 — SmtpEmailService

Имплементирајте `IEmailService` во `Service/Implementation/SmtpEmailService.cs`.

**Зависности:** `IOptions<EmailSettings>`, `ILogger<SmtpEmailService>`

**Чекори:**
1. Креирајте `MimeMessage` со `From` (од `EmailSettings`), `To`, `Subject`
2. Користете `BodyBuilder` за да го поставите `HtmlBody` и `PlainText`
3. Ако има `Attachments`, додајте ги преку `builder.Attachments.Add()`
4. Конектирајте се на SMTP серверот преку `MailKit.Net.Smtp.SmtpClient`:
   ```csharp
   await smtp.ConnectAsync(settings.SmtpHost, settings.SmtpPort,
       settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
   await smtp.AuthenticateAsync(settings.Username, settings.Password);
   await smtp.SendAsync(email);
   ```
5. Секогаш дисконектирајте во `finally`

### 6.2 — Email Queue и Background Service

Веќе имплементирани и дадени:
- `ChannelEmailQueue` — користи `Channel<EmailMessage>` за async queue
- `EmailBackgroundService` — чита од каналот и испраќа преку `IEmailService`

### 6.3 — Регистрација во `Program.cs`

```csharp
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddSingleton<IEmailQueue, ChannelEmailQueue>();
builder.Services.AddSingleton(Channel.CreateUnbounded<EmailMessage>());
builder.Services.AddHostedService<EmailBackgroundService>();
```

### 6.4 — EmailController (даден)

`POST /api/Email/send-enrollment-report` — Генерира Excel извештај и го испраќа на дадена email адреса.

---

## Барање 7 — Excel Import и Export со ClosedXML (59 поени)

### 7.1 — Excel Export: `ExcelExportService`

Имплементирајте `IExcelExportService` во `Service/Implementation/ExcelExportService.cs`.

**Зависности:** `IEnrollmentService`

#### `ExportEnrollmentsToExcelAsync(Guid workshopId)`

1. Земете ги сите enrollments за дадениот workshop (`GetAllByWorkshopIdAsync`)
2. Креирајте `XLWorkbook` со sheet именуван **"Enrollments"**
3. Header ред со колони: `"Enrollment ID"`, `"User"`, `"Status"`, `"Notes"`, `"Venue"`
4. Стилизирајте го header редот:
   - `Font.Bold = true`
   - `Fill.BackgroundColor = XLColor.FromHtml("#4F46E5")`
   - `Font.FontColor = XLColor.White`
   - `Alignment.Horizontal = XLAlignmentHorizontalValues.Center`
5. Пополнете податочни редови:
   - Enrollment ID = `enrollment.Id.ToString()`
   - User = `$"{enrollment.User.FirstName} {enrollment.User.LastName}"`
   - Status = `enrollment.Status.ToString()`
   - Notes = `enrollment.Notes`
   - Venue = `enrollment.Venue.Name` (или преку навигациска пропертија)
6. `ws.Columns().AdjustToContents()` — авто ширина
7. `ws.RangeUsed()?.SetAutoFilter()` — авто филтер
8. `ws.SheetView.FreezeRows(1)` — замрзни header ред
9. Зачувајте во `MemoryStream` и вратете `byte[]`

**Endpoint (даден):** `GET /api/Export/enrollments/{workshopId}` — враќа `.xlsx` фајл

### 7.2 — Excel Import: `ExcelImportService`

Имплементирајте `IExcelImportService` во `Service/Implementation/ExcelImportService.cs`.

**Зависности:** `IRepository<Venue>` (за валидација на venue имиња)

#### `ImportWorkshopsAsync(Stream fileStream)`

1. Отворете го `XLWorkbook` од stream-от
2. Земете го првиот worksheet
3. Прочитајте го header редот и мапирајте ги имињата на колони (lowercase) → индекси
4. **Валидирајте** дека постојат задолжителни колони: `"title"`, `"startdate"`, `"enddate"`, `"venue"`
   - Ако недостасува колона, додајте грешка во `ImportResult.Errors`
5. За секој податочен ред (од ред 2 до последен):
   - **Title:** не смее да биде празно
   - **StartDate:** мора да биде валиден `DateTime`
   - **EndDate:** мора да биде валиден `DateTime` и **поголем** од StartDate
   - **Venue:** мора да постои во базата (проверка преку `IRepository<Venue>`)
   - При грешка: додајте `ImportError` со `Row`, `Column`, `Message`
   - При успех: додајте `WorkshopImportDto` во `SuccessfulRecords`
6. Вратете `ImportResult<WorkshopImportDto>`

**Endpoint (даден):** `POST /api/Import/workshops` — прима `.xlsx` фајл

**Валидации на endpoint ниво (веќе имплементирани):**
- Фајлот мора да е `.xlsx` (инаку 400)
- Фајлот не смее да надминува 5 MB (инаку 400)

### 7.3 — Регистрација во `Program.cs`

```csharp
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
builder.Services.AddScoped<IExcelImportService, ExcelImportService>();
```

### 7.4 — NuGet пакети

Додајте во `Service.csproj`:
```xml
<PackageReference Include="ClosedXML" Version="0.105.0" />
<PackageReference Include="MailKit" Version="4.16.0" />
```

Додајте во `Web.csproj` и `TestExamIS.Tests.csproj`:
```xml
<PackageReference Include="ClosedXML" Version="0.105.0" />
```

---

## Преглед на датотеки кои треба да ги имплементирате

| # | Датотека | Барање | Опис |
|---|----------|--------|------|
| 1 | `Web/appsettings.Development.json` | Б1 | Конфигурација за Development |
| 2 | `Web/appsettings.Staging.json` | Б1 | Конфигурација за Staging |
| 3 | `Web/appsettings.Production.json` | Б1 | Конфигурација за Production |
| 4 | `Service/Implementation/WorkshopService.cs` | Б2 | CRUD + кеширање со IMemoryCache |
| 5 | `Web/Middleware/ApiKeyAuthMiddleware.cs` | Б3 | API Key middleware за /api/external |
| 6 | `Web/Controllers/InboundController.cs` | Б3 | External enrollment endpoints |
| 7 | `Service/Implementation/InboundEventEntryService.cs` | Б3 | Креирање и читање на InboundEventEntry |
| 8 | `Service/Implementation/InboundEventEntryProcessor.cs` | Б3 | Процесирање на Pending записи |
| 9 | `Service/Jobs/BackgroundEnrollmentEntryService.cs` | Б3 | BackgroundService (1 мин) |
| 10 | `Service/Implementation/WorkshopsApiClient.cs` | Б5 | HTTP client за надворешно API |
| 11 | `Service/Implementation/EtlSyncService.cs` | Б5 | Инкрементален ETL sync |
| 12 | `Service/Jobs/SyncWorkshopsBackgroundService.cs` | Б5 | BackgroundService за ETL (5 мин) |
| 13 | `Service/Implementation/SmtpEmailService.cs` | Б6 | SMTP email со MailKit |
| 14 | `Service/Implementation/ExcelExportService.cs` | Б7 | Excel export со ClosedXML |
| 15 | `Service/Implementation/ExcelImportService.cs` | Б7 | Excel import со ClosedXML |
| 16 | `Web/Program.cs` | Сите | DI регистрации, middleware, rate limiter |

---

## Тестирање

Проектот вклучува тестови во `TestExamIS.Tests/`. Извршете ги со:

```bash
dotnet test
```

### Распределба на поени по категорија

| Категорија | Тестови | Поени |
|------------|---------|-------|
| Configuration — appsettings per environment | 7 | 21 |
| Cache — IMemoryCache на GET /api/Workshop | 3 | 15 |
| ExternalApi — POST/GET endpoints, API Key auth | 6 | 34 |
| InboundEvent — процесирање на pending events | 5 | 31 |
| RateLimit — 429 при надминување | 2 | 13 |
| EtlSync — incremental import, dedup, failure | 8 | 58 |
| BackgroundService — registration | 2 | 13 |
| Email — DI, queue, settings, background service | 5 | 25 |
| ExcelExport — sheet, headers, styling, endpoint | 5 | 28 |
| ExcelImport — valid data, invalid venue/dates, missing columns | 6 | 31 |
| **Вкупно** | **49** | **~269** |

---

## Совети

1. **Започнете со конфигурацијата** (Барање 1) — тоа се лесни поени и многу тестови зависат од тоа.
2. **Регистрирајте сè во Program.cs** пред да почнете да имплементирате — DI грешките се најчести.
3. **Middleware редослед е важен:** `UseMiddleware<ApiKeyAuthMiddleware>()` мора да биде пред `UseRateLimiter()`.
4. **PropertyNameCaseInsensitive = true** — JSON payload-от е camelCase, C# property-та се PascalCase.
5. **EtlSyncLog.JobName** мора да биде точно `"WorkshopsSync"` — тестовите ја seed-ираат базата со таа вредност.
6. **После запишување на 401 одговор во middleware**, мора да има `return` — инаку ќе добиете "Response has already started".
7. **ClosedXML** — sheet-от мора да се именува точно **"Enrollments"** за export тестовите да поминат.
