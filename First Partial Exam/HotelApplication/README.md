# Hotel Application
На линкот е даден почетен код со решението од првата лабораториска вежба.
Додадени се дополнителни елементи:
- PaginatedRequest
- PaginatedResult
- PaginatedResponse
- Екстензија за мапирање на резултатот во response во класата PaginatedResultMappingExtensions.
- Функција во Repository GetAllPagedAsync<E> која враќа PaginatedResult<E>
- Интерфејс IRoomService и празна имплементација на истиот сервис
- CurrentUserService
- AuditInterceptor
- AuthController
Цел на оваа лабораториска вежба е да се имплементира целиот податочен тек:
**Controller -> Mapper -> Service -> Repository -> DB и назад**
  
## Задачи:
1. Да се креира DTO **RoomDto** кое се користи во InsertAsync и UpdateAsync методите во RoomService
2. Да се имплементираат сите методи во **RoomService**
   - GetByIdNotNullAsync(Guid id): Task<Room> - враќа еден ентитет, никогаш null — потребен е соодветен exception handling
   - GetAllAsync(int? status): Task<List<Room>> - враќа листа ентитети; ако staus не е null, применува филтер
   - InsertAsync(RoomDto dto): Task<Room> - прима DTO и го враќа зачуваниот ентитет
   - UpdateAsync(Guid id, RoomDto dto): Task<Room> - прима id и DTO и го враќа ажурираниот ентитет
   - DeleteAsync(Guid id): Task<Room> - го брише ентитетот и го враќа избришаниот
   - GetPagedAsync(int pageNumber, int pageSize): Task<PaginatedResult<Room>> - враќа одредена страница од базата 
     - Овој метод треба да враќа податоци и за сите Reservation објекти кои се наоѓаат во колекцијата
     - Не смее да се користи Lazy Load
3. Да се креира **RoomMapper** кој ги пресретнува барањата од Web слојот, ги повикува сервисните методи и ги мапира податоците во двете насоки.
   - Маперот треба да содржи методи за повикување на СИТЕ сервисни методи.
4. Да се креира record **RoomResponse** кој ќе има податоци за идентификаторот на собата, бројот, капацитетот, статусот, цена за ноќевање, идентификаторот на хотелот во кој се наоѓа собата како и неговото име.
   - Притоа, потребно е да се прикаже текстуална информација за статусот, не вредноста на енумерацијата.
5. Да се креира record **ReservationResponse** кој ќе има податоци за идентификаторот на резервацијата, почетниот и крајниот датум, датум на хотелската услуга (доколку има) идентификаторот на корисникот, целосното име на корисникот, идентификаторот на хотелската услуга и нејзината цена.
6. Да се креира record **RoomWithReservationsResponse** кој ќе ги содржи сите податоци како и RoomResponse, но дополнително ќе чува и List<ReservationResponse>.
7. Да се креира екстензија **ReservationMappingExtensions** која ќе мапира:
   - Reservation-> ReservationResponse
   - List<Reservation> -> List<ReservationResponse>
8. Да се креира екстензија **RoomMappingExtenstions** која ќе ги мапира:
   - Room-> RoomResponse
   - Room-> RoomWithReservationsResponse
   - List<Room> -> List<RoomResponse>
   - CreateOrUpdateRoomRequest -> RoomDto
   - PaginatedResult<Room> -> PaginatedResponse<RoomWithReservationsResponse>
9. Да се креира **RoomController** на патека /api/rooms со следните endpoints:
   - GET /{id} - враќа една консултација според ID
   - GET /?status- ги враќа сите консултации; поддржува опционален query параметар status за филтрирање
   - GET /paged?pageNumber=&pageSize= - враќа пагинирани резултати; задолжителни параметри: pageNumber, pageSize (PaginatedRequest)
   - POST / - креира нов Room од телото на барањето
     - Само најавени корисници може да креираат соби
   - PUT /{id} - ажурира постоечки Room со податоците од телото на барањето
     - Само најавени корисници може да ажурираат соби
   - DELETE /{id} - брише постоечки Room според ID


<img width="1104" height="917" alt="Hotel" src="https://github.com/user-attachments/assets/0f1f1af2-b127-465e-9cee-f88d36aadef7" />
