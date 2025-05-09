## Постановка задачи:
1. Спроектировать диаграмму состояний сканера.
2. Разработать лексический анализатор, позволяющий выделить в тексте лексемы, иные символы считать недопустимыми (выводить ошибку).
3. Встроить сканер в ранее разработанный интерфейс текстового редактора. Учесть, что текст для разбора может состоять из множества строк.

## Вариант:
Объявление и инициализация строковой константы на языке Java.
## Пример допустимой строки:
```Java
  final String str = "Hello World"; 
```
```Java
  final String str123 = "Hello"; 
```

## Диаграмма состояний сканера
![](https://github.com/Vikoops/NewCompiler/blob/master/Диаграмма.drawio.png)

## Граф
![](https://github.com/Vikoops/NewCompiler/blob/master/Граф.drawio%20.png)

## Грамматика
1. `‹START›` → `'final'‹SPACE1›`
2. `‹SPACE1›` → `' '‹TYPE›`
3. `‹TYPE›` → `'String'‹SPACE2›`
4. `‹SPACE2›` → `' '‹ID›`
5. `‹ID›` → `‹LETTER›‹IDREM›`
6. `‹IDREM›` → `‹LETTER›‹IDREM›`
7. `‹IDREM›` → `‹DIGIT›‹IDREM›`
8. `‹IDREM›` → `'='‹QUOTE›`
9. `‹QUOTE›` → `'"'‹STRING›`
10. `‹STRING›` → `‹SYMBOL›‹STRINGREM›`
11. `‹STRINGREM›` → `‹SYMBOL›‹STRINGREM›`
12. `‹STRINGREM›` → `'"'‹END›`
13. `‹END›` → `';'`

-`‹LETTER›` → `"a"|"b"|...|"z"|"A"|"B"|...|"Z"`

-`‹DIGIT›` → `"0"|"1"|...|"9"`

-`‹SYMBOL›` → `“a” | “b” | “c” | ... | “z” | “A” | “B” | “C” | ... | “Z”| “0” | “1” | “2” | “3” | “4” | “5” | “6” | “7” | “8” | “9”| “!” | “.” | “,” | “?” | “№” | “#” | “^” | “@” | “$”| “;” | “:” | “ ” | “%” | “&” | “*” | “(” | “)” | “-” | “=” | “+” | “/” | “<” | “>” | “~” | “[” | “]” | “{” | “}” | “\” | “|” | “_” | “’”`

Следуя введенному формальному определению грамматики, представим G[‹START›] ее составляющими:
- **Z** = ‹START›;
- **VT** = {a, b, c, ..., z, A, B, C, ..., Z, !, ., ,, ?, №, #, ^, @, $, ;, : , %, &, *, (, ), -, _, =, +, /, \, |, <, >, ~, [, ], {, }, ‘, 0, 1, 2, ..., 9, final, String};
- **VN** = {‹START›, ‹SPACE1›, ‹TYPE›, ‹SPACE2›, ‹ID›, ‹IDREM›, ‹QUOTE›, ‹STRING›, ‹STRINGREM›, ‹END›}

## Тестовые примеры
![](https://github.com/Vikoops/NewCompiler/blob/master/Тестовый_пример_1.png)
![](https://github.com/Vikoops/NewCompiler/blob/master/Тестовый_пример_2.png)

# Справка
## Файл
- **Создать** - создание нового файла.
- **Открыть** - открытие существующего файла в формате `.txt`.
- **Сохранить** - сохранение текущего файла.
- **Сохранить как** - сохранение файла под новым именем.
- **Выход** - выход из программы.

## Правка
- **Отменить/Повторить** - отмена последнего действия или повтор отмененного действия.
- **Вырезать** - выделенный текст вырезается и копируется в буфер обмена.
- **Копировать** - выделенный текст копируется в буфер обмена.
- **Вставить** - текст из буфера обмена вставляется в окно редактирования.
- **Удалить** - удаление выделенного текста.
- **Выделить все** - выделяется весь текст из окна редактирования.
