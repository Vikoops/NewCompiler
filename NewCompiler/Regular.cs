using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace NewCompiler
{
    internal class Regular
    {
        // Регулярные выражения для задач
        private static readonly Regex PunctuationRx = new Regex(@"[^\w\s-]|_");
        //private static readonly Regex AmexCardRx = new Regex(@"\b3[47]\d{13}\b");
        private static readonly Regex AmexCardRx = new Regex(@"\b3[47]\d{2}[-\s]?\d{6}[-\s]?\d{5}\b");
        private static readonly Regex RgbColorRx = new Regex(@"(?:^|\s)rgb\(\s*(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\s*,\s*(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\s*,\s*(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\s*\)",
    RegexOptions.IgnoreCase);

        //private static readonly Regex RgbColorRx = new Regex(@"\brgb\(\s*(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\s*,\s*(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\s*,\s*(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\s*\)\b", RegexOptions.IgnoreCase);

        public static void Analyze(string inputText, RichTextBox richTextBox, DataGridView dataGridView)
        {
            SetupGrid(dataGridView);

            // Поиск знаков препинания (кроме дефиса)
            Search(PunctuationRx, "Знаки препинания", null, richTextBox, dataGridView);

            // Поиск номеров карт Amex
            Search(AmexCardRx, "Amex Card", ValidateAmexCard, richTextBox, dataGridView);

            // Поиск RGB цветов
            Search(RgbColorRx, "RGB цвет", ValidateRgbColor, richTextBox, dataGridView);
        }

        private static void SetupGrid(DataGridView dataGridView)
        {
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();

            dataGridView.Columns.Add("Type", "Тип");
            dataGridView.Columns.Add("Value", "Значение");
            dataGridView.Columns.Add("Position", "Позиция");
            dataGridView.Columns.Add("Valid", "Валидность");
        }

        private static void Search(Regex rx, string type, Func<string, bool> validator, RichTextBox richTextBox, DataGridView dataGridView)
        {
            foreach (Match m in rx.Matches(richTextBox.Text))
            {
                bool ok = validator?.Invoke(m.Value) ?? true;
                dataGridView.Rows.Add(type, m.Value, m.Index, ok ? "✔" : "✖");

                if (ok)
                {
                    richTextBox.Select(m.Index, m.Length);
                    richTextBox.SelectionBackColor = Color.Yellow;
                }
            }
        }

        // Валидация номера карты Amex (дополнительная проверка длины)
        /*private static bool ValidateAmexCard(string s)
        {
            return s.Length == 15;
        }
        */
        private static bool ValidateAmexCard(string s)
        {
            string cleaned = s.Replace(" ", "").Replace("-", "");
            return cleaned.Length == 15;
        }

        // Валидация RGB цвета (уже проверено регулярным выражением)
        private static bool ValidateRgbColor(string s)
        {
            return true; // Все проверки уже сделаны в регулярном выражении
        }
    }
}