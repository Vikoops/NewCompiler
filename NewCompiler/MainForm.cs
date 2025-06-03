using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static NewCompiler.Regular;

namespace NewCompiler
{
    public partial class Compiler : Form
    {
        private string currentFilePath = string.Empty;
        private bool isTextChanged = false;
        private Stack<string> undoStack = new Stack<string>();
        private Stack<string> redoStack = new Stack<string>();
        private bool isUndoRedoOperation = false;
        private Analyzer analyzer = new Analyzer();

        public Compiler()
        {
            InitializeComponent();

            // Настройка вкладок
            InputTabControl.TabPages.Add("Новый файл");
            //OutputTabControl.TabPages.Add("Результат");

            // Настройка RichTextBox
            var inputRichTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                AcceptsTab = true
            };
            inputRichTextBox.TextChanged += InputRichTextBox_TextChanged;
            InputTabControl.TabPages[0].Controls.Add(inputRichTextBox);

            // Обработчик закрытия вкладки по крестику
            InputTabControl.MouseDown += (s, e) =>
            {
                for (int i = 0; i < InputTabControl.TabPages.Count; i++)
                {
                    var rect = InputTabControl.GetTabRect(i);
                    var closeButton = new Rectangle(rect.Right - 20, rect.Top + 5, 15, 15);
                    if (closeButton.Contains(e.Location))
                    {
                        CloseTab(i);
                        break;
                    }
                }
            };
            this.FormClosing += Compiler_FormClosing;
        }


        private void RunAnalyzer()
        {
            if (InputTabControl.TabCount > 0 && GetCurrentRichTextBox() != null)
            {
                string inputText = GetCurrentRichTextBox().Text;
                // 1-4 лабы
                /*
                analyzer.Analyze(inputText, GetOutputDataGridView());

                TabPage existingParserTab = OutputTabControl.TabPages
                    .Cast<TabPage>()
                    .FirstOrDefault(tp => tp.Text == "Parser Output");

                if (existingParserTab != null)
                {
                    OutputTabControl.TabPages.Remove(existingParserTab);
                    existingParserTab.Dispose();
                }

                TabPage parserTab = new TabPage("Parser Output");
                DataGridView parserGridView = new DataGridView
                {
                    Dock = DockStyle.Fill
                };
                parserTab.Controls.Add(parserGridView);
                OutputTabControl.TabPages.Add(parserTab);

                Parser parser = new Parser();
                parser.Parse(inputText);
                parser.ShowResults(parserGridView);
                */

                // тетрады
                /*
                TabPage existingTetradaTab = OutputTabControl.TabPages
    .Cast<TabPage>()
    .FirstOrDefault(tp => tp.Text == "Тетрады");

                if (existingTetradaTab != null)
                {
                    OutputTabControl.TabPages.Remove(existingTetradaTab);
                    existingTetradaTab.Dispose();
                }

                // Создаем новую вкладку для вывода тетрад
                TabPage tetradaTab = new TabPage("Тетрады");
                SplitContainer splitContainer = new SplitContainer
                {
                    Dock = DockStyle.Fill,
                    Orientation = Orientation.Vertical
                };

                DataGridView tetradaGridView = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                };

                TextBox errorTextBox = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    ReadOnly = true,
                    BackColor = SystemColors.Window,
                    Visible = false
                };

                splitContainer.Panel1.Controls.Add(tetradaGridView);
                splitContainer.Panel2.Controls.Add(errorTextBox);
                tetradaTab.Controls.Add(splitContainer);
                OutputTabControl.TabPages.Add(tetradaTab);

                // Запускаем анализ и выводим результаты
                Tetrada.AnalyzeExpression(
                    inputText,
                    GetCurrentRichTextBox(),
                    tetradaGridView,
                    errorTextBox
                );
                */

                // регулярные выражения
                /*
                // Создаем новую вкладку для вывода результатов регулярных выражений
                TabPage existingRegularTab = OutputTabControl.TabPages
                    .Cast<TabPage>()
                    .FirstOrDefault(tp => tp.Text == "Regular Expressions");

                if (existingRegularTab != null)
                {
                    OutputTabControl.TabPages.Remove(existingRegularTab);
                    existingRegularTab.Dispose();
                }

                TabPage regularTab = new TabPage("Regular Expressions");
                DataGridView regularGridView = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                };
                regularTab.Controls.Add(regularGridView);
                OutputTabControl.TabPages.Add(regularTab);

                // Запускаем анализ регулярных выражений
                Regular.Analyze(inputText, GetCurrentRichTextBox(), regularGridView);
                */

                // рекурсия
                /*
                TabPage existingRecursionTab = OutputTabControl.TabPages
            .Cast<TabPage>()
            .FirstOrDefault(tp => tp.Text == "Рекурсивный спуск");

                if (existingRecursionTab != null)
                {
                    OutputTabControl.TabPages.Remove(existingRecursionTab);
                    existingRecursionTab.Dispose();
                }

                TabPage recursionTab = new TabPage("Рекурсивный спуск");
                SplitContainer splitContainer = new SplitContainer
                {
                    Dock = DockStyle.Fill,
                    Orientation = Orientation.Vertical
                };

                DataGridView recursionGridView = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                };

                TextBox errorTextBox = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    ReadOnly = true,
                    BackColor = SystemColors.Window,
                    Visible = false
                };

                splitContainer.Panel1.Controls.Add(recursionGridView);
                splitContainer.Panel2.Controls.Add(errorTextBox);
                recursionTab.Controls.Add(splitContainer);
                OutputTabControl.TabPages.Add(recursionTab);

                // Run recursive descent parser
                Recursion.Analyze(
                    inputText,
                    GetCurrentRichTextBox(),
                    recursionGridView,
                    errorTextBox
                );*/
                TabPage existingParserTab = OutputTabControl.TabPages
            .Cast<TabPage>()
            .FirstOrDefault(tp => tp.Text == "Unsigned Number Parser");

                if (existingParserTab != null)
                {
                    OutputTabControl.TabPages.Remove(existingParserTab);
                    existingParserTab.Dispose();
                }

                TabPage parserTab = new TabPage("Unsigned Number Parser");
                SplitContainer splitContainer = new SplitContainer
                {
                    Dock = DockStyle.Fill,
                    Orientation = Orientation.Vertical
                };

                DataGridView parserGridView = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                };

                TextBox errorTextBox = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    ReadOnly = true,
                    BackColor = SystemColors.Window
                };

