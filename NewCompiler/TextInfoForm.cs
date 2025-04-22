using System;
using System.Drawing;
using System.Windows.Forms;

namespace NewCompiler
{
    public partial class TextInfoForm : Form
    {
        public TextInfoForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Текст: информация";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            // Создаем TabControl для подпунктов
            var tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.Normal
            };

            // Добавляем вкладки для каждого подпункта
            AddProblemStatementTab(tabControl);
            AddGrammarTab(tabControl);
            AddGrammarClassificationTab(tabControl);
            AddAnalysisMethodTab(tabControl);
            AddErrorDiagnosticsTab(tabControl);
            AddTestCasesTab(tabControl);
            AddReferencesTab(tabControl);
            AddSourceCodeTab(tabControl);

            this.Controls.Add(tabControl);
        }

        private void AddProblemStatementTab(TabControl tabControl)
        {
            var tabPage = new TabPage("Постановка задачи");
            var richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Text = @"Константы — это элементы данных, значения которых известны и не изменяются в процессе выполнения программы. В языке Java для объявления строковой константы используется ключевое слово final.

Формат записи: final String имя_переменной = ""значение""; 

Примеры:
1. Обычная строковая константа: final String str = ""Hello World"";
2. Строка с символами Unicode (UTF-8) поддерживает символы кириллицы или других алфавитов: final String greeting = ""Привет, мир!"";
3. Объявление константы с явным указанием типа: final String message = ""Example"";

Согласно разработанной автоматной грамматике G[‹START›] лексический анализатор (сканер) строковых констант будет считать верными следующие записи:
• final String text123 = ""Sample"";
• final String greeting = ""Привет, мир!"";
• final String word = ""Hello"";"
            };
            tabPage.Controls.Add(richTextBox);
            tabControl.TabPages.Add(tabPage);
        }

        private void AddGrammarTab(TabControl tabControl)
        {
            var tabPage = new TabPage("Грамматика");
            var richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Text = @"1. ‹START› → 'final'‹SPACE1›
2. ‹SPACE1› → ' '‹TYPE›
3. ‹TYPE› → 'String'‹SPACE2›
4. ‹SPACE2› → ' '‹ID›
5. ‹ID› → ‹LETTER›‹IDREM›
6. ‹IDREM› → ‹LETTER›‹IDREM›
7. ‹IDREM› → ‹DIGIT›‹IDREM›
8. ‹IDREM› → '='‹QUOTE›
9. ‹QUOTE› → '""'‹STRING›
10. ‹STRING› → ‹SYMBOL›‹STRINGREM›
11. ‹STRINGREM› → ‹SYMBOL›‹STRINGREM›
12. ‹STRINGREM› → '""'‹END›
13. ‹END› → ';'

• ‹DIGIT› → ""0"" | ""1"" | ""2"" | ""3"" | ""4"" | ""5"" | ""6"" | ""7"" | ""8"" | ""9""
• ‹LETTER› → ""a"" | ""b"" | ""c"" | ... | ""z"" | ""A"" | ""B"" | ""C"" | ... | ""Z""
• ‹SYMBOL› → ""a"" | ""b"" | ""c"" | ... | ""z"" | ""A"" | ""B"" | ""C"" | ... | ""Z""| ""0"" | ""1"" | ""2"" | ""3"" | ""4"" | ""5"" | ""6"" | ""7"" | ""8"" | ""9""| ""!"" | ""."" | "","" | ""?"" | ""№"" | ""#"" | ""^"" | ""@"" | ""$""| "";"" | "":"" | "" "" | ""%"" | ""&"" | ""*"" | ""("" | "")"" | ""-"" | ""_"" | ""="" | ""+"" | ""/"" | ""\"" | ""|"" | ""<"" | "">"" | ""~"" | ""`""| ""["" | ""]"" | ""{"" | ""}"" | ""'"""

            };
            tabPage.Controls.Add(richTextBox);
            tabControl.TabPages.Add(tabPage);
        }

        private void AddGrammarClassificationTab(TabControl tabControl)
        {
            var tabPage = new TabPage("Классификация грамматики");
            var richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Text = @"Согласно классификации Хомского, грамматика G[‹START›] является автоматной.

Все правила относятся к классу праворекурсивных продукций (A → aB | a | ε), следовательно, грамматика является полностью автоматной."
            };
            tabPage.Controls.Add(richTextBox);
            tabControl.TabPages.Add(tabPage);
        }

        private void AddAnalysisMethodTab(TabControl tabControl)
        {
            // Создаем вкладку
            var tabPage = new TabPage("Метод анализа");

            // Основной контейнер с прокруткой
            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            // Текущая позиция Y для размещения элементов
            int currentY = 10;

            // Заголовок раздела
            var headerLabel = new Label
            {
                Text = "Метод анализа",
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, currentY)
            };
            scrollPanel.Controls.Add(headerLabel);
            currentY += headerLabel.Height + 15;

            // Первая часть текста
            var text1 = new Label
            {
                Text = "На рисунке 1 представлена диаграмма состояний сканера:",
                AutoSize = true,
                Location = new Point(10, currentY),
                Font = new Font("Arial", 9.5f)
            };
            scrollPanel.Controls.Add(text1);
            currentY += text1.Height + 10;

            // Первое изображение
            try
            {
                var image = Image.FromFile("Resources/Диаграмма.drawio.png");
                var picture1 = new PictureBox
                {
                    Image = image,
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    Location = new Point(10, currentY)
                };
                scrollPanel.Controls.Add(picture1);
                currentY += picture1.Height + 5;
            }
            catch
            {
                var errorLabel = new Label
                {
                    Text = "Изображение не найдено: Диаграмма.drawio.png",
                    ForeColor = Color.Red,
                    AutoSize = true,
                    Location = new Point(10, currentY)
                };
                scrollPanel.Controls.Add(errorLabel);
                currentY += errorLabel.Height + 15;
            }

            // Подпись к первому изображению
            var caption1 = new Label
            {
                Text = "Рисунок 1 - Диаграмма состояний сканера",
                AutoSize = true,
                Font = new Font("Arial", 8.5f, FontStyle.Italic),
                Location = new Point(10, currentY)
            };
            scrollPanel.Controls.Add(caption1);
            currentY += caption1.Height + 20;

            // Основной текст
            var mainText = new Label
            {
                Text = "Грамматика G[‹START›] является автоматной.\n\n" +
                      "Правила (1)-(13) для G[‹START›] реализованы на графе (см. рисунок 2).\n\n" +
                      "Сплошные стрелки на графе характеризуют синтаксически верный разбор; " +
                      "двойные символизируют состояние ошибки (ERROR). Состояние 11 " +
                      "символизирует успешное завершение разбора.",
                AutoSize = true,
                Location = new Point(10, currentY),
                Font = new Font("Arial", 9.5f),
                MaximumSize = new Size(tabControl.Width - 40, 0)
            };
            scrollPanel.Controls.Add(mainText);
            currentY += mainText.Height + 20;

            // Вторая часть текста
            var text2 = new Label
            {
                Text = "На рисунке 2 представлен граф G[‹START›]:",
                AutoSize = true,
                Location = new Point(10, currentY),
                Font = new Font("Arial", 9.5f)
            };
            scrollPanel.Controls.Add(text2);
            currentY += text2.Height + 10;

            // Второе изображение
            try
            {
                var image2 = Image.FromFile("Resources/Граф.drawio .png");
                var picture2 = new PictureBox
                {
                    Image = image2,
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    Location = new Point(10, currentY)
                };
                scrollPanel.Controls.Add(picture2);
                currentY += picture2.Height + 5;
            }
            catch
            {
                var errorLabel2 = new Label
                {
                    Text = "Изображение не найдено: Граф.drawio .png",
                    ForeColor = Color.Red,
                    AutoSize = true,
                    Location = new Point(10, currentY)
                };
                scrollPanel.Controls.Add(errorLabel2);
                currentY += errorLabel2.Height + 15;
            }

            // Подпись ко второму изображению
            var caption2 = new Label
            {
                Text = "Рисунок 2 - Граф G[‹START›]",
                AutoSize = true,
                Font = new Font("Arial", 8.5f, FontStyle.Italic),
                Location = new Point(10, currentY)
            };
            scrollPanel.Controls.Add(caption2);

            tabPage.Controls.Add(scrollPanel);
            tabControl.TabPages.Add(tabPage);
        }

        private void AddErrorDiagnosticsTab(TabControl tabControl)
        {
            var tabPage = new TabPage("Диагностика ошибок");
            var richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Text = @"В данном разделе должна быть описана диагностика и нейтрализация ошибок, которые могут возникнуть при анализе строковых констант."
            };
            tabPage.Controls.Add(richTextBox);
            tabControl.TabPages.Add(tabPage);
        }

        private void AddTestCasesTab(TabControl tabControl)
        {
            var tabPage = new TabPage("Тестовый пример");
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            var label = new Label
            {
                Text = "Ниже представлены тестовые примеры:",
                Dock = DockStyle.Top,
                AutoSize = true
            };
            panel.Controls.Add(label);

            // Загрузка тестовых изображений
            for (int i = 1; i <= 6; i++)
            {
                try
                {
                    var image = Image.FromFile($"Resources/ТестовыйПример{i}.png");
                    var pictureBox = new PictureBox
                    {
                        Image = image,
                        SizeMode = PictureBoxSizeMode.AutoSize,
                        Dock = DockStyle.Top
                    };
                    panel.Controls.Add(pictureBox);

                    var imgLabel = new Label
                    {
                        Text = $"Тестовый пример {i}",
                        Dock = DockStyle.Top,
                        AutoSize = true
                    };
                    panel.Controls.Add(imgLabel);
                }
                catch
                {
                    var errorLabel = new Label
                    {
                        Text = $"Изображение не найдено: ТестовыйПример{i}.png",
                        ForeColor = Color.Red,
                        Dock = DockStyle.Top,
                        AutoSize = true
                    };
                    panel.Controls.Add(errorLabel);
                }
            }

            tabPage.Controls.Add(panel);
            tabControl.TabPages.Add(tabPage);
        }

        private void AddReferencesTab(TabControl tabControl)
        {
            var tabPage = new TabPage("Список литературы");
            var richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Text = @"1. Шорников Ю.В. Теория языков программирования: проектирование и реализация : учеб. пособие / Ю.В. Шорников. – Новосибирск: Изд-во НГТУ, 2022.
2. Теория формальных языков и компиляторов [Электронный ресурс] / Электрон. дан. URL: https://dispace.edu.nstu.ru/didesk/course/show/8594, свободный. Яз.рус. (дата обращения 20.04.2025)."
            };
            tabPage.Controls.Add(richTextBox);
            tabControl.TabPages.Add(tabPage);
        }

        private void AddSourceCodeTab(TabControl tabControl)
        {
            var tabPage = new TabPage("Исходный код");
            var richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Text = @"В данном разделе должен быть представлен исходный код программы, реализующей лексический анализатор строковых констант."
            };
            tabPage.Controls.Add(richTextBox);
            tabControl.TabPages.Add(tabPage);
        }
    }
}