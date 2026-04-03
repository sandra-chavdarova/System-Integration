# Consultation Application

На линкот е даден почетен код со решението од првата лабораториска вежба.

Додадени се дополнителни елементи:
- PaginatedRequest
- PaginatedResult
- PaginatedResponse
- Екстензија за мапирање на резултатот во response во класата PaginatedResultMappingExtensions
- Функција во Repository GetAllPagedAsync<E> која враќа PaginatedResult<E>
- Интерфејс IConsultationService и празна имплементација на истиот сервис
- CurrentUserService
- AuditInterceptor
- AuthController

Цел на оваа лабораториска вежба е да се имплементира целиот податочен тек:  
**Controller -> Mapper -> Service -> Repository -> DB и назад**

## Задачи:

1. Да се креира DTO **ConsultationDto** кое се користи во CreateAsync и UpdateAsync методите во ConsultationService

2. Да се имплементираат сите методи во **ConsultationService**
   - GetByIdNotNullAsync(Guid id): Task<Consultation> - враќа еден ентитет, никогаш null — потребен е соодветен exception handling
   - GetAllAsync(string? roomName): Task<List<Consultation>> - враќа листа ентитети; ако roomName не е null, применува филтер
   - CreateAsync(ConsultationDto dto): Task<Consultation> - прима DTO и го враќа зачуваниот ентитет
   - UpdateAsync(Guid id, ConsultationDto dto): Task<Consultation> - прима DTO и го враќа ажурираниот ентитет
   - DeleteAsync(Guid id): Task<Consultation> - го брише ентитетот и го враќа избришаниот
   - GetPagedAsync(int pageNumber, int pageSize): Task<PaginatedResult<Consultation>> - враќа одредена страница од базата  
     - Овој метод треба да враќа податоци и за сите Attendance објекти кои се наоѓаат во колекцијата  
     - Не смее да се користи Lazy Load

3. Да се креира **ConsultationMapper** кој ги пресретнува барањата од Web слојот, ги повикува сервисните методи и ги мапира податоците во двете насоки  
   - Маперот треба да содржи методи за повикување на СИТЕ сервисни методи

4. Да се креира record **ConsultationResponse** кој ќе има податоци за:
   - идентификаторот на консултацијата  
   - времето за почеток и крај  
   - идентификаторот на собата  
   - името на собата  

5. Да се креира record **AttendanceResponse** кој ќе има податоци за:
   - идентификаторот на присуството  
   - идентификаторот на корисникот  
   - целосното име на корисникот  
   - статусот  
   - *(статусот треба да биде текстуален, не вредност од енумерација)*

6. Да се креира record **ConsultationWithAttendancesResponse** кој ќе ги содржи сите податоци како ConsultationResponse, но дополнително ќе чува `List<AttendanceResponse>`

7. Да се креира екстензија **AttendanceMappingExtensions** која ќе мапира:
   - Attendance -> AttendanceResponse
   - List<Attendance> -> List<AttendanceResponse>

8. Да се креира екстензија **ConsultationMappingExtensions** која ќе ги мапира:
   - Consultation -> ConsultationResponse
   - Consultation -> ConsultationWithAttendancesResponse
   - List<Consultation> -> List<ConsultationResponse>
   - CreateOrUpdateConsultationRequest -> ConsultationDto
   - PaginatedResult<Consultation> -> PaginatedResponse<ConsultationWithAttendancesResponse>

9. Да се креира **ConsultationsController** на патека `/api/consultations` со следните endpoints:
   - GET /{id} - враќа една консултација според ID
   - GET /?roomName - ги враќа сите консултации; поддржува опционален query параметар roomName за филтрирање
   - GET /paged?pageNumber=&pageSize= - враќа пагинирани резултати; задолжителни параметри: pageNumber, pageSize (PaginatedRequest)
   - POST / - креира нов Consultation од телото на барањето  
     - Само најавени корисници може да креираат консултации
   - PUT /{id} - ажурира постоечки Consultation со податоците од телото на барањето  
     - Само најавени корисници може да ажурираат консултации
   - DELETE /{id} - брише постоечки Consultation според ID
  

<img width="1886" height="1143" alt="Consultations" src="https://github.com/user-attachments/assets/f89780b1-7017-495b-964e-5956a7f9f111" />
