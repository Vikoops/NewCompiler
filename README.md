
## Содержание

- [Лабораторные работы №1-4](#Лабораторные-1-4)
- [Лабораторная работа №5: Включение семантики в анализатор. Создание внутренней формы представления программы](#Лабораторная-5)
- [Лабораторная работа №6: Реализация алгоритма поиска подстрок с помощью регулярных выражений](#Лабораторная-6)
   - [Дополнительное задание](#Дополнительное-задание-для-6-лабораторной-работы)
- [Лабораторная работа №8: Реализация метода рекурсивного спуска для синтаксического анализа](#Лабораторная-8)


# Лабораторные 1-4

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
![](https://github.com/Vikoops/NewCompiler/blob/master/image/Диаграмма.drawio.png)

## Граф
![](https://github.com/Vikoops/NewCompiler/blob/master/image/Граф.drawio%20.png)

## Грамматика
1. `‹START›` → `'final'‹SPACE1›`
2. `‹SPACE1›` → `' '‹TYPE›`
3. `‹TYPE›` → `'String'‹SPACE2›`
4. `‹SPACE2›` → `' '‹ID›`
5. `‹ID›` → `‹LETTER›‹IDREM›`
6. `‹IDREM›` → `‹LETTER›‹IDREM›` | `‹DIGIT›‹IDREM›` | `'='‹QUOTE›`
7. `‹QUOTE›` → `'"'‹STRING›`
8. `‹STRING›` → `‹SYMBOL›‹STRINGREM›`
9. `‹STRINGREM›` → `‹SYMBOL›‹STRINGREM›` | `'"'‹END›`
10. `‹END›` → `';'`

-`‹LETTER›` → `"a"|"b"|...|"z"|"A"|"B"|...|"Z"`

-`‹DIGIT›` → `"0"|"1"|...|"9"`

-`‹SYMBOL›` → `“a” | “b” | “c” | ... | “z” | “A” | “B” | “C” | ... | “Z”| “0” | “1” | “2” | “3” | “4” | “5” | “6” | “7” | “8” | “9”| “!” | “.” | “,” | “?” | “№” | “#” | “^” | “@” | “$”| “;” | “:” | “ ” | “%” | “&” | “*” | “(” | “)” | “-” | “=” | “+” | “/” | “<” | “>” | “~” | “[” | “]” | “{” | “}” | “\” | “|” | “_” | “’”`

Следуя введенному формальному определению грамматики, представим G[‹START›] ее составляющими:
- **Z** = ‹START›;
- **VT** = {a, b, c, ..., z, A, B, C, ..., Z, !, ., ,, ?, №, #, ^, @, $, ;, : , %, &, *, (, ), -, _, =, +, /, \, |, <, >, ~, [, ], {, }, ‘, 0, 1, 2, ..., 9, final, String};
- **VN** = {‹START›, ‹SPACE1›, ‹TYPE›, ‹SPACE2›, ‹ID›, ‹IDREM›, ‹QUOTE›, ‹STRING›, ‹STRINGREM›, ‹END›}

## Тестовые примеры
![](https://github.com/Vikoops/NewCompiler/blob/master/image/Тестовый_пример_1.png)
![](https://github.com/Vikoops/NewCompiler/blob/master/image/Тестовый_пример_2.png)

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


# Лабораторная 5

## Постановка задачи
Выполнить разбор строки в виде тетрад

1) Реализовать в текстовом редакторе поиск лексических и синтаксических ошибок для грамматики G[<E>]. Реализовать данную КС-граммматику методом рекурсивного спуска:

1. E → TA 
2. A → ε | + TA | - TA 
3. T → ОВ 
4. В → ε | *ОВ | /ОВ 
5. О → id | (E) 
6. id → letter {letter}

2) Реализовать алгоритм записи выражений в форме тетрад.

### Примеры верных строк:
```
1. a + b * (c - d)
2. ((a+b)/(c*d)) - k - t
3. c = a + b
```

## Тестовые примеры
![](https://github.com/Vikoops/NewCompiler/blob/master/image/ТестТетрады1.png)
![](https://github.com/Vikoops/NewCompiler/blob/master/image/ТестТетрады2.png)
![](https://github.com/Vikoops/NewCompiler/blob/master/image/ТестТетрады3.png)


# Лабораторная 6 

## Решение 1 блока задач:
```
private static readonly Regex PunctuationRx = new Regex(@"[^\w\s-]|_");
```
## Решение 2 блока задач:
```
private static readonly Regex AmexCardRx = new Regex(@"\b3[47]\d{2}[-\s]?\d{6}[-\s]?\d{5}\b");
```
## Решение 3 блока задач:
```
private static readonly Regex RgbColorRx = new Regex(@"(?:^|\s)rgb\(\s*(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\s*,\s*(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\s*,\s*(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\s*\)",
RegexOptions.IgnoreCase);
```

## Тестовые примеры
![](https://github.com/Vikoops/NewCompiler/blob/master/image/ТестовыйПример6_1.png)
![](https://github.com/Vikoops/NewCompiler/blob/master/image/ТестовыйПример6_2.png)
![](https://github.com/Vikoops/NewCompiler/blob/master/image/ТестовыйПример6_3.png)

## Дополнительное задание для 6 лабораторной работы
![](https://github.com/Vikoops/NewCompiler/blob/master/image/Граф6.drawio.png)


# Лабораторная 8

## Вариант 24
```
G[unsigned_number]:
1. unsigned_number ::= decimal_number | exponential_part |
decimal_number exponential_part
2. decimal_number ::= unsigned_integer | decimal_fraction |
unsigned_integer decimal_fraction
3. unsigned_integer ::= digit | unsigned_integer digit
4. integer ::= unsigned_integer | '+' unsigned_integer | '-'
unsigned_integer
5. decimal_fraction ::= '.' unsigned_integer
6. exponential_part ::= '10' integer
digit ::= 0 | 1 |…| 9
```

## Язык, задаваемый грамматикой

Эта грамматика описывает **беззнаковые числа**, которые могут быть представлены в следующих формах:
   - Целые числа (например: ```42```, ```1005000```);
   - Десятичные дроби: ```3.14```, ```0.001```;
   - Числа с экспоненциальной записью: ```10+5```, ```10-12```;
   - Комбинации целой и дробной части с экспонентой: ```6.02210+23```.

Пример корректного выражения:
```
123
3.14159
10-5
2.71810+8
```

## Классификация грамматики

- Эта грамматика относится к **контекстно-свободным (КС)**.
- Каждое правило имеет форму: ```<нетерминал> ::= <цепочка из терминалов и нетерминалов>```.
- Подходит для построения рекурсивного спуска, так как не содержит леворекурсивных правил.


## Схема вызова функций (дерево разбора)

**В процессе синтаксического анализа вызываются правила грамматики в следующем порядке:**

```
<unsigned_number>
├── <decimal_number>
│   ├── <unsigned_integer>
│   │   ├── <digit>
│   │   └── { <digit> }                     ← рекурсивное наращивание цифр
│   ├── (опционально) <decimal_fraction>
│   │   ├── "."
│   │   └── <unsigned_integer>              ← дробная часть
│   └── или комбинация:
│       ├── <unsigned_integer>
│       └── <decimal_fraction>
├── или <exponential_part>
│   ├── "10"
│   └── <integer>
│       ├── (опционально) "+" | "-"
│       └── <unsigned_integer>
└── или комбинация:
    ├── <decimal_number>
    └── <exponential_part>
```

**Схема вызова функций в программе:**
```
UnsignedNumberParser::Analyze
  └── ParseUnsignedNumber
      ├── ParseDecimalNumber
      │    ├── ParseUnsignedInteger
      │    │    ├── IsDigit (проверка каждого символа)
      │    │    └── Advance (перемещение позиции)
      │    ├── ParseDecimalFraction (при наличии '.')
      │    │    ├── CurrentChar (проверка текущего символа)
      │    │    ├── Advance (пропуск '.')
      │    │    └── ParseUnsignedInteger
      │    └── (при отсутствии числа: возврат на начальную позицию)
      └── ParseExponentialPart
           ├── CurrentChar (проверка символов '10')
           ├── Advance (пропуск '1' и '0')
           ├── CurrentChar (проверка знака '+' или '-')
           ├── Advance (пропуск знака)
           └── ParseUnsignedInteger (проверка числа после знака)
```

## Тестовые примеры
![](https://github.com/Vikoops/NewCompiler/blob/master/image/ТестовыйПример8_1.png)
![](https://github.com/Vikoops/NewCompiler/blob/master/image/ТестовыйПример8_2.png)
![](https://github.com/Vikoops/NewCompiler/blob/master/image/ТестовыйПример8_3.png)
![](https://github.com/Vikoops/NewCompiler/blob/master/image/ТестовыйПример8_4.png)
![](https://github.com/Vikoops/NewCompiler/blob/master/image/ТестовыйПример8_5.png)
