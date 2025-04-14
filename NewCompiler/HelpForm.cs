using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            RichTextBox helpBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = SystemColors.Window
            };

            helpBox.SelectionFont = new Font("Arial", 14, FontStyle.Regular);
            helpBox.AppendText("Файл:\n");
            helpBox.SelectionBullet = true;
            helpBox.SelectionFont = new Font("Arial", 12, FontStyle.Regular);
            helpBox.AppendText("Создать - создание нового файла.\n");
            helpBox.SelectionFont = new Font("Arial", 12, FontStyle.Regular);
            helpBox.AppendText("Открыть - открытие существующего файла в формате .txt.\n");
            helpBox.SelectionFont = new Font("Arial", 12, FontStyle.Regular);
            helpBox.AppendText("Сохранить - сохранение текущего файла.\n");
            helpBox.SelectionFont = new Font("Arial", 12, FontStyle.Regular);
            helpBox.AppendText("Сохранить как - сохранение файла под новым именем.\n");
            helpBox.SelectionFont = new Font("Arial", 12, FontStyle.Regular);
            helpBox.AppendText("Выход - выйти из программы.\n");
            helpBox.SelectionBullet = false;

            helpBox.SelectionFont = new Font("Arial", 14, FontStyle.Regular);
            helpBox.AppendText("\nПравка:\n");
            helpBox.SelectionBullet = true;
            helpBox.SelectionFont = new Font("Arial", 12, FontStyle.Regular);
            helpBox.AppendText("Отменить/Повторить - отмена последнего/повтор отменненого действия.\n");
            helpBox.SelectionFont = new Font("Arial", 12, FontStyle.Regular);
            helpBox.AppendText("Вырезать - выделенный текст вырезается и копируется в буфер обмена.\n");
            helpBox.SelectionFont = new Font("Arial", 12, FontStyle.Regular);
            helpBox.AppendText("Копировать - выделенный текст копируется в буфер обмена.\n");
            helpBox.SelectionFont = new Font("Arial", 12, FontStyle.Regular);
            helpBox.AppendText("Вставить - текст из буфера обмена вставляется в окно редактирования.\n");
            helpBox.SelectionFont = new Font("Arial", 12, FontStyle.Regular);
            helpBox.AppendText("Удалить - удаление выделенного текста.\n");
            helpBox.SelectionFont = new Font("Arial", 12, FontStyle.Regular);
            helpBox.AppendText("Выделить все - выделяется весь текст из окна редактирования.\n");

            helpBox.SelectionBullet = false;

            this.Controls.Add(helpBox);
        }

        private void HelpForm_Load(object sender, EventArgs e)
        {

        }
    }
}
