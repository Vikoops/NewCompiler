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
            // Создаем и настраиваем WebBrowser
            WebBrowser webBrowser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                IsWebBrowserContextMenuEnabled = false, // Отключаем контекстное меню
                ScrollBarsEnabled = true              // Включаем скроллбар
            };

            // Генерируем HTML-контент
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
                    }
                    .version {
                        font-weight: bold;
                        margin-bottom: 15px;
                    }
                    .description {
                        margin-top: 10px;
                        text-align: justify;
                    }
                </style>
            </head>
            <body>
                <h1>Программа</h1>
                <div class='version'>Версия: 1.0</div>
                <div>Разработчик: Зачиняева В.К.</div>

                <h2>Описание</h2>
                <div class='description'>
                    Данное приложение предназначено для работы с текстовыми файлами. Оно включает в себя функции создания, 
                    редактирования, сохранения и открытия файлов. Также доступны функции отмены, повтора действий, 
                    работы с буфером обмена и вызова справочной информации.
                </div>
            </body>
            </html>";

            // Загружаем HTML
            webBrowser.DocumentText = htmlContent;
            this.Controls.Add(webBrowser);
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
            // Дополнительные действия при загрузке формы
        }
    }
}