                splitContainer.Panel1.Controls.Add(parserGridView);
                splitContainer.Panel2.Controls.Add(errorTextBox);
                parserTab.Controls.Add(splitContainer);
                OutputTabControl.TabPages.Add(parserTab);

                // Запускаем анализатор
                UnsignedNumberParser parser = new UnsignedNumberParser();
                parser.Analyze(inputText, parserGridView, errorTextBox);
            }
            else
            {
                MessageBox.Show("Нет открытых файлов для анализа", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private DataGridView GetOutputDataGridView()
        {
            // Создаем DataGridView в первой вкладке OutputTabControl, если его нет
            if (OutputTabControl.TabPages[0].Controls.Count == 0)
            {
                var dgv = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    RowHeadersVisible = false
                };
                OutputTabControl.TabPages[0].Controls.Add(dgv);
            }
            return OutputTabControl.TabPages[0].Controls[0] as DataGridView;
        }

        private RichTextBox GetCurrentRichTextBox()
        {
            return InputTabControl.SelectedTab.Controls[0] as RichTextBox;
        }

        private void MarkTextChanged()
        {
            isTextChanged = true;
        }

        private void CheckAndSave()
        {
            if (isTextChanged && InputTabControl.TabCount > 0 && GetCurrentRichTextBox() != null)
            {
                var result = MessageBox.Show(
                    "Сохранить изменения в файле перед закрытием?",
                    "Сохранение",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    сохранитьToolStripMenuItem_Click(null, null);

                    // Проверяем, было ли сохранение отменено
                    if (isTextChanged) // Если после сохранения флаг не сбросился
                        throw new OperationCanceledException();
                }
                else if (result == DialogResult.Cancel)
                {
                    throw new OperationCanceledException();
                }
            }
        }

        #region Файл
        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CheckAndSave();

                var tabPage = new TabPage("Новый файл");
                var richTextBox = new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Consolas", 10),
                    AcceptsTab = true
                };
                richTextBox.TextChanged += InputRichTextBox_TextChanged;
                tabPage.Controls.Add(richTextBox);

                InputTabControl.TabPages.Add(tabPage);
                InputTabControl.SelectedTab = tabPage;

                currentFilePath = string.Empty;
                isTextChanged = false;
                undoStack.Clear();
                redoStack.Clear();
            }
            catch (OperationCanceledException) { }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CheckAndSave();

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Text Files|*.txt|All Files|*.*";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        currentFilePath = openFileDialog.FileName;

                        var tabPage = new TabPage(Path.GetFileName(currentFilePath));
                        var richTextBox = new RichTextBox
                        {
                            Dock = DockStyle.Fill,
                            Font = new Font("Consolas", 10),
                            AcceptsTab = true,
                            Text = File.ReadAllText(currentFilePath)
                        };
                        richTextBox.TextChanged += InputRichTextBox_TextChanged;
                        tabPage.Controls.Add(richTextBox);

                        InputTabControl.TabPages.Add(tabPage);
                        InputTabControl.SelectedTab = tabPage;

                        isTextChanged = false;
                        undoStack.Clear();
                        redoStack.Clear();
                    }
                }
            }
            catch (OperationCanceledException) { }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                сохранитьКакToolStripMenuItem_Click(sender, e);
            }
            else
            {
                File.WriteAllText(currentFilePath, GetCurrentRichTextBox().Text);
                isTextChanged = false;
                InputTabControl.SelectedTab.Text = Path.GetFileName(currentFilePath);
            }
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files|*.txt|All Files|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    currentFilePath = saveFileDialog.FileName;
                    File.WriteAllText(currentFilePath, GetCurrentRichTextBox().Text);
                    isTextChanged = false;
                    InputTabControl.SelectedTab.Text = Path.GetFileName(currentFilePath);
                }
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close(); // Просто закрываем форму, FormClosing сам вызовет CheckAndSave()
        }
        #endregion

        #region Правка
        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 0 && InputTabControl.TabCount > 0)
            {
                isUndoRedoOperation = true;
                redoStack.Push(GetCurrentRichTextBox().Text);
                GetCurrentRichTextBox().Text = undoStack.Pop();
                isUndoRedoOperation = false;
            }
        }

        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0 && InputTabControl.TabCount > 0)
            {
                isUndoRedoOperation = true;
                undoStack.Push(GetCurrentRichTextBox().Text);
                GetCurrentRichTextBox().Text = redoStack.Pop();
                isUndoRedoOperation = false;
            }
        }

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (InputTabControl.TabCount > 0)
                GetCurrentRichTextBox().Cut();
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (InputTabControl.TabCount > 0)
                GetCurrentRichTextBox().Copy();
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (InputTabControl.TabCount > 0)
                GetCurrentRichTextBox().Paste();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (InputTabControl.TabCount > 0 && !string.IsNullOrEmpty(GetCurrentRichTextBox().SelectedText))
            {
                GetCurrentRichTextBox().SelectedText = string.Empty;
                isTextChanged = true;
            }
        }

        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (InputTabControl.TabCount > 0)
                GetCurrentRichTextBox().SelectAll();
        }
        #endregion

        #region Обработчики кнопок
        private void PlusButton_Click(object sender, EventArgs e)
        {
            создатьToolStripMenuItem_Click(sender, e);
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            открытьToolStripMenuItem_Click(sender, e);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            сохранитьToolStripMenuItem_Click(sender, e);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            отменитьToolStripMenuItem_Click(sender, e);
        }

        private void RepeatButton_Click(object sender, EventArgs e)
        {
            повторитьToolStripMenuItem_Click(sender, e);
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            копироватьToolStripMenuItem_Click(sender, e);
        }

        private void CutButton_Click(object sender, EventArgs e)
        {
            вырезатьToolStripMenuItem_Click(sender, e);
        }

        private void PasteButton_Click(object sender, EventArgs e)
        {
            вставитьToolStripMenuItem_Click(sender, e);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            RunAnalyzer();
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            вызовСправкиToolStripMenuItem_Click(sender, e);
        }

        private void InfoButton_Click(object sender, EventArgs e)
        {
            оПрограммеToolStripMenuItem_Click(sender, e);
        }
        #endregion

        #region Справка
        private void вызовСправкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm helpForm = new HelpForm();
            helpForm.ShowDialog();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoForm infoForm = new InfoForm();
            infoForm.ShowDialog();
        }
        #endregion

        private void InputRichTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!isUndoRedoOperation && InputTabControl.TabCount > 0)
            {
                undoStack.Push(GetCurrentRichTextBox().Text);
                redoStack.Clear();
            }
            MarkTextChanged();
        }

        private void InputTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InputTabControl.TabCount > 0)
            {
                isTextChanged = false;
                undoStack.Clear();
                redoStack.Clear();
            }
        }



        private void Compiler_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                CheckAndSave();
            }
            catch (OperationCanceledException)
            {
                e.Cancel = true; // Отменяем закрытие формы, если пользователь нажал "Отмена"
            }
        }

        private void CloseTab(int tabIndex)
        {
            try
            {
                // Сохраняем ссылки на текущие элементы
                var tabPage = InputTabControl.TabPages[tabIndex];
                var richTextBox = tabPage.Controls[0] as RichTextBox;

                // Проверяем изменения
                if (isTextChanged)
                {
                    var result = MessageBox.Show($"Сохранить изменения в файле \"{tabPage.Text}\"?",
                                              "Сохранение",
                                              MessageBoxButtons.YesNoCancel);

                    if (result == DialogResult.Yes)
                    {
                        if (string.IsNullOrEmpty(currentFilePath))
                        {
                            сохранитьКакToolStripMenuItem_Click(null, null);
                            // Если пользователь отменил "Сохранить как", не закрываем вкладку
                            if (string.IsNullOrEmpty(currentFilePath))
                                return;
                        }
                        else
                        {
                            File.WriteAllText(currentFilePath, richTextBox.Text);
                        }
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return; // Не закрываем вкладку, если отмена
                    }
                }

                // Закрываем вкладку
                InputTabControl.TabPages.RemoveAt(tabIndex);

                // Очищаем историю, если вкладок не осталось
                if (InputTabControl.TabCount == 0)
                {
                    undoStack.Clear();
                    redoStack.Clear();
                    currentFilePath = string.Empty;
                    isTextChanged = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при закрытии вкладки: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Остальные методы оставляем пустыми, как у вас было
        private void файлToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void правкаToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void текстToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TextInfoForm textInfoForm = new TextInfoForm();
            //textInfoForm.ShowDialog();
        }
        private void пускToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            RunAnalyzer();
        }
        private void справкаToolStripMenuItem1_Click(object sender, EventArgs e) { }
        private void постановкаЗадачиToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowTextInfoForm("Постановка задачи");
        }
        private void грамматикаToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowTextInfoForm("Грамматика");
        }
        private void классификацияГрамматикиToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowTextInfoForm("Классификация грамматики");
        }
        private void методАнализаToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowTextInfoForm("Метод анализа");
        }
        private void тестовыйПримерToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowTextInfoForm("Тестовый пример");
        }
        private void OutputTabControl_SelectedIndexChanged(object sender, EventArgs e) { }
        private void Compiler_Load(object sender, EventArgs e) { }

        private void списокЛитературыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTextInfoForm("Список литературы");
        }

        private void исходныйКодПрограммыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTextInfoForm("Исходный код");
        }

        private void диToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTextInfoForm("Диагностика ошибок");
        }

        private void ShowTextInfoForm(string tabName)
        {
            // Создаем новую форму каждый раз
            TextInfoForm textInfoForm = new TextInfoForm();

            // Находим TabControl в форме
            var tabControl = textInfoForm.Controls[0] as TabControl;

            // Находим нужную вкладку и делаем её активной
            foreach (TabPage tab in tabControl.TabPages)
            {
                if (tab.Text == tabName)
                {
                    tabControl.SelectedTab = tab;
                    break;
                }
            }

            // Показываем форму как модальное окно
            textInfoForm.ShowDialog();
        }

        // В классе Compiler добавьте следующие методы для обработки кнопок:

        private void button1_Click(object sender, EventArgs e)
        {
            RunSpecificAnalyzer(1); // Для кнопки 1 - знаки препинания
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RunSpecificAnalyzer(2); // Для кнопки 2 - Amex Card
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RunSpecificAnalyzer(3); // Для кнопки 3 - RGB цвета
        }

        private void RunSpecificAnalyzer(int analyzerType)
        {
            if (InputTabControl.TabCount > 0 && GetCurrentRichTextBox() != null)
            {
                string inputText = GetCurrentRichTextBox().Text;

                // Создаем новую вкладку для вывода результатов
                TabPage existingRegularTab = OutputTabControl.TabPages
                    .Cast<TabPage>()
                    .FirstOrDefault(tp => tp.Text == "Regular Expressions");

                if (existingRegularTab != null)
                {
                    OutputTabControl.TabPages.Remove(existingRegularTab);
                    existingRegularTab.Dispose();
                }

                TabPage regularTab = new TabPage("Regular Expressions");
                DataGridView regularGridView = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                };
                regularTab.Controls.Add(regularGridView);
                OutputTabControl.TabPages.Add(regularTab);

                // Запускаем выбранный анализатор
                switch (analyzerType)
                {
                    case 1:
                        Regular.AnalyzePunctuation(inputText, GetCurrentRichTextBox(), regularGridView);
                        break;
                    case 2:
                        Regular.AnalyzeAmexCards(inputText, GetCurrentRichTextBox(), regularGridView);
                        break;
                    case 3:
                        Regular.AnalyzeRgbColors(inputText, GetCurrentRichTextBox(), regularGridView);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Нет открытых файлов для анализа", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
    }
}