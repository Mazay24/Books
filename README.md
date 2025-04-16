# Описанием проекта и его функциональности
**Система управления библиотекой**
- Добавление и каталогизация книг
- Отслеживание доступности
- Функционал выдачи и возврата
- Поиск и фильтрация книг

# Инструкциями по установке и запуску
Скачайте ZIP файл проекта.
![image](https://github.com/user-attachments/assets/4fa92795-e048-4514-9445-2d6f72355a13)
  
Разархивируйте его, перейдите в папку Books\bin\Debug\net8.0-windows и запустите .exe файл.
  
![image](https://github.com/user-attachments/assets/d2f49e96-2fea-42a0-8274-e1d0c2c7e4cc)

# Скриншотами интерфейса

![image](https://github.com/user-attachments/assets/31fff172-abaa-48cc-a7d5-570407f11b37)

![image](https://github.com/user-attachments/assets/a9addaf9-8b62-4c64-94db-7aee681c48d1)

![image](https://github.com/user-attachments/assets/fc9b5816-fc40-4eb7-b787-e6ddada95f5e)

![image](https://github.com/user-attachments/assets/38d0b212-fa47-4cf1-9c4e-ff530b47cf16)

![image](https://github.com/user-attachments/assets/7a986345-d5bf-4877-a0f5-e6b4c6c813a2)

# Списком использованных технологий
1. Язык программирования - C#
 2. Платформа - .NET (Windows Forms для UI)
3. Инструменты разработки - Visual Studio (IDE), Draw.io (диаграммы архитектуры)
4. Контроль версий - GitHub
 
  # Информацией об архитектуре приложения
Приложение разделено на 3 основных слоя с четким разделением ответственности:

1. Presentation Layer (UI)

- Windows Forms (.NET Framework/.NET Core)

- Формы: MainForm, BookForm, ReaderForm, BorrowForm

- Отвечает за:

- - Взаимодействие с пользователем

- - Валидацию ввода

- - Отображение данных

2. Business Logic Layer

- Классы-сервисы: BookService, ReaderService, BorrowService

- Отвечает за:

- - Обработку бизнес-правил (например, проверку доступности книги)

- - Координацию между UI и данными

3. Data Access Layer

- Классы-репозитории: BookRepository, ReaderRepository

- Отвечает за:

- - Работу с данными (в текущей версии — коллекции в памяти)








