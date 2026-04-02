# Course Application
На линкот е даден почетен код со решението од првата лабораториска вежба.
Додадени се дополнителни елементи:
- PaginatedRequest
- PaginatedResult
- PaginatedResponse
- Екстензија за мапирање на резултатот во response во класата PaginatedResultMappingExtensions
- Функција во Repository GetAllPagedAsync<E> која враќа PaginatedResult<E>
- Интерфејс ICourseService за кој дополнително треба да се напише имплементацијата
Цел на оваа лабораториска вежба е да се имплементира целиот податочен тек:  
**Controller -> Mapper -> Service -> Repository -> DB и назад**
## Задачи:
1. Да се креира DTO **CourseDto** кое се користи во CreateAsync и UpdateAsync методите во CourseService
2. Да се имплементираат сите методи во **CourseService**
   - GetByIdNotNullAsync(Guid id): Task<Course> - враќа еден ентитет, никогаш null — потребен е соодветен exception handling
   - GetAllAsync(string? category): Task<List<Course>> - враќа листа ентитети; ако category не е null, применува филтер
   - CreateAsync(CourseDto dto): Task<Course> - прима DTO и го враќа зачуваниот ентитет
   - UpdateAsync(Guid id, CourseDto dto): Task<Course> - прима id и DTO и го враќа ажурираниот ентитет
   - DeleteAsync(Guid id): Task<Course> - го брише ентитетот и го враќа избришаниот
   - GetPagedAsync(int pageNumber, int pageSize): Task<PaginatedResult<Course>> - враќа одредена страница од базата  
     - Овој метод треба да враќа податоци и за сите Enrollment објекти кои се наоѓаат во колекцијата  
     - Не смее да се користи Lazy Load

3. Да се креира **CourseMapper** кој ги пресретнува барањата од Web слојот, ги повикува сервисните методи и ги мапира податоците во двете насоки  
   - Маперот треба да содржи методи за повикување на СИТЕ сервисни методи

4. Да се креира record **CourseResponse** кој ќе има податоци за:
   - идентификаторот на курсот  
   - името  
   - описот  
   - кредитите  
   - категоријата  
   - идентификаторот на семестарот  
   - името на семестарот  

5. Да се креира record **EnrollmentResponse** кој ќе има податоци за:
   - идентификаторот на запишувањето  
   - идентификаторот на корисникот  
   - целосното име на корисникот  
   - датумот на запишување  

6. Да се креира record **CourseWithEnrollmentsResponse** кој ќе ги содржи сите податоци како CourseResponse, но дополнително ќе чува `List<EnrollmentResponse>`

7. Да се креира екстензија **EnrollmentMappingExtensions** која ќе мапира:
   - Enrollment -> EnrollmentResponse
   - List<Enrollment> -> List<EnrollmentResponse>

8. Да се креира екстензија **CourseMappingExtensions** која ќе ги мапира:
   - Course -> CourseResponse
   - Course -> CourseWithEnrollmentsResponse
   - List<Course> -> List<CourseResponse>
   - CreateOrUpdateCourseRequest -> CourseDto
   - PaginatedResult<Course> -> PaginatedResponse<CourseWithEnrollmentsResponse>

9. Да се креира **CourseController** на патека `/api/courses` со следните endpoints:
   - GET /{id} - враќа еден Course според ID
   - GET /?category - ги враќа сите Course-ови; поддржува опционален query параметар category за филтрирање
   - GET /paged?pageNumber=&pageSize= - враќа пагинирани резултати; задолжителни параметри: pageNumber, pageSize (PaginatedRequest)
   - POST / - креира нов Course од телото на барањето
   - PUT /{id} - ажурира постоечки Course со податоците од телото на барањето
   - DELETE /{id} - брише постоечки Course според ID
  


<img width="1510" height="939" alt="Course" src="https://github.com/user-attachments/assets/da807918-cee2-44ef-8a0b-1308c8c73572" />
