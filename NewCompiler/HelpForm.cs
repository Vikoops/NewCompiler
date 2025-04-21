using System;
using System.Windows.Forms;

namespace NewCompiler
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
            InitializeHelpContent();
        }

        private void InitializeHelpContent()
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
                        font-family: 'Segoe UI', Arial, sans-serif;
                        padding: 20px;
                        color: #333;
                        line-height: 1.6;
                    }
                    h1 {
                        color: #2c3e50;
                        font-size: 18px;
                        margin-bottom: 10px;
                    }
                    h2 {
                        color: #2980b9;
                        font-size: 16px;
                        margin: 20px 0 10px 0;
                        border-bottom: 1px solid #eee;
                        padding-bottom: 5px;
                    }
                    ul {
                        margin: 5px 0;
                        padding-left: 25px;
                    }
                    li {
                        margin-bottom: 8px;
                    }
                    .command {
                        font-weight: bold;
                        color: #e74c3c;
                    }
                </style>
            </head>
            <body>
                <h1>Руководство пользователя</h1>
                
                <h2>Файл</h2>
                <ul>
                    <li><span class='command'>Создать</span> - создание нового файла</li>
                    <li><span class='command'>Открыть</span> - открытие существующего файла в формате .txt</li>
                    <li><span class='command'>Сохранить</span> - сохранение текущего файла</li>
                    <li><span class='command'>Сохранить как</span> - сохранение файла под новым именем</li>
                    <li><span class='command'>Выход</span> - выход из программы</li>
                </ul>

                <h2>Правка</h2>
                <ul>
                    <li><span class='command'>Отменить/Повторить</span> - отмена последнего/повтор отмененного действия</li>
                    <li><span class='command'>Вырезать</span> - выделенный текст вырезается и копируется в буфер обмена</li>
                    <li><span class='command'>Копировать</span> - выделенный текст копируется в буфер обмена</li>
                    <li><span class='command'>Вставить</span> - текст из буфера обмена вставляется в окно редактирования</li>
                    <li><span class='command'>Удалить</span> - удаление выделенного текста</li>
                    <li><span class='command'>Выделить все</span> - выделение всего текста в окне редактирования</li>
                </ul>
            </body>
            </html>";

            webBrowser.DocumentText = htmlContent;
            this.Controls.Add(webBrowser);
        }

        private void HelpForm_Load(object sender, EventArgs e)
        {
            // Дополнительные действия при загрузке
        }
    }
}