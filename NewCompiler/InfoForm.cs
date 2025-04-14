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
    public partial class InfoForm : Form
    {
        public InfoForm()
        {
            InitializeComponent();
            InitializeInfoContent();
        }
        private void InitializeInfoContent()
        {
            RichTextBox infoBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = SystemColors.Window
            };


            infoBox.SelectionFont = new Font("Arial", 12, FontStyle.Bold);
            infoBox.AppendText("Программа:\n");
            infoBox.SelectionFont = new Font("Arial", 11, FontStyle.Regular);
            infoBox.AppendText("Версия: 1.0\n");
            infoBox.SelectionFont = new Font("Arial", 11, FontStyle.Regular);
            infoBox.AppendText("Разработчик: Зачиняева В.К.\n\n");

            infoBox.SelectionFont = new Font("Arial", 12, FontStyle.Bold);
            infoBox.AppendText("Описание:\n");

            infoBox.SelectionFont = new Font("Arial", 11, FontStyle.Regular);
            infoBox.AppendText("Данное приложение предназначено для работы с текстовыми файлами. Оно включает в себя функции создания, редактирования, сохранения и открытия файлов. Также доступны функции отмены, повтора действий, работы с буфером обмена и вызова справочной информации.\n\n");

            this.Controls.Add(infoBox);
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {

        }
    }
}
