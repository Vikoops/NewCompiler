using System;
using System.Windows.Forms;

namespace NewCompiler
{
    public partial class InfoForm : Form
    {
        public InfoForm()
        {
            InitializeComponent();
            InitializeInfoContent();
        }

        private void InitializeInfoContent()
        {
            WebBrowser webBrowser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                IsWebBrowserContextMenuEnabled = false,
                ScrollBarsEnabled = true
            };

            string htmlContent = @"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {
                        font-family: Arial;
                        padding: 15px;
                        color: #333;
                        line-height: 1.6;
                    }
                    h1 {
                        color: #2c3e50;
                        border-bottom: 1px solid #3498db;
                        padding-bottom: 5px;
                    }
                    h2 {
                        color: #2980b9;
                        margin-top: 20px;
                        border-bottom: 1px solid #eee;
                        padding-bottom: 3px;
                    }
                    h3 {
                        color: #16a085;
                        margin: 15px 0 5px 0;
                    }
                    .version {
                        font-weight: bold;
                        margin-bottom: 15px;
                    }
                    .description {
                        margin-top: 10px;
                        text-align: justify;
                    }
                    .feature-list {
                        margin-left: 20px;
                    }
                    .feature-list li {
                        margin-bottom: 8px;
                    }
                    .code {
                        font-family: Consolas, monospace;
                        background: #f5f5f5;
                        padding: 2px 5px;
                        border-radius: 3px;
                    }
                </style>
            </head>
            <body>
                <h1>Программа</h1>
                <div class='version'>Версия: 1.0</div>
                <div>Разработчик: Зачиняева В.К.</div>

                <h2>Описание</h2>
                <div class='description'>
                    Данное приложение предназначено для работы с текстовыми файлами и включает:
                </div>
                <ul class='feature-list'>
                    <li>Функции создания, редактирования и сохранения файлов</li>
                    <li>Операции с буфером обмена (копирование/вставка)</li>
                    <li>Механизмы отмены и повтора действий</li>
                    <li>Лексический анализатор</li>
                </ul>

                <h2>Лексический анализатор</h2>
                <div class='description'>
                    Встроенный анализатор проверяет корректность объявления строковых констант в Java.
                </div>

                <h3>Поддерживаемые конструкции</h3>
                <ul class='feature-list'>
                    <li>Объявление с модификатором <span class='code'>final</span>:
                        <br><span class='code'>final String str123 = ""Hello"";</span></li>
                    <li>Строки с Unicode-символами:
                        <br><span class='code'>final String greeting = ""Привет, мир!"";</span></li>
                    <li>Строки с escape-последовательностями:
                        <br><span class='code'>final String path = ""C:\\Program Files\\"";</span></li>
                </ul>

                <h3>Особенности работы</h3>
                <ul class='feature-list'>
                    <li>Проверка синтаксиса объявлений</li>
                    <li>Выделение лексем (ключевые слова, идентификаторы, строки)</li>
                    <li>Обнаружение ошибок с указанием позиции</li>
                    <li>Поддержка многострочного анализа</li>
                </ul>

                <h3>Пример корректного ввода</h3>
                <div class='code' style='padding: 10px; margin: 10px 0;'>
                    final String message = ""Hello World"";<br>
                    final String empty123_ = """";<br>
                    final String path = ""C:\\\\Users\\\\"";<br>
                </div>
            </body>
            </html>";

            webBrowser.DocumentText = htmlContent;
            this.Controls.Add(webBrowser);
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
           
        }
    }
}