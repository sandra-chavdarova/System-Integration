# Exams Application
На линкот е даден почетен код со решението од првата лабораториска вежба.
Додадени се дополнителни елементи:
- PaginatedRequest
- PaginatedResult
- PaginatedResponse
- Екстензија за мапирање на резултатот во response во класата PaginatedResultMappingExtensions
- Функција во Repository GetAllPagedAsync<E> која враќа PaginatedResult<E>
- Интерфејс IExamSlotService и празна имплементација на истиот сервис
Цел на оваа лабораториска вежба е да се имплементира целиот податочен тек:  
**Controller -> Mapper -> Service -> Repository -> DB и назад**

## Задачи:
1. Да се креира DTO **ExamSlotDto** кое се користи во CreateAsync и UpdateAsync методите во ExamSlotService
2. Да се имплементираат сите методи во **ExamSlotService**
   - GetByIdNotNullAsync(Guid id): Task<ExamSlot> - враќа еден ентитет, никогаш null — потребен е соодветен exception handling
   - GetAllAsync(string? sessionType): Task<List<ExamSlot>> - враќа листа ентитети; ако sessionType не е null, применува филтер
   - CreateAsync(ExamSlotDto dto): Task<ExamSlot> - прима DTO и го враќа зачуваниот ентитет
   - UpdateAsync(Guid id, ExamSlotDto dto): Task<ExamSlot> - прима DTO и го враќа ажурираниот ентитет
   - DeleteAsync(Guid id): Task<ExamSlot> - го брише ентитетот и го враќа избришаниот
   - GetPagedAsync(int pageNumber, int pageSize): Task<PaginatedResult<ExamSlot>> - враќа одредена страница од базата  
     - Овој метод треба да враќа податоци и за сите ExamAttempt објекти кои се наоѓаат во колекцијата  
     - Не смее да се користи Lazy Load
3. Да се креира **ExamSlotMapper** кој ги пресретнува барањата од Web слојот, ги повикува сервисните методи и ги мапира податоците во двете насоки  
   - Маперот треба да содржи методи за повикување на СИТЕ сервисни методи
4. Да се креира record **ExamSlotResponse** кој ќе има податоци за:
   - идентификаторот на слотот  
   - времето за почеток и крај  
   - идентификаторот на курсот  
   - името на курсот  
5. Да се креира record **ExamAttemptResponse** кој ќе има податоци за:
   - идентификаторот на обидот  
   - идентификаторот на корисникот  
   - целосното име на корисникот  
   - оцената  
6. Да се креира record **ExamSlotWithExamAttemptsResponse** кој ќе ги содржи сите податоци како ExamSlotResponse, но дополнително ќе чува и `List<ExamAttemptResponse>`
7. Да се креира екстензија **ExamAttemptMappingExtensions** која ќе мапира:
   - ExamAttempt -> ExamAttemptResponse
   - List<ExamAttempt> -> List<ExamAttemptResponse>
8. Да се креира екстензија **ExamSlotMappingExtensions** која ќе мапира:
   - ExamSlot -> ExamSlotResponse
   - ExamSlot -> ExamSlotWithExamAttemptsResponse
   - List<ExamSlot> -> List<ExamSlotResponse>
   - CreateOrUpdateExamSlotRequest -> ExamSlotDto
   - PaginatedResult<ExamSlot> -> PaginatedResponse<ExamSlotWithExamAttemptsResponse>
9. Да се креира **ExamSlotController** на патека `/api/exam-slots` со следните endpoints:
   - GET /{id} - враќа еден ExamSlot според ID
   - GET /?sessionType - ги враќа сите ExamSlot-ови; поддржува опционален query параметар sessionType за филтрирање
   - GET /paged?pageNumber=&pageSize= - враќа пагинирани резултати; задолжителни параметри: pageNumber, pageSize (PaginatedRequest)
   - POST / - креира нов ExamSlot од телото на барањето
   - PUT /{id} - ажурира постоечки ExamSlot со податоците од телото на барањето
   - DELETE /{id} - брише постоечки ExamSlot според ID


<img width="1867" height="1076" alt="Exams" src="https://github.com/user-attachments/assets/92081540-63ac-41fe-a993-d0c012ca0637" />